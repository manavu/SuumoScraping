namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("prices")]
    public partial class Price
    {
        [Key]
        public int Id { get; set; }

        public DateTime ChangedAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Min { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Max { get; set; }

        public string Text { get; set; }

        [ForeignKey("NewBukken_Id")]
        public virtual NewBukken NewBukken { get; set; }
    }
}