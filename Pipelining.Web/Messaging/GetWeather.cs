namespace Pipelining.Web.Messaging;

using Mediation;

public record GetWeather: IRequest<IEnumerable<WeatherForecast>>;
