using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SoruCevapPortal.Models;

public class Question : BaseEntity
{
    [Required(ErrorMessage = "Başlık gereklidir.")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    [DisplayName("Başlık")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "İçerik gereklidir.")]
    [DisplayName("İçerik")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kullanıcı seçilmelidir.")]
    [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir kullanıcı seçin.")]
    [DisplayName("Kullanıcı")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Kategori seçilmelidir.")]
    [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir kategori seçin.")]
    [DisplayName("Kategori")]
    public int CategoryId { get; set; }

    [DisplayName("Aktif")]
    public bool IsActive { get; set; } = true;

    [DisplayName("Görüntülenme")]
    public int ViewCount { get; set; } = 0;
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public virtual ICollection<QuestionVote> QuestionVotes { get; set; } = new List<QuestionVote>();
}

