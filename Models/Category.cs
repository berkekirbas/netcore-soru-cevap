using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SoruCevapPortal.Models;

public class Category : BaseEntity
{
    [Required(ErrorMessage = "Kategori adı gereklidir.")]
    [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
    [DisplayName("Kategori Adı")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
    [DisplayName("Açıklama")]
    public string? Description { get; set; }

    [DisplayName("Aktif")]
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}

