using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DM
{
    
    public class DayHigh
    {
        //Symbol Name    Average DaysAboveAvg    Total AdjustedTotal   DaysCloseAboveOpen DaysHighAboveOpen	% Day High Above Open   StdDev Records
        //USO NULL	0.14	21	9.22	5.74	29	66	97	0.18	68
        [DisplayName("DayHigh")]
        public string symbol { get; set; }
        public string name { get; set; }
        public string average { get; set; }
        public string DaysAboveAvg { get; set; }
        public string PercentDaysAboveAvg { get; set; }
        public string DaysCloseAboveOpen { get; set; }
        public string PercentDaysCloseAboveOpen { get; set; }
        public string DaysHighAboveOpen { get; set; }
        public string PercentHighAboveOpen { get; set; }
        public string StdDev { get; set; }
        public string records { get; set; }

        public string lastClose { get; set; }
        public string avgVolume { get; set; }
        public string DHArray { get; set; }

        public string V100 { get; set; }
        public string V90 { get; set; }
        public string V80 { get; set; }
        public string V70 { get; set; }
        public string V60 { get; set; }
        public string V50 { get; set; }
        public string V40 { get; set; }
        public string V30 { get; set; }
        public string V20 { get; set; }
        public string V10 { get; set; }
    }
}
