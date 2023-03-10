// ReSharper disable UnusedMethodReturnValue.Global

namespace Mediator.Web.Mediation.Implementation;

using Messaging.Handlers;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class MediationRegistrationExtensions
{
   public static IServiceCollection AddMediation( this IServiceCollection services, Action<IMediatorConfiguration> configure )
   {
      services.TryAddSingleton<IMediator, Mediator>();

      configure( new MediatorConfiguration( services ) );

      return services;
   }

   public interface IMediatorConfiguration
   {
      IMediatorConfiguration AddHandler<THandler>( ServiceLifetime lifetime = ServiceLifetime.Transient )
         where THandler: IRequestHandler;

      IMediatorConfiguration AddStreamHandler<THandler>( ServiceLifetime lifetime = ServiceLifetime.Transient )
         where THandler: IStreamHandler;

      IMediatorConfiguration AddNotificationHandler<THandler>( ServiceLifetime lifetime = ServiceLifetime.Transient )
         where THandler: INotificationHandler;

      IMediatorConfiguration AddPipeline( Action<PipelineBuilder> build );
   }

   private class MediatorConfiguration: IMediatorConfiguration
   {
      private readonly IServiceCollection services;

      public MediatorConfiguration( IServiceCollection services )
      {
         this.services = services;
      }

      public IMediatorConfiguration AddHandler<THandler>( ServiceLifetime lifetime = ServiceLifetime.Transient )
         where THandler: IRequestHandler
      {
         Type? implementedInterface = typeof( THandler ).GetInterface( Mediator.FunctionHandlerType.FullName! ) ??
                                      typeof( THandler ).GetInterface( Mediator.ActionHandlerType.FullName! );

         if( implementedInterface is null )
            throw MediatorHandlerConfigurationException.InvalidRequestHandler<THandler>();

         services.Add( new ServiceDescriptor( implementedInterface, typeof( THandler ), lifetime ) );

         return this;
      }

      public IMediatorConfiguration AddStreamHandler<THandler>( ServiceLifetime lifetime = ServiceLifetime.Transient )
         where THandler: IStreamHandler
      {
         Type? implementedInterface = typeof( THandler ).GetInterface( Mediator.StreamHandlerType.FullName! );

         if( implementedInterface is null )
            throw MediatorHandlerConfigurationException.InvalidStreamHandler<THandler>();

         services.Add( new ServiceDescriptor( implementedInterface, typeof( THandler ), lifetime ) );

         return this;
      }

      public IMediatorConfiguration AddNotificationHandler<THandler>( ServiceLifetime lifetime = ServiceLifetime.Transient )
         where THandler: INotificationHandler
      {
         Type? implementedInterface = typeof( THandler ).GetInterface( Mediator.NotificationHandlerType.FullName! );

         if( implementedInterface is null )
            throw MediatorHandlerConfigurationException.InvalidNotificationHandler<THandler>();

         services.Add( new ServiceDescriptor( implementedInterface, typeof( THandler ), lifetime ) );

         return this;
      }

      public IMediatorConfiguration AddPipeline( Action<PipelineBuilder> build )
      {
         var builder = new PipelineBuilder();

         build( builder );

         builder.Build( services );

         return this;
      }
   }
}
