using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SoruCevapPortal.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı veya e-posta zorunludur.")]
    [DisplayName("Kullanıcı Adı / E-posta")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [DisplayName("Şifre")]
    public string Password { get; set; } = string.Empty;

    [DisplayName("Beni Hatırla")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

