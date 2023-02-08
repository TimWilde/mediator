// ReSharper disable UnusedMethodReturnValue.Global

namespace Pipelining.Web.Mediation;

using Microsoft.Extensions.DependencyInjection.Extensions;

public static class MediationExtensions
{
   public static IServiceCollection AddMediation( this IServiceCollection services, Action<IMediatorConfiguration> configure )
   {
      services.TryAddSingleton<IMediator, Mediator>();

      configure?.Invoke( new MediatorConfiguration( services ) );

      return services;
   }

   public interface IMediatorConfiguration
   {
      IMediatorConfiguration AddHandler<THandler>( ServiceLifetime lifetime = ServiceLifetime.Transient )
         where THandler: IRequestHandler;
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
            throw RequestHandlerConfigurationException.InvalidRequestHandler<THandler>();

         services.Add( new ServiceDescriptor( implementedInterface, typeof( THandler ), lifetime ) );

         return this;
      }
   }
}
