using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SoruCevapPortal.Models;

public class ApplicationRole : IdentityRole<int>
{
    [DisplayName("Açıklama")]
    [StringLength(200)]
    public string? Description { get; set; }

    [DisplayName("Oluşturulma Tarihi")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

