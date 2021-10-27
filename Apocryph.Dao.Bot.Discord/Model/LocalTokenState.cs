using System.IO;
using Newtonsoft.Json;

namespace Apocryph.Dao.Bot.Discord.Model
{
    public class LocalTokenState
    {
        const string TokenStateFile = "token.state";
		
        public void Initialize(out LocalToken localTokenState)
        {
            var filePath = Path.GetFullPath(TokenStateFile);
			
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                localTokenState = JsonConvert.DeserializeObject<LocalToken>(content);
            }
            else
            {
                localTokenState = new LocalToken();
            }
        }
		
        public void Store(LocalToken localTokenState)
        {
            var filePath = Path.GetFullPath(TokenStateFile);

            if (File.Exists(filePath))
            {
                var content = JsonConvert.SerializeObject(localTokenState);
                File.WriteAllText(filePath, content);
            }
        }
    }
}