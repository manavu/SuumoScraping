namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("newbukkenfiles")]
    public partial class NewBukkenFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [ForeignKey("File_Id")]
        public virtual File File { get; set; }

        [ForeignKey("NewBukken_Id")]
        public virtual NewBukken NewBukken { get; set; }
    }
}