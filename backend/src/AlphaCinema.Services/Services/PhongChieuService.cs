using AlphaCinema.Core.DTOs.PhongChieu;
using AlphaCinema.Core.Entities;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Services.Services;

public class PhongChieuService : IPhongChieuService
{
    private readonly AlphaCinemaDbContext _context;

    public PhongChieuService(AlphaCinemaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PhongChieuResponse>> GetAllAsync()
    {
        var phongs = await _context.PhongChieus
            .Include(x => x.Ghes)
            .ToListAsync();
            
        return phongs.Select(p => new PhongChieuResponse
        {
            MaPhong = p.MaPhong,
            TenPhong = p.TenPhong,
            LoaiPhong = p.LoaiPhong,
            Ghes = p.Ghes.Select(g => new GheResponse
            {
                MaGhe = g.MaGhe,
                Hang = g.Hang,
                SoGhe = g.SoGhe,
                LoaiGhe = g.LoaiGhe
            }).ToList()
        });
    }

    public async Task<PhongChieuResponse?> GetByIdAsync(int id)
    {
        var p = await _context.PhongChieus
            .Include(x => x.Ghes)
            .FirstOrDefaultAsync(x => x.MaPhong == id);

        if (p == null) return null;

        return new PhongChieuResponse
        {
            MaPhong = p.MaPhong,
            TenPhong = p.TenPhong,
            LoaiPhong = p.LoaiPhong,
            Ghes = p.Ghes.Select(g => new GheResponse
            {
                MaGhe = g.MaGhe,
                Hang = g.Hang,
                SoGhe = g.SoGhe,
                LoaiGhe = g.LoaiGhe
            }).ToList()
        };
    }

    public async Task<PhongChieuResponse> CreateAsync(CreatePhongChieuRequest request)
    {
        var p = new PhongChieu { TenPhong = request.TenPhong, LoaiPhong = request.LoaiPhong };
        _context.PhongChieus.Add(p);
        await _context.SaveChangesAsync();

        return new PhongChieuResponse { MaPhong = p.MaPhong, TenPhong = p.TenPhong, LoaiPhong = p.LoaiPhong };
    }

    public async Task DeleteAsync(int id)
    {
        var p = await _context.PhongChieus
            .Include(x => x.Ghes)
            .FirstOrDefaultAsync(x => x.MaPhong == id);

        if (p != null)
        {
            // Kiểm tra xem phòng có suất chiếu nào không
            var hasShowtimes = await _context.SuatChieus.AnyAsync(s => s.MaPhong == id);
            if (hasShowtimes) 
            {
                throw new Exception("Không thể xóa phòng chiếu này vì đang có suất chiếu được lập lịch. Hãy xóa các suất chiếu liên quan trước.");
            }

            // Xóa tất cả ghế của phòng trước
            if (p.Ghes.Any())
            {
                _context.Ghes.RemoveRange(p.Ghes);
            }

            _context.PhongChieus.Remove(p);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Không tìm thấy phòng chiếu để xóa.");
        }
    }

    public async Task UpdateAsync(int id, CreatePhongChieuRequest request)
    {
        var p = await _context.PhongChieus.FindAsync(id);
        if (p == null) throw new Exception("Không tìm thấy phòng chiếu.");

        p.TenPhong = request.TenPhong;
        p.LoaiPhong = request.LoaiPhong;
        await _context.SaveChangesAsync();
    }

    public async Task GenerateGheAsync(int maPhong, GenerateGheRequest request)
    {
        var p = await _context.PhongChieus.Include(x => x.Ghes).FirstOrDefaultAsync(x => x.MaPhong == maPhong);
        if (p == null) throw new Exception("Không tìm thấy phòng chiếu.");

        // Xóa tất cả ghế cũ
        _context.Ghes.RemoveRange(p.Ghes);

        // Tạo sơ đồ ghế mới
        var newGhes = new List<Ghe>();
        for (int i = 0; i < request.SoHang; i++)
        {
            char hang = (char)('A' + i);
            for (int j = 1; j <= request.SoGheMotHang; j++)
            {
                newGhes.Add(new Ghe
                {
                    MaPhong = maPhong,
                    Hang = hang,
                    SoGhe = j,
                    LoaiGhe = "Thuong" // Default type
                });
            }
        }

        _context.Ghes.AddRange(newGhes);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateGheAsync(int maGhe, UpdateGheRequest request)
    {
        var ghe = await _context.Ghes.FindAsync(maGhe);
        if (ghe == null) throw new Exception("Không tìm thấy ghế.");

        ghe.LoaiGhe = request.LoaiGhe;
        await _context.SaveChangesAsync();
    }
}
