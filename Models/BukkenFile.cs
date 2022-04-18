namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    [Table("bukkenfiles")]
    public partial class BukkenFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [ForeignKey("Bukken_Id")]
        public virtual Bukken Bukken { get; set; }

        [ForeignKey("File_Id")]
        public virtual File File { get; set; }
    }
}
