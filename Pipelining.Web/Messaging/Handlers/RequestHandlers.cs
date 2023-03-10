// ReSharper disable ClassNeverInstantiated.Global

namespace Mediator.Web.Messaging.Handlers;

using Mediation;

public class RequestHandlers
{
   public class WeatherRequestHandler: IRequestHandler<GetWeather, IEnumerable<WeatherForecast>>
   {
      public Task<IEnumerable<WeatherForecast>> Handle( GetWeather request ) =>
         Task.FromResult( Enumerable.Range( 1, 5 )
                                    .Select( x => new WeatherForecast { Date = DateTime.UtcNow.AddDays( x ).DateOnly(), Summary = "Weather", TemperatureC = x * 2 } ) );
   }

   public class SetSomethingHandler: IRequestHandler<SetSomething>
   {
      private readonly ILogger<SetSomethingHandler> logger;

      public SetSomethingHandler( ILogger<SetSomethingHandler> logger )
      {
         this.logger = logger;
      }

      public Task Handle( SetSomething request )
      {
         logger.LogInformation( "Set something handler called!" );
         return Task.CompletedTask;
      }
   }
}
