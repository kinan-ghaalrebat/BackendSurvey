using System.Text.Json.Serialization;
using SurveyManagement.API.Middleware;
using SurveyManagement.Application;
using SurveyManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    // Accept and return enums as text ("SingleSelect") instead of numbers
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
