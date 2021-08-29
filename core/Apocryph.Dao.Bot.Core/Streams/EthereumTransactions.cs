using Apocryph.Dao.Bot.Core.Data;
using Nethereum.Parity;
using Perper.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class EthereumTransactions
    {
        private readonly IContext context;

        public EthereumTransactions(IContext context) => this.context = context;

        public async IAsyncEnumerable<DaoTransaction> RunAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var channel = Channel.CreateUnbounded<DaoTransaction>();
            var processorTask = ProcessAsync(channel, cancellationTokenSource);
            try
            {
                await foreach (var transaction in channel.Reader.ReadAllAsync(cancellationTokenSource.Token))
                {
                    yield return transaction;
                }
            }
            finally
            {
                await processorTask;
            }
        }

        private static async Task ProcessAsync(Channel<DaoTransaction> channel, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                await new Web3Parity(Environment.GetEnvironmentVariable("ETHEREUM_PARITY_NODE_ENDPOINT")).Processing.Blocks.CreateBlockProcessor(steps =>
                {
                    steps.BlockStep.AddProcessorHandler(async block =>
                    {
                        await channel.Writer.WriteAsync(new DaoTransaction { BlockHash = block.BlockHash }, cancellationTokenSource.Token);
                    });
                }).ExecuteAsync(cancellationTokenSource.Token);
            }
            finally
            {
                if (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                }
            }
        }
    }
}
