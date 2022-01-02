namespace SuumoScraping.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class BukkenInfo
    {
        public BukkenInfo()
        {
            Files = new List<FileInfo>();
        }

        [Display(Name = "ファイル")]
        public IEnumerable<FileInfo> Files { get; set; }

        public int Id { get; set; }

        [Display(Name = "タイトル")]
        public string Title { get; set; }

        [Display(Name = "価格")]
        public string Price { get; set; }

        [Display(Name = "URL")]
        public string DetailUrl { get; set; }

        [Display(Name = "所在地")]
        public string Address { get; set; }

        [Display(Name = "交通")]
        public string Access1 { get; set; }

        public string Access2 { get; set; }

        public string Access3 { get; set; }

        [Display(Name = "間取り")]
        public string Layout { get; set; }

        [Display(Name = "専有面積")]
        public string FloorArea { get; set; }

        [Display(Name = "バルコニー面積")]
        public string Balcony { get; set; }

        [Display(Name = "築年月")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月}")]
        public DateTime? BuiltYears { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "インポート年月日")]
        public DateTime ImportedDate { get; set; }

        [Display(Name = "管理費")]
        public string ManagementFee { get; set; }

        [Display(Name = "修繕積立金")]
        public string RepairingDeposit { get; set; }

        [Display(Name = "修繕積立基金")]
        public string RepairingFund { get; set; }

        [Display(Name = "所在階")]
        public string Floor { get; set; }

        [Display(Name = "向き")]
        public string Direction { get; set; }

        [Display(Name = "用途地域")]
        public string UseDistrict { get; set; }

        [Display(Name = "構造・階建て")]
        public string Structure { get; set; }

        [Display(Name = "敷地の権利形態")]
        public string RightsStyle { get; set; }

        [Display(Name = "入居時期")]
        public string MoveInTime { get; set; }

        [Display(Name = "企業名")]
        public string CompanyName { get; set; }

        [Display(Name = "企業住所")]
        public string CompanyAddress { get; set; }

        [Display(Name = "インポート回数")]
        public int ImportCount { get; set; }
    }
}