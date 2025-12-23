using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SoruCevapPortal.Models;

public class ApplicationUser : IdentityUser<int>
{
    [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
    [DisplayName("Ad Soyad")]
    public string? FullName { get; set; }

    [DisplayName("Aktif")]
    public bool IsActive { get; set; } = true;

    [DisplayName("Oluşturulma Tarihi")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DisplayName("Profil Resmi URL")]
    public string? ProfileImageUrl { get; set; }

    [DisplayName("Hakkımda")]
    [StringLength(500)]
    public string? Bio { get; set; }

    // Navigation properties
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public virtual ICollection<QuestionVote> QuestionVotes { get; set; } = new List<QuestionVote>();
}

