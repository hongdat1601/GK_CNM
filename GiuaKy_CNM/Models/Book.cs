using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiuaKy_CNM.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Tên sách")]
        public string? Name { get; set; }

        [Display(Name = "Ảnh")]
        public string? ImagePath { get; set; }

        [Display(Name = "File")]
        public string? PdfPath { get; set; }

        [Required]
        [StringLength(120)]
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Tên tác giả")]
        public string? Author { get; set; }

        [Required]
        [StringLength(250)]
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Thể loại")]
        public string? Categories { get; set; }

        [Display(Name = "Ngày phát hành")]
        public DateTime? PublishDate { get; set; }

        [NotMapped]
        [Display(Name = "Ảnh")]
        public IFormFile? Image { get; set; }

        [NotMapped]
        [Display(Name = "File")]
        public IFormFile? Pdf { get; set; }
    }
}
