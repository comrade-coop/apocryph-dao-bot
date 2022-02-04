using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.CoreApi;
using Ipfs.Http;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Ipfs
{
    public class IpfsTryoutsTests
    {
        [Test]
        public async Task UploadAndDownload()
        {
            var text = " some text goes here";
            
            var ipfsClient = new IpfsClient();
            var cid = await ipfsClient.FileSystem.AddTextAsync(text, new AddFileOptions
            {
                Pin = true
            });
            
            var client = new IpfsClient();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10 * 1000);

            await using var stream = client.PostDownloadAsync("cat", cancellationTokenSource.Token, cid.Id).GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();

            Assert.AreSame(text, data);
        }
    }
}