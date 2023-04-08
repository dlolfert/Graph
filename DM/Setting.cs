using System.Collections.Generic;

namespace DM
{
    public class Setting
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public IDictionary<string,string> AllSettings { get; set; }
    }
}
