namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class BukkenFulltext
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "２文字区切りの交通情報")]
        public string AccessBigram { get; set; }

        [Required]
        [Display(Name = "２文字区切りの住所情報")]
        public string AddressBigram { get; set; }
    }
}