namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class File
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
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
        public string Hash { get; }

        [Required]
        [MaxLength(500)]
        [Index("IX_Files_Url", 1)]
        public string Url { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BukkenFile> BukkenFiles { get; set; }
    }
}
