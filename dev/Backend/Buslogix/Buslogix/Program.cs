using System.Text;
using Buslogix.DataAccess;
using Buslogix.EmailIngestion;
using Buslogix.EmailIngestion.Abstractions;
using Buslogix.Handlers;
using Buslogix.Interfaces;
using Buslogix.Matching;
using Buslogix.Matching.Abstractions;
using Buslogix.MessageExtraction;
using Buslogix.MessageExtraction.Abstractions;
using Buslogix.Middlewares;
using Buslogix.Models;
using Buslogix.Models.DTO;
using Buslogix.Repositories;
using Buslogix.Services;
using Buslogix.Triggers;
using Buslogix.Triggers.Queues;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using static Buslogix.Utilities.Enums;
using TokenHandler = Buslogix.Handlers.TokenHandler;

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
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenHandler, TokenHandler>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ISalaryRepository, SalaryRepository>();
builder.Services.AddScoped<ISalaryService, SalaryService>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<ICustomTransportRepository, CustomTransportRepository>();
builder.Services.AddScoped<ICustomTransportService, CustomTransportService>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IFuelExpenseRepository, FuelExpenseRepository>();
builder.Services.AddScoped<IFuelExpenseService, FuelExpenseService>();
builder.Services.AddScoped<IMaintenanceExpenseRepository, MaintenanceExpenseRepository>();
builder.Services.AddScoped<IMaintenanceExpenseService, MaintenanceExpenseService>();
builder.Services.AddScoped<ISalaryExpenseRepository, SalaryExpenseRepository>();
builder.Services.AddScoped<ISalaryExpenseService, SalaryExpenseService>();
builder.Services.AddScoped<IIncidentExpenseRepository, IncidentExpenseRepository>();
builder.Services.AddScoped<IIncidentExpenseService, IncidentExpenseService>();
builder.Services.AddScoped<IPaymentPeriodRequestRepository, PaymentPeriodRequestRepository>();
builder.Services.AddScoped<IPaymentPeriodRequestService, PaymentPeriodRequestService>();
builder.Services.AddScoped<IPaymentPeriodRepository, PaymentPeriodRepository>();
builder.Services.AddScoped<IPaymentPeriodService, PaymentPeriodService>();
builder.Services.AddScoped<IVacationRepository, VacationRepository>();
builder.Services.AddScoped<IVacationService, VacationService>();
builder.Services.AddScoped<ISpecificExemptionRepository, SpecificExemptionRepository>();
builder.Services.AddScoped<ISpecificExemptionService, SpecificExemptionService>();
builder.Services.AddScoped<IPeriodicExemptionRepository, PeriodicExemptionRepository>();
builder.Services.AddScoped<IPeriodicExemptionService, PeriodicExemptionService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRequestRepository, PaymentRequestRepository>();
builder.Services.AddScoped<IPaymentRequestService, PaymentRequestService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddScoped<IEmailAccountRepository, EmailAccountRepository>();
builder.Services.AddScoped<IEmailAccountService, EmailAccountService>();
builder.Services.AddScoped<IEmailSenderRepository, EmailSenderRepository>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();


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

// Background triggers: each endpoint below only signals its TriggerQueue and
// returns immediately - the actual work runs on its own TriggerWorker
// instance, in its own DI scope, never overlapping with itself.
builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<EmailPollQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        EmailIngestionResult result = await services.GetRequiredService<IEmailIngestionService>().ProcessAllAccountsAsync(ct);
        await services.GetRequiredService<ILogHandler>().WriteLog(
            $"Email poll completed: {result.AccountsChecked} accounts checked, {result.MessagesFound} messages found, {result.MessagesQueued} queued for extraction.",
            LogType.Info);
    },
    sp.GetRequiredService<ILogHandler>()));

builder.Services.AddSingleton<PaymentPeriodScheduleQueue>();
builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<PaymentPeriodScheduleQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        SchedulePaymentPeriodsResult result = await services.GetRequiredService<IPaymentPeriodService>().SchedulePaymentPeriods();
        await services.GetRequiredService<ILogHandler>().WriteLog(
            $"Payment period scheduling completed: {result.ProcessedCount} processed, {result.ScheduledCount} scheduled, {result.SkippedCount} skipped, {result.FailedCount} failed.",
            LogType.Info);
    },
    sp.GetRequiredService<ILogHandler>()));

builder.Services.AddSingleton<PaymentAutoApprovalQueue>();
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

builder.Services.AddSingleton<PaymentMatchSweepQueue>();
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

builder.Services.AddSingleton<MessageExtractionRetryQueue>();
builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<MessageExtractionRetryQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        int requeuedCount = await services.GetRequiredService<IMessageExtractionMaintenanceService>().RetryFailedExtractionsAsync(ct);
        await services.GetRequiredService<ILogHandler>().WriteLog(
            $"Message extraction retry completed: {requeuedCount} failed message(s) requeued for extraction.",
            LogType.Info);
    },
    sp.GetRequiredService<ILogHandler>()));

builder.Services.AddSingleton<MessageExtractionPurgeQueue>();
builder.Services.AddHostedService(sp => new TriggerWorker(
    sp.GetRequiredService<MessageExtractionPurgeQueue>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    async (services, ct) =>
    {
        MessageExtractionPurgeResult result = await services.GetRequiredService<IMessageExtractionMaintenanceService>().PurgeExpiredRecordsAsync();
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
