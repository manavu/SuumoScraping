namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("bukkens")]
    [Index(nameof(DetailUrl), nameof(ImportedDate), Name = "IX_Bukkens_DetailUrl_ImportedDate")]
    [Index(nameof(ImportedDate), nameof(DetailUrl), Name = "IX_Bukkens_ImportedDate_DetailUrl")]
    // [Index(nameof(FullText.Id), Name="IX_FullText_Id")]  // こういうことはできないので、OnModelCreation で対応するしかないかも
    public partial class Bukken
    {
        public Bukken()
        {
            this.Files = new HashSet<BukkenFile>();
            this.Company = new Company();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Display(Name = "価格（文字列）")]
        [Required]
        [MaxLength(50)]
        public string Price { get; set; }

        [Display(Name = "価格")]
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price1 { get; set; }

        [Display(Name = "価格（最大値）")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price2 { get; set; }

        [Required]
        [MaxLength(200)]
        public string DetailUrl { get; set; }

        [Required]
        [MaxLength(100)]
        public string Address { get; set; }

        [Display(Name = "交通手段1")]
        [Required]
        [MaxLength(100)]
        public string Access { get; set; }

        [Display(Name = "交通手段2")]
        [MaxLength(100)]
        public string Access2 { get; set; }

        [Display(Name = "交通手段3")]
        [MaxLength(100)]
        public string Access3 { get; set; }

        [Display(Name = "間取り")]
        [Required]
        [MaxLength(100)]
        public string Layout { get; set; }

        [Display(Name = "専有面積（文字列）")]
        [Required]
        [MaxLength(100)]
        public string FloorArea { get; set; }

        [Display(Name = "専有面積")]
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FloorArea1 { get; set; }

        [Display(Name = "専有面積（坪）")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? FloorTubo { get; set; }

        [Display(Name = "計測方法")]
        public string FloorAreaMeasuringMethod { get; set; }

        [Display(Name = "バルコニー面積")]
        [Required]
        [MaxLength(50)]
        public string Balcony { get; set; }

        [Display(Name = "築年月")]
        [Required]
        [MaxLength(20)]
        public string BuiltYears { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public System.DateTime ImportedDate { get; set; }

        [MaxLength(50)]
        public string ManagementFee { get; set; }

        [MaxLength(50)]
        public string RepairingDeposit { get; set; }

        [MaxLength(50)]
        public string RepairingFund { get; set; }

        [MaxLength(20)]
        public string Floor { get; set; }

        [MaxLength(5)]
        public string Direction { get; set; }

        [MaxLength(50)]
        public string UseDistrict { get; set; }

        [MaxLength(50)]
        public string Structure { get; set; }

        [Display(Name = "敷地の権利形態")]
        [MaxLength(300)]
        public string RightsStyle { get; set; }

        [Display(Name = "入居時期")]
        [MaxLength(50)]
        public string MoveInTime { get; set; }

        [Display(Name = "その他制限事項")]
        [MaxLength(500)]
        public string Restriction { get; set; }

        public Company Company { get; set; }

        [ForeignKey("FullText_Id")]
        public virtual BukkenFulltext FullText { get; set; }

        public virtual ICollection<BukkenFile> Files { get; set; }
    }
}
