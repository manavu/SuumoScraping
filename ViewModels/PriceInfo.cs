namespace SuumoScraping.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PriceInfo
    {
        public int Index { get; set; }

        public DateTime ChangedAt { get; set; }

        public string Value { get; set; }
    }
}