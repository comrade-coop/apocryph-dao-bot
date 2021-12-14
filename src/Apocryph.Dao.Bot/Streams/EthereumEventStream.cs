using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Perper.Model;

namespace Apocryph.Dao.Bot.Streams
{
    
    public class EthereumEventStream<TEvent> where TEvent : IEventDTO, new()
    {
        private readonly IWeb3 _web3;
        private readonly IState _state;
        private readonly IOptions<Configuration.Dao> _options;
        
        protected EthereumEventStream(IState state, IWeb3 web3, IOptions<Configuration.Dao> options)
        {
            _web3 = web3;
            _state = state;
            _options = options;
        }
        
        public async IAsyncEnumerable<TEvent> RunAsync()
        {
            var lastBlockNUmber = BlockParameter.CreateEarliest();
            var blockData = await _state.GetLatestBlockData<TEvent>();
            if (blockData != null)
            {
                lastBlockNUmber = new BlockParameter(blockData.BlockNumber);
            }
                
            var transferEventHandler = _web3.Eth.GetEvent<TEvent>(_options.Value.EventContractAddress);
            var transferFilterInput = transferEventHandler.CreateFilterInput(lastBlockNUmber, BlockParameter.CreateLatest());
            var allEvents = await transferEventHandler.GetAllChangesAsync(transferFilterInput);
            
            foreach (var @event in allEvents)
            {
                await _state.AppendDataToLatestBlock((ulong)@event.Log.BlockNumber.Value, @event.Event);
                yield return @event.Event;
            }
            
            var transferFilterInputId = await transferEventHandler.CreateFilterAsync(transferFilterInput);
            while (true)
            {
                var changes = await transferEventHandler.GetFilterChangesAsync(transferFilterInputId);
                foreach (var change in changes)
                {
                    await _state.AppendDataToLatestBlock((ulong)change.Log.BlockNumber.Value, change.Event);
                    yield return change.Event;
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            
            // ReSharper disable once IteratorNeverReturns
        }
      
   }
}