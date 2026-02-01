using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifly.Builders;
using Notifly.Mappers;
using Notlify.Core00;
using Notlify.Core00.Interfaces;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IAmadeusClient, AmadeusClient>();
builder.Services.AddSingleton<IWhatsAppClient, WhatsAppClient>();
builder.Services.AddSingleton<IFlightInfoMapper, FlightInfoMapper>();
builder.Services.AddSingleton<IWhatsAppNotificationBuilder, WhatsAppNotificationBuilder>();


builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();