using AlphaCinema.Core.DTOs.NguoiDung;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Services.Services;

public class NguoiDungService : INguoiDungService
{
    private readonly AlphaCinemaDbContext _context;

    public NguoiDungService(AlphaCinemaDbContext context) => _context = context;

    private string GetHang(int diem)
    {
        if (diem >= 1500) return "Kim Cương";
        if (diem >= 500) return "Vàng";
        return "Bạc";
    }

    public async Task<IEnumerable<NguoiDungResponse>> GetAllAsync()
        => await _context.NguoiDungs.Include(u => u.VaiTro)
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(u => new NguoiDungResponse
            {
                MaNguoiDung = u.MaNguoiDung, Email = u.Email,
                HoTen = u.HoTen, VaiTro = u.VaiTro.TenVaiTro,
                DiemTichLuy = u.DiemTichLuy, HangThanhVien = GetHang(u.DiemTichLuy)
            }));

    public async Task<NguoiDungResponse?> GetByIdAsync(int id)
    {
        var u = await _context.NguoiDungs.Include(x => x.VaiTro).FirstOrDefaultAsync(x => x.MaNguoiDung == id);
        if (u == null) return null;
        return new NguoiDungResponse
        {
            MaNguoiDung = u.MaNguoiDung, Email = u.Email,
            HoTen = u.HoTen, VaiTro = u.VaiTro.TenVaiTro, 
            DiemTichLuy = u.DiemTichLuy, HangThanhVien = GetHang(u.DiemTichLuy)
        };
    }

    public async Task<NguoiDungResponse> UpdateAsync(int id, UpdateNguoiDungRequest request)
    {
        var u = await _context.NguoiDungs.Include(x => x.VaiTro).FirstOrDefaultAsync(x => x.MaNguoiDung == id)
            ?? throw new Exception("Người dùng không tồn tại.");

        if (request.HoTen != null) u.HoTen = request.HoTen;

        if (!string.IsNullOrEmpty(request.VaiTro))
        {
            var role = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == request.VaiTro);
            if (role != null)
            {
                u.MaVaiTro = role.MaVaiTro;
                u.VaiTro = role;
            }
        }

        if (!string.IsNullOrEmpty(request.MatKhauMoi))
        {
            if (!string.IsNullOrEmpty(request.MatKhauCu))
            {
                if (!BCrypt.Net.BCrypt.Verify(request.MatKhauCu, u.MatKhau))
                    throw new Exception("Mật khẩu cũ không chính xác.");
            }
            u.MatKhau = BCrypt.Net.BCrypt.HashPassword(request.MatKhauMoi);
        }

        await _context.SaveChangesAsync();
        return new NguoiDungResponse
        {
            MaNguoiDung = u.MaNguoiDung, Email = u.Email,
            HoTen = u.HoTen, VaiTro = u.VaiTro.TenVaiTro, 
            DiemTichLuy = u.DiemTichLuy, HangThanhVien = GetHang(u.DiemTichLuy)
        };
    }

    public async Task DeleteAsync(int id)
    {
        var u = await _context.NguoiDungs.FindAsync(id) ?? throw new Exception("Người dùng không tồn tại.");
        _context.NguoiDungs.Remove(u);
        await _context.SaveChangesAsync();
    }
}
