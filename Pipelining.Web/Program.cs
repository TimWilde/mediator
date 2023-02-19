using Mediator.Web.Mediation.Implementation;
using Mediator.Web.Messaging.Handlers;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

// -- Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediation( config =>
                               {
                                  config.AddHandler<RequestHandlers.WeatherRequestHandler>()
                                        .AddHandler<RequestHandlers.SetSomethingHandler>();

                                  config.AddStreamHandler<StreamHandlers.TemperatureStreamHandler>()
                                        .AddStreamHandler<StreamHandlers.OtherTempStreamHandler>();

                                  config.AddNotificationHandler<NotificationHandlers.FirstNotificationHandler>()
                                        .AddNotificationHandler<NotificationHandlers.SecondNotificationHandler>();

                                  config.AddPipeline( p => p.AddOperation<PipelineHandlers.StringToDateTime>()
                                                            .AddOperation<PipelineHandlers.DateTimeToDateOnly>()
                                                            .AddOperation<PipelineHandlers.DateOnlyToDaysSince>() );
                               } );

WebApplication app = builder.Build();

// -- Configure the request pipeline.

if( app.Environment.IsDevelopment() )
{
   app.UseSwagger();
   app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
