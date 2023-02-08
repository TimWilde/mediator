namespace Pipelining.Web.Controllers;

using Mediation;
using Messaging;
using Microsoft.AspNetCore.Mvc;

[ ApiController ]
[ Route( "[controller]" ) ]
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
}
