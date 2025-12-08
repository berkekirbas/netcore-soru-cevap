using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SoruCevapPortal.Models;

public class User : BaseEntity
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır.")]
    [DisplayName("Kullanıcı Adı")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(100, ErrorMessage = "E-posta adresi en fazla 100 karakter olabilir.")]
    [DisplayName("E-posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DisplayName("Şifre")]
    public string Password { get; set; } = string.Empty; // In production, this should be hashed

    [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
    [DisplayName("Ad Soyad")]
    public string? FullName { get; set; }

    [DisplayName("Aktif")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public virtual ICollection<QuestionVote> QuestionVotes { get; set; } = new List<QuestionVote>();
}

