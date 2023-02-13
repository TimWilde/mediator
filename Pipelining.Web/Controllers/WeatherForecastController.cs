namespace Mediator.Web.Controllers;

using Mediation;
using Messaging;
using Microsoft.AspNetCore.Mvc;

[ ApiController, Route( "[controller]" ) ]
public class WeatherForecastController: ControllerBase
{
   private readonly IMediator mediator;

   public WeatherForecastController( IMediator mediator )
   {
      this.mediator = mediator;
   }

   [ HttpGet ]
   public async Task<IEnumerable<WeatherForecast>> Get()
   {
      await mediator.DispatchAsync( new SetSomething() );
      return await mediator.DispatchAsync( new GetWeather() );
   }

   [ HttpGet( "/temps" ) ]
   public IAsyncEnumerable<int> Temperatures( CancellationToken cancellationToken ) =>
      mediator.CreateStream( new TemperatureStreamRequest(), cancellationToken );

   [ HttpGet( "/notify" ) ]
   public async Task<NoContentResult> Notify()
   {
      await mediator.PublishAsync( new DemoNotification() );
      return NoContent();
   }
}