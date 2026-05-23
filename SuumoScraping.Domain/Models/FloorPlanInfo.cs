using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuumoScraping.Domain.Models
{
    public class FloorPlanInfo
    {
        public int FileId { get; set; }

        public int BukkenId { get; set; }

        public string FloorArea { get; set; }
    }
}
