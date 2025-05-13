using Serilog;
using Domain.Manager;
using Services.External_Services;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration, sectionName: "Serilog")
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddTransient<GiftStoreServices>();
builder.Services.AddTransient<IGiftManager, GiftManager>();
builder.Services.AddHttpClient<GiftStoreServices>();
builder.Services.AddHttpClient<IPatientCodeService, PatientCodeService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
});


builder.Services.AddSingleton<IPatientManager>(sp =>
    new PatientManager(
        builder.Configuration["Data:PatientsFile"],
        sp.GetRequiredService<ILogger<PatientManager>>(),
        sp.GetRequiredService<IPatientCodeService>()     // ← aquí
    ));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context => 
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        if (feature != null) 
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(feature.Error, "Error no controlado");
        }
        var result = JsonSerializer.Serialize(new
        {
            Message = "Ocurri� un error interno. Por favor int�ntalo de nuevo m�s tarde."
        });
        await context.Response.WriteAsync(result);
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
