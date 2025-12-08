using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SoruCevapPortal.Models;

public class Role : BaseEntity
{
    [Required(ErrorMessage = "Rol adı zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Rol adı 2-50 karakter arasında olmalıdır.")]
    [DisplayName("Rol Adı")]
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

