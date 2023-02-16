// ReSharper disable ClassNeverInstantiated.Global

namespace Mediator.Web.Messaging.Handlers;

using System.Runtime.CompilerServices;
using Mediation;

public class TemperatureStreamHandler: IStreamHandler<TemperatureStreamRequest, int>
{
   public async IAsyncEnumerable<int> Handle( TemperatureStreamRequest request,
      [ EnumeratorCancellation ] CancellationToken cancellationToken )
   {
      while ( cancellationToken.IsCancellationRequested is false )
      {
         await Task.Delay( TimeSpan.FromSeconds( 1 ), cancellationToken );
         yield return Random.Shared.Next( -5, 40 );
      }
   }
}

public class OtherTempStreamHandler: IStreamHandler<TemperatureStreamRequest, int>
{
   public IAsyncEnumerable<int> Handle( TemperatureStreamRequest request, CancellationToken cancellationToken )
   {
      throw new NotImplementedException( "OtherTempStreamHandler.Handle" );
   }
}