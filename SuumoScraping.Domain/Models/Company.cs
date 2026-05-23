namespace SuumoScraping.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Company
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        [MaxLength(80)]
        public string TakkenLicense { get; set; }

        [MaxLength(80)]
        public string TransactionAspect { get; set; }
    }
}
