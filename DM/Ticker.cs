using System.ComponentModel;

namespace DM
{
    public class Ticker
    {
        [DisplayName("Ticker")]
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }

        public string Rank { get; set; }
    }
}