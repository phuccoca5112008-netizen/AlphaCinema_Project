using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaCinema.Core.Entities;

public class HoaDonDoAnVat
{
    [Key]
    public int MaHoaDonDoAnVat { get; set; }

    [Required]
    public int MaHoaDon { get; set; }

    [ForeignKey("MaHoaDon")]
    public virtual HoaDon HoaDon { get; set; } = null!;

    [Required]
    public int MaDoAnVat { get; set; }

    [ForeignKey("MaDoAnVat")]
    public virtual DoAnVat DoAnVat { get; set; } = null!;

    [Required]
    public int SoLuong { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal DonGia { get; set; } // Giá tại thời điểm đặt

    public decimal ThanhTien => SoLuong * DonGia;
}
