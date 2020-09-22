namespace SuumoScraping.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class FilterForm
    {
        [Display(Name = "住所")]
        public string Address { get; set; }

        [Display(Name = "交通")]
        public string Access { get; set; }

        [Display(Name = "タイトル")]
        public string Title { get; set; }

        [Display(Name = "最少額")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "最大額")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "最少面積")]
        public decimal? MinArea { get; set; }

        [Display(Name = "最大面積")]
        public decimal? MaxArea { get; set; }

        [Display(Name = "インポート日から")]
        [DataType(DataType.Date)]
        public DateTime? ImportedDateFrom { get; set; }

        [Display(Name = "インポート日まで")]
        [DataType(DataType.Date)]
        public DateTime? ImportedDateTo { get; set; }
    }
}