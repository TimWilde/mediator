// ReSharper disable ClassNeverInstantiated.Global

namespace Pipelining.Web.Messaging;

using Mediation;

public class WeatherRequestHandler: IRequestHandler<GetWeather, IEnumerable<WeatherForecast>>
{
   public Task<IEnumerable<WeatherForecast>> Handle( GetWeather request ) =>
      Task.FromResult( Enumerable.Range( 1, 5 )
                                 .Select( x => new WeatherForecast { Date = DateTime.UtcNow.AddDays( x ).DateOnly(), Summary = "Weather", TemperatureC = x * 2 } ) );
}
