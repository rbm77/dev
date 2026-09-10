using System.Text;
using Buslogix.Features.EmailIngestion;
using Buslogix.Features.EmailIngestion.Abstractions;
using Buslogix.Features.PaymentMatching;
using Buslogix.Features.PaymentMatching.Abstractions;
using Buslogix.Features.MessageExtraction;
using Buslogix.Features.MessageExtraction.Abstractions;
using Buslogix.Common.BackgroundJobs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using static Buslogix.Common.Shared.Enums;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("extraction-patterns.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableDateOnlyJsonConverter());
    });

builder.Services.AddSingleton<ILogHandler, LogHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") ?? "";
builder.Services.AddScoped<IDataAccess>(provider => new MySqlDataAccess(connectionString));
builder.Services.AddCompanies();
builder.Services.AddUsers();
builder.Services.AddRoles();
builder.Services.AddEmployees();
builder.Services.AddSalaries();
builder.Services.AddVehicles();
builder.Services.AddRoutes();
builder.Services.AddGrades();
builder.Services.AddStudents();
builder.Services.AddContacts();
builder.Services.AddMaintenances();
builder.Services.AddIncidents();
builder.Services.AddCustomTransports();
builder.Services.AddExpenses();
builder.Services.AddFuelExpenses();
builder.Services.AddMaintenanceExpenses();
builder.Services.AddSalaryExpenses();
builder.Services.AddIncidentExpenses();
builder.Services.AddPaymentPeriodRequests();
builder.Services.AddPaymentPeriods();
builder.Services.AddVacations();
builder.Services.AddSpecificExemptions();
builder.Services.AddPeriodicExemptions();
builder.Services.AddPayments();
builder.Services.AddPaymentRequests();
builder.Services.AddReports();
builder.Services.AddEmailAccounts();
builder.Services.AddEmailSenders();

string secretKey = builder.Configuration["JWT:SecretKey"] ?? "";
byte[] key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ServiceAuth.SchemeName, _ => { });

builder.Services.AddAuthorization(options =>
{
    foreach (string permission in PermissionMap.PermissionToCode.Keys)
    {
        options.AddPolicy(permission, policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));
    }

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddOpenApi();
builder.Services.AddMessageExtraction();
builder.Services.AddEmailIngestion();
builder.Services.AddPaymentMatching();

builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<EmailPollQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        EmailIngestionResult result = await services.GetRequiredService<IEmailIngestionService>().ProcessAllAccounts(ct);
        await services.GetRequiredService<ILogHandler>().WriteLog(
            $"Email poll completed: {result.AccountsChecked} accounts checked, {result.MessagesFound} messages found, {result.MessagesQueued} queued for extraction.",
            LogType.Info);
    },
    sp.GetRequiredService<ILogHandler>()));

builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<PaymentPeriodScheduleQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        SchedulePaymentPeriodsResult result = await services.GetRequiredService<IPaymentPeriodRepository>().SchedulePaymentPeriods();
        await services.GetRequiredService<ILogHandler>().WriteLog(
            $"Payment period scheduling completed: {result.ProcessedCount} processed, {result.ScheduledCount} scheduled, {result.SkippedCount} skipped, {result.FailedCount} failed.",
            LogType.Info);
    },
    sp.GetRequiredService<ILogHandler>()));

builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<PaymentAutoApprovalQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        AutoApprovalResult result = await services.GetRequiredService<IPaymentRequestService>().AutoApprovePaymentRequests();
        await services.GetRequiredService<ILogHandler>().WriteLog(
            $"Payment auto-approval completed: {result.ProcessedCount} processed, {result.ApprovedCount} approved, {result.FailedCount} failed.",
            LogType.Info);
    },
    sp.GetRequiredService<ILogHandler>()));

builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<PaymentMatchSweepQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        MatchSweepResult result = await services.GetRequiredService<IPaymentMatchingRepository>().MatchPendingPaymentRequests();
        await services.GetRequiredService<ILogHandler>().WriteLog(
            $"Payment match sweep completed: {result.MatchedCount} matched. Auto-approval of newly-validated requests runs separately via auto_approve_payment_requests.",
            LogType.Info);
    },
    sp.GetRequiredService<ILogHandler>()));

builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<MessageExtractionRetryQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        int requeuedCount = await services.GetRequiredService<IMessageExtractionMaintenanceService>().RetryFailedExtractions(ct);
        await services.GetRequiredService<ILogHandler>().WriteLog(
            $"Message extraction retry completed: {requeuedCount} failed message(s) requeued for extraction.",
            LogType.Info);
    },
    sp.GetRequiredService<ILogHandler>()));

builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<MessageExtractionPurgeQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        MessageExtractionPurgeResult result = await services.GetRequiredService<IMessageExtractionMaintenanceService>().PurgeExpiredRecords();
        await services.GetRequiredService<ILogHandler>().WriteLog(
            $"Message extraction purge completed: {result.FailuresDeletedCount} failure record(s) and {result.ResultsDeletedCount} result record(s) older than 3 days deleted.",
            LogType.Info);
    },
    sp.GetRequiredService<ILogHandler>()));

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<CompanyMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
