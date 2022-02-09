using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Perper.Model;
using Serilog;

namespace Apocryph.Dao.Bot.Streams
{
    public abstract class EthereumEventStream<TEvent> where TEvent : IEventDTO, new()
    {
        private readonly IWeb3 _web3;
        private readonly IState _state;
        
        protected EthereumEventStream(IState state, IWeb3 web3)
        {
            _web3 = web3;
            _state = state;
        }
 
        public async IAsyncEnumerable<EventLog<TEvent>> RunAsync(string contractAddress)
        {
            if(string.IsNullOrWhiteSpace(contractAddress))
                yield break;
            
            Event<TEvent> _event = null;
            NewFilterInput _filterInput = null;
            List<EventLog<TEvent>> _eventLogs = null;
                
            try
            {
                Log.Information("Listening for {@EventType} on {@ContractAddress}", nameof(TEvent), contractAddress);
                
                var lastBlockNUmber = BlockParameter.CreateEarliest();
                var blockData = await _state.GetLatestBlockData<TEvent>(contractAddress);
                if (blockData != null)
                {
                    lastBlockNUmber = new BlockParameter(blockData.BlockNumber);
                }
                
                _event = _web3.Eth.GetEvent<TEvent>(contractAddress.ToLower());
                _filterInput = _event.CreateFilterInput(lastBlockNUmber, BlockParameter.CreateLatest());
                _eventLogs = await _event.GetAllChangesAsync(_filterInput);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to process {@EventType} on {@ContractAddress}",  nameof(TEvent), contractAddress);
            }
            
            foreach (var @event in _eventLogs)
            {
                yield return @event;
            }
            
            var transferFilterInputId = await _event.CreateFilterAsync(_filterInput);
            
            while (true)
            {
                var changes = await _event.GetFilterChangesAsync(transferFilterInputId);
                foreach (var change in changes)
                {
                    await _state.AppendDataToLatestBlock(contractAddress, (ulong)change.Log.BlockNumber.Value, change.Event);
                    yield return change;
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
   }
}