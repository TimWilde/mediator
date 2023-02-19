namespace Mediator.Web.Messaging;

public record DaysSinceResponse( int Days )
{
   public string DaysSince => $"{Days} days";
}
