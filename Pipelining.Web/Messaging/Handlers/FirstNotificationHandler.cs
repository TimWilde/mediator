// ReSharper disable ClassNeverInstantiated.Global

namespace Mediator.Web.Messaging.Handlers;

using Mediation;

public class FirstNotificationHandler: INotificationHandler<DemoNotification>
{
   private readonly ILogger<FirstNotificationHandler> logger;

   public FirstNotificationHandler( ILogger<FirstNotificationHandler> logger )
   {
      this.logger = logger;
   }

   public async Task Handle( DemoNotification request )
   {
      logger.LogInformation( "First notification handler called." );
      await Task.Delay( TimeSpan.FromSeconds( 2 ) );
   }
}