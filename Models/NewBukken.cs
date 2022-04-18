namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("newbukkens")]
    [Index(nameof(DetailUrl), Name = "IX_Bukkens_DetailUrl", IsUnique = true)]
    public partial class NewBukken
    {
        public NewBukken()
        {
            this.Files = new HashSet<NewBukkenFile>();
            this.PriceChangesets = new HashSet<Price>();
            this.Company = new Company();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(200)]
        public string DetailUrl { get; set; }

        [Required]
        [MaxLength(100)]
        public string Address { get; set; }

        [Display(Name = "交通手段1")]
        [Required]
        [MaxLength(100)]
        public string Access1 { get; set; }

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
        [Column(TypeName = "datetime")] // 型を指定しないと datetime(6) になる
        public DateTime? BuiltYears { get; set; }

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

        public virtual ICollection<NewBukkenFile> Files { get; set; }

        [Display(Name = "インポート回数")]
        public int ImportCount { get; set; }

        [Display(Name = "インポート日")]
        [Column(TypeName = "datetime")] // 型を指定しないと datetime(6) になる
        public DateTime ImportedAt { get; set; }

        [Column(TypeName = "datetime")] // 型を指定しないと datetime(6) になる
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Price> PriceChangesets { get; set; }
    }
}