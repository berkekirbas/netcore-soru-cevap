using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SoruCevapPortal.ViewModels;

public class ProfileViewModel
{
    [DisplayName("Kullanıcı Adı")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [DisplayName("E-posta")]
    public string Email { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
    [DisplayName("Ad Soyad")]
    public string? FullName { get; set; }

    [StringLength(500, ErrorMessage = "Hakkımda en fazla 500 karakter olabilir.")]
    [DisplayName("Hakkımda")]
    public string? Bio { get; set; }

    [DisplayName("Profil Resmi URL")]
    public string? ProfileImageUrl { get; set; }

    public int QuestionCount { get; set; }
    public int AnswerCount { get; set; }
    public DateTime MemberSince { get; set; }
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Mevcut şifre zorunludur.")]
    [DataType(DataType.Password)]
    [DisplayName("Mevcut Şifre")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [DisplayName("Yeni Şifre")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
    [DisplayName("Yeni Şifre Tekrar")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

