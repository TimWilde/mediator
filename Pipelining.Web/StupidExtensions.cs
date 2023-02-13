namespace Mediator.Web;

public static class StupidExtensions
{
   public static DateOnly DateOnly( this DateTime input ) =>
      System.DateOnly.FromDateTime( input );
}