using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Perper.Model;
using Serilog;

namespace Apocryph.Dao.Bot.Streams
{
    public abstract class InboundStream<TInboundMessage, TOutboundMessage> 
        where TInboundMessage : IInboundMessage
        where TOutboundMessage : IOutboundMessage
    {
        protected readonly IState State;

        protected InboundStream(IState state)
        {
            State = state;
        }
        
        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<IInboundMessage> messages)
        {
            await foreach (var message in messages.Where(m => m is TInboundMessage).Cast<TInboundMessage>())
            {
                TOutboundMessage outputMessage = default;
                
                try
                {
                    outputMessage = await RunImplAsync(message);

                    if (!outputMessage.Errors.Any())
                    {
                        Log.Information("Processed {@Message}", message);
                    }
                    else
                    {
                        Log.Information("Processed {@Message} with errors {@Errors}", message, outputMessage.Errors);
                    }
                    
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to process {@Message}", message);
                }

                if (outputMessage != null)
                {
                    yield return outputMessage;    
                }
            }
        }

        protected abstract Task<TOutboundMessage> RunImplAsync(TInboundMessage message);
    }
}