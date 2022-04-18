namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("files")]
    [Index(nameof(Url), Name = "IX_Files_Url")]
    public partial class File
    {
        public File()
        {
            this.BukkenFiles = new HashSet<BukkenFile>();
        }

        public File(byte[] data, string contentType, string url) : this()
        {
            var hashString = string.Empty;

            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var hash = sha.ComputeHash(data);

                hashString = BitConverter.ToString(hash).Replace("-", "");
            }

            this.ContentType = contentType;
            this.FileData = data;
            this.Hash = hashString;
            this.Url = url;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public byte[] FileData { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContentType { get; set; }

        [Required]
        [MaxLength(64)]
        [Column(TypeName = "nchar")]
        public string Hash { get; private set; }

        [Required]
        [MaxLength(500)]
        public string Url { get; set; }

        public virtual ICollection<BukkenFile> BukkenFiles { get; set; }
    }
}
