using Buslogix.DataAccess;
using Buslogix.Interfaces;
using Buslogix.Repositories;
using Buslogix.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") ?? "";
builder.Services.AddScoped<IDataAccess>(provider => new MySqlDataAccess(connectionString));
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
