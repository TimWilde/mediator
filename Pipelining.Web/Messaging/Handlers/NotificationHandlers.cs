// ReSharper disable ClassNeverInstantiated.Global

namespace Mediator.Web.Messaging.Handlers;

using Mediation;

public class NotificationHandlers
{
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

   public class SecondNotificationHandler: INotificationHandler<DemoNotification>
   {
      private readonly ILogger<SecondNotificationHandler> logger;

      public SecondNotificationHandler( ILogger<SecondNotificationHandler> logger )
      {
         this.logger = logger;
      }

      public async Task Handle( DemoNotification request )
      {
         logger.LogInformation( "Second notification handler called." );
         await Task.Delay( TimeSpan.FromSeconds( 3 ) );
      }
   }
}
