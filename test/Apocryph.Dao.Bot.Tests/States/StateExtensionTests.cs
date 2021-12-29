using System.Threading.Tasks;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.States
{
    public class StateExtensionTests
    {
        private record EthEvent(int Value);
        
        [Test]
        public async Task AppendDataToLatestBlock_And_GetLatestBlockData()
        {
            InMemoryState state = new();
            var contractAddress = "0x08f2951251bC4Cb33b47d9f6b63E6E820AA17AB8";
            
            var blockData = await state.GetLatestBlockData<EthEvent>(contractAddress);
            blockData.Should().BeNull();
            
            await state.AppendDataToLatestBlock(contractAddress, 100, new EthEvent(10));
            await state.AppendDataToLatestBlock(contractAddress, 100, new EthEvent(11));
            await state.AppendDataToLatestBlock(contractAddress, 100, new EthEvent(12));

            blockData = await state.GetLatestBlockData<EthEvent>(contractAddress);
            blockData.Items.Should().HaveCount(3);
            blockData.BlockNumber.Should().Be(100);
            
            await state.AppendDataToLatestBlock(contractAddress, 101, new EthEvent(13));
            blockData = await state.GetLatestBlockData<EthEvent>(contractAddress);
            blockData.Items.Should().HaveCount(1);
            blockData.BlockNumber.Should().Be(101);
        }
    }
}