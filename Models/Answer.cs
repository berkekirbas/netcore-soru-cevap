using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SoruCevapPortal.Models;

public class Answer : BaseEntity
{
    [Required(ErrorMessage = "Cevap içeriği gereklidir.")]
    [DisplayName("İçerik")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soru seçilmelidir.")]
    [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir soru seçin.")]
    [DisplayName("Soru")]
    public int QuestionId { get; set; }

    [Required(ErrorMessage = "Kullanıcı seçilmelidir.")]
    [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir kullanıcı seçin.")]
    [DisplayName("Kullanıcı")]
    public int UserId { get; set; }

    [DisplayName("Onaylı")]
    public bool IsApproved { get; set; } = false;

    [DisplayName("Aktif")]
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Question Question { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}

