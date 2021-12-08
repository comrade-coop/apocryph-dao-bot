namespace Apocryph.Dao.Bot.Configuration
{
    public class Airdrop
    {
        public   Tent Tent { get; set; }
    }
    
    public class Tent
    {
        public decimal Amount { get; set; }
        public string SourceAddress { get; set; }
    }
}