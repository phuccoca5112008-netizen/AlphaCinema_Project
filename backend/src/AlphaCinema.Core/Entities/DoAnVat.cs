using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaCinema.Core.Entities;

public class DoAnVat
{
    [Key]
    public int MaDoAnVat { get; set; }

    [Required]
    [StringLength(100)]
    public string TenMon { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Gia { get; set; }

    [StringLength(500)]
    public string? HinhAnh { get; set; }

    public string? Loai { get; set; } // "Bắp", "Nước", "Combo"

    // Relationships
    public virtual ICollection<HoaDonDoAnVat> HoaDonDoAnVats { get; set; } = new List<HoaDonDoAnVat>();
}
