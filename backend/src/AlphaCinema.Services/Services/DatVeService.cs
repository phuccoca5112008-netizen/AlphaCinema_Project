using AlphaCinema.Core.DTOs.Ve;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Core.Entities;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
// Removed QRCoder to avoid server-side runtime issues.

namespace AlphaCinema.Services.Services;

public class DatVeService : IDatVeService
{
    private readonly AlphaCinemaDbContext _context;
    private readonly IKhuyenMaiService _khuyenMaiService;

    public DatVeService(AlphaCinemaDbContext context, IKhuyenMaiService khuyenMaiService)
    {
        _context = context;
        _khuyenMaiService = khuyenMaiService;
    }

    public async Task<DatVeResponse> LockSeatsAsync(int maNguoiDung, LockSeatsRequest request)
    {
        // Sử dụng Transaction để đảm bảo tính nhất quán (Atomic operation)
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Validate suất chiếu
            var suatChieu = await _context.SuatChieus
                .Include(s => s.Phim).Include(s => s.PhongChieu)
                .FirstOrDefaultAsync(s => s.MaSuatChieu == request.MaSuatChieu)
                ?? throw new Exception("Suất chiếu không tồn tại.");

            // 2. Kiểm tra ghế hợp lệ trong phòng
            var ghes = await _context.Ghes
                .Where(g => request.MaGheIds.Contains(g.MaGhe) && g.MaPhong == suatChieu.MaPhong)
                .ToListAsync();

            if (ghes.Count != request.MaGheIds.Count)
                throw new Exception("Một hoặc nhiều ghế không hợp lệ cho phòng chiếu này.");

            // 3. KIỂM TRA TRÙNG LẶP VÀ DỌN DẸP VÉ HẾT HẠN (CLEANUP EXPIRED PENDING)
            var limitTime = DateTime.Now.AddMinutes(-10);
            
            // Tìm và xóa các hóa đơn "Đang chờ" của MỌI NGƯỜI đã quá hạn
            var expiredHoaDons = await _context.HoaDons
                .Include(h => h.Ves)
                .Where(h => h.TrangThai == "Đang chờ" && h.NgayGiaoDich <= limitTime)
                .ToListAsync();

            // ĐẶC BIỆT: Xóa sạch các hóa đơn nháp của CHÍNH NGƯỜI DÙNG NÀY cho suất chiếu này (nếu có) để tránh lỗi trùng lặp khi nhấn lại
            var myPendingForThisShow = await _context.HoaDons
                .Include(h => h.Ves)
                .Where(h => h.MaNguoiDung == maNguoiDung && h.TrangThai == "Đang chờ")
                .Where(h => h.Ves.Any(v => v.MaSuatChieu == request.MaSuatChieu))
                .ToListAsync();

            var combinedToRemove = expiredHoaDons.Concat(myPendingForThisShow).Distinct().ToList();

            if (combinedToRemove.Any())
            {
                foreach (var hd in combinedToRemove)
                {
                    if (hd.Ves != null) _context.Ves.RemoveRange(hd.Ves);
                    _context.HoaDons.Remove(hd);
                }
                await _context.SaveChangesAsync();
            }

            // Kiểm tra ghế trống thực tế
            var occupiedGheIds = await _context.Ves
                .Include(v => v.HoaDon)
                .Where(v => v.MaSuatChieu == request.MaSuatChieu
                         && v.TrangThai != "Đã hủy"
                         && (v.TrangThai != "Đang chờ" || v.HoaDon.NgayGiaoDich > limitTime))
                .Select(v => v.MaGhe)
                .ToListAsync();

            var doubleBooked = request.MaGheIds.Intersect(occupiedGheIds).Any();
            if (doubleBooked)
            {
                throw new Exception("Ghế này vừa có người nhanh tay chọn trước. Vui lòng chọn ghế khác.");
            }

            // 4. Tạo HoaDon trạng thái "Đang chờ"
            var tongTienGoc = ghes.Sum(g => g.LoaiGhe == "VIP" ? suatChieu.GiaVeGoc * 1.5m : suatChieu.GiaVeGoc);
            var hoaDon = new Core.Entities.HoaDon
            {
                MaNguoiDung = maNguoiDung,
                TongTien = tongTienGoc,
                PhuongThucThanhToan = "Chưa chọn",
                NgayGiaoDich = DateTime.Now,
                MaVaoCong = $"PENDING-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
                TrangThai = "Đang chờ"
            };
            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync(); // Lưu để lấy MaHoaDon

            // 5. Tạo vé trạng thái "Đang chờ"
            var vesCreated = new List<Core.Entities.Ve>();
            foreach (var ghe in ghes)
            {
                vesCreated.Add(new Core.Entities.Ve
                {
                    MaHoaDon = hoaDon.MaHoaDon,
                    MaGhe = ghe.MaGhe,
                    MaSuatChieu = request.MaSuatChieu,
                    MaVaoCong = $"WAIT-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}", // Cấp mã tạm cho vé
                    TrangThai = "Đang chờ",
                    GiaVe = ghe.LoaiGhe == "VIP" ? suatChieu.GiaVeGoc * 1.5m : suatChieu.GiaVeGoc
                });
            }
            _context.Ves.AddRange(vesCreated);
            await _context.SaveChangesAsync();

            // Xác nhận giao dịch thành công
            await transaction.CommitAsync();

            return new DatVeResponse
            {
                MaHoaDon = hoaDon.MaHoaDon,
                TongTien = tongTienGoc,
                NgayGiaoDich = hoaDon.NgayGiaoDich,
                TrangThai = hoaDon.TrangThai,
                Ves = vesCreated.Select(v => {
                    var ghe = ghes.First(x => x.MaGhe == v.MaGhe);
                    return new VeResponse {
                        MaVe = v.MaVe, TrangThai = v.TrangThai, GiaVe = v.GiaVe,
                        TenPhim = suatChieu.Phim.TenPhim, ThoiGianChieu = suatChieu.ThoiGianBatDau,
                        TenPhong = suatChieu.PhongChieu.TenPhong,
                        ViTriGhe = $"{ghe.Hang}{ghe.SoGhe}", LoaiGhe = ghe.LoaiGhe
                    };
                }).ToList()
            };
        }
        catch (DbUpdateException dbEx)
        {
            await transaction.RollbackAsync();
            var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
            throw new Exception($"Lỗi Database: {innerMsg}");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<DatVeResponse> DatVeAsync(int maNguoiDung, DatVeRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Validate suất chiếu
            var suatChieu = await _context.SuatChieus
                .Include(s => s.Phim).Include(s => s.PhongChieu)
                .FirstOrDefaultAsync(s => s.MaSuatChieu == request.MaSuatChieu)
                ?? throw new Exception("Suất chiếu không tồn tại.");

            // 2. Validate ghế
            var ghes = await _context.Ghes
                .Where(g => request.MaGheIds.Contains(g.MaGhe) && g.MaPhong == suatChieu.MaPhong)
                .ToListAsync();

            if (ghes.Count != request.MaGheIds.Count)
                throw new Exception("Một hoặc nhiều ghế không hợp lệ.");

            // KIỂM TRA TRÙNG (Redundant check inside transaction)
            var limitTime = DateTime.Now.AddMinutes(-10);
            var occupiedGheIds = await _context.Ves
                .Include(v => v.HoaDon)
                .Where(v => v.MaSuatChieu == request.MaSuatChieu
                         && v.TrangThai != "Đã hủy"
                         && (v.TrangThai != "Đang chờ" || v.HoaDon.NgayGiaoDich > limitTime))
                .Where(v => !request.MaHoaDon.HasValue || v.MaHoaDon != request.MaHoaDon.Value) // Bỏ qua chính đơn hàng đang thanh toán
                .Select(v => v.MaGhe)
                .ToListAsync();

            if (request.MaGheIds.Intersect(occupiedGheIds).Any())
                throw new Exception("Ghế bạn chọn đã bị người khác hoàn tất đặt vé trước. Vui lòng thử lại.");

            // 3. Tính toán tiền và khuyến mãi
            var tongTienGoc = ghes.Sum(g => g.LoaiGhe == "VIP" ? suatChieu.GiaVeGoc * 1.5m : suatChieu.GiaVeGoc);
            decimal tienGiam = 0;

            if (!string.IsNullOrEmpty(request.MaCodeGiamGia))
            {
                var km = await _context.KhuyenMais
                    .FirstOrDefaultAsync(k => k.MaCodeGiamGia == request.MaCodeGiamGia
                        && k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now);

                if (km != null)
                {
                    if (km.DonHangToiThieu.HasValue && tongTienGoc < km.DonHangToiThieu)
                        throw new Exception($"Đơn hàng tối thiểu {km.DonHangToiThieu:N0}đ để áp dụng mã này.");

                    tienGiam = km.LoaiGiamGia == "PhanTram" ? tongTienGoc * km.GiaTriGiam / 100 : km.GiaTriGiam;
                    if (km.GiamToiDa.HasValue && tienGiam > km.GiamToiDa) tienGiam = km.GiamToiDa.Value;
                }
            }
            var tongTienSauGiam = tongTienGoc - tienGiam;

            // 4. Cập nhật hoặc Tạo HoaDon
            Core.Entities.HoaDon? hoaDon;
            if (request.MaHoaDon.HasValue)
            {
                hoaDon = await _context.HoaDons.Include(h => h.Ves).FirstOrDefaultAsync(h => h.MaHoaDon == request.MaHoaDon.Value && h.MaNguoiDung == maNguoiDung)
                         ?? throw new Exception("Không tìm thấy thông tin đặt vé nháp.");
                
                hoaDon.TongTien = tongTienSauGiam;
                hoaDon.PhuongThucThanhToan = request.PhuongThucThanhToan;
                hoaDon.NgayGiaoDich = DateTime.Now;
                hoaDon.MaVaoCong = $"BL-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
                hoaDon.TrangThai = "Đã thanh toán";
                
                var km2 = string.IsNullOrEmpty(request.MaCodeGiamGia) ? null : await _context.KhuyenMais.FirstOrDefaultAsync(k => k.MaCodeGiamGia == request.MaCodeGiamGia);
                hoaDon.MaKhuyenMai = km2?.MaKhuyenMai;
            }
            else
            {
                var km2 = string.IsNullOrEmpty(request.MaCodeGiamGia) ? null : await _context.KhuyenMais.FirstOrDefaultAsync(k => k.MaCodeGiamGia == request.MaCodeGiamGia);
                hoaDon = new Core.Entities.HoaDon
                {
                    MaNguoiDung = maNguoiDung,
                    MaKhuyenMai = km2?.MaKhuyenMai,
                    TongTien = tongTienSauGiam,
                    PhuongThucThanhToan = request.PhuongThucThanhToan,
                    NgayGiaoDich = DateTime.Now,
                    MaVaoCong = $"BL-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
                    TrangThai = "Đã thanh toán"
                };
                _context.HoaDons.Add(hoaDon);
            }
            await _context.SaveChangesAsync();

            // 5. Cập nhật hoặc Tạo Vé & Đồ Ăn Vặt
            var vesResult = new List<Core.Entities.Ve>();
            if (request.MaHoaDon.HasValue)
            {
                foreach (var v in hoaDon.Ves)
                {
                    v.TrangThai = "Đã thanh toán";
                    v.MaVaoCong = $"AL-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
                    v.MaQR = GenerateQRCode($"ALPHA-{hoaDon.MaHoaDon}-{v.MaGhe}-{request.MaSuatChieu}");
                }
                vesResult = hoaDon.Ves.ToList();
            }
            else
            {
                foreach (var ghe in ghes)
                {
                    var ve = new Core.Entities.Ve
                    {
                        MaHoaDon = hoaDon.MaHoaDon,
                        MaGhe = ghe.MaGhe,
                        MaSuatChieu = request.MaSuatChieu,
                        MaQR = GenerateQRCode($"ALPHA-{hoaDon.MaHoaDon}-{ghe.MaGhe}-{request.MaSuatChieu}"),
                        MaVaoCong = $"AL-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
                        TrangThai = "Đã thanh toán",
                        GiaVe = ghe.LoaiGhe == "VIP" ? suatChieu.GiaVeGoc * 1.5m : suatChieu.GiaVeGoc
                    };
                    vesResult.Add(ve);
                }
                _context.Ves.AddRange(vesResult);
            }

            // 5.1 Xử lý đồ ăn vặt (Concessions)
            decimal concessionTotal = 0;

            // Tự động thêm quà tặng từ mã khuyến mãi vào hóa đơn (nếu có)
            if (!string.IsNullOrEmpty(request.MaCodeGiamGia))
            {
                var kmGift = await _context.KhuyenMais.FirstOrDefaultAsync(k => k.MaCodeGiamGia == request.MaCodeGiamGia);
                if (kmGift != null && kmGift.MaDoAnVatTang.HasValue)
                {
                    var giftItem = await _context.DoAnVats.FindAsync(kmGift.MaDoAnVatTang.Value);
                    if (giftItem != null)
                    {
                        var hdGift = new HoaDonDoAnVat
                        {
                            MaHoaDon = hoaDon.MaHoaDon,
                            MaDoAnVat = giftItem.MaDoAnVat,
                            SoLuong = 1,
                            DonGia = 0 // Miễn phí
                        };
                        _context.HoaDonDoAnVats.Add(hdGift);
                    }
                }
            }

            if (request.Concessions != null && request.Concessions.Any())
            {
                var concessionIds = request.Concessions.Select(c => c.MaDoAnVat).ToList();
                var dbConcessions = await _context.DoAnVats
                    .Where(d => concessionIds.Contains(d.MaDoAnVat))
                    .ToListAsync();

                foreach (var reqItem in request.Concessions)
                {
                    var dbItem = dbConcessions.FirstOrDefault(d => d.MaDoAnVat == reqItem.MaDoAnVat);
                    if (dbItem != null)
                    {
                        var hdDav = new HoaDonDoAnVat
                        {
                            MaHoaDon = hoaDon.MaHoaDon,
                            MaDoAnVat = dbItem.MaDoAnVat,
                            SoLuong = reqItem.SoLuong,
                            DonGia = dbItem.Gia
                        };
                        _context.HoaDonDoAnVats.Add(hdDav);
                        concessionTotal += reqItem.SoLuong * dbItem.Gia;
                    }
                }
            }
            
            hoaDon.TongTien += concessionTotal;

            // 6. Cộng điểm
            var nguoiDung = await _context.NguoiDungs.FindAsync(maNguoiDung);
            if (nguoiDung != null) nguoiDung.DiemTichLuy += (int)(hoaDon.TongTien / 10000);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new DatVeResponse
            {
                MaHoaDon = hoaDon.MaHoaDon,
                TongTien = tongTienSauGiam + concessionTotal,
                TienGiam = tienGiam,
                PhuongThucThanhToan = request.PhuongThucThanhToan,
                NgayGiaoDich = hoaDon.NgayGiaoDich,
                MaVaoCong = hoaDon.MaVaoCong,
                TrangThai = hoaDon.TrangThai,
                Ves = vesResult.Select(v => {
                    var g = ghes.FirstOrDefault(x => x.MaGhe == v.MaGhe);
                    return new VeResponse {
                        MaVe = v.MaVe, MaQR = v.MaQR, MaVaoCong = v.MaVaoCong, TrangThai = v.TrangThai, GiaVe = v.GiaVe,
                        TenPhim = suatChieu.Phim.TenPhim, ThoiGianChieu = suatChieu.ThoiGianBatDau, TenPhong = suatChieu.PhongChieu.TenPhong,
                        ViTriGhe = g != null ? $"{g.Hang}{g.SoGhe}" : "??", LoaiGhe = g?.LoaiGhe ?? "Thường"
                    };
                }).ToList()
            };
        }
        catch (DbUpdateException dbEx)
        {
            await transaction.RollbackAsync();
            var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
            throw new Exception($"Lỗi Database: {innerMsg}");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<VeResponse>> GetVeByNguoiDungAsync(int maNguoiDung)
    {
        return await _context.Ves
            .Include(v => v.HoaDon).Include(v => v.Ghe)
            .Include(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(v => v.SuatChieu).ThenInclude(s => s.PhongChieu)
            .Where(v => v.HoaDon.MaNguoiDung == maNguoiDung)
            .Select(v => new VeResponse
            {
                MaVe = v.MaVe, MaQR = v.MaQR, MaVaoCong = v.MaVaoCong, TrangThai = v.TrangThai, GiaVe = v.GiaVe,
                TenPhim = v.SuatChieu.Phim.TenPhim,
                ThoiGianChieu = v.SuatChieu.ThoiGianBatDau,
                TenPhong = v.SuatChieu.PhongChieu.TenPhong,
                ViTriGhe = v.Ghe.Hang.ToString() + v.Ghe.SoGhe.ToString(),
                LoaiGhe = v.Ghe.LoaiGhe
            }).ToListAsync();
    }

    public async Task<VeResponse?> GetVeByIdAsync(int maVe)
    {
        return await _context.Ves
            .Include(v => v.HoaDon).Include(v => v.Ghe)
            .Include(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(v => v.SuatChieu).ThenInclude(s => s.PhongChieu)
            .Where(v => v.MaVe == maVe)
            .Select(v => new VeResponse
            {
                MaVe = v.MaVe, MaQR = v.MaQR, TrangThai = v.TrangThai, GiaVe = v.GiaVe,
                TenPhim = v.SuatChieu.Phim.TenPhim,
                ThoiGianChieu = v.SuatChieu.ThoiGianBatDau,
                TenPhong = v.SuatChieu.PhongChieu.TenPhong,
                ViTriGhe = v.Ghe.Hang.ToString() + v.Ghe.SoGhe.ToString(),
                LoaiGhe = v.Ghe.LoaiGhe
            }).FirstOrDefaultAsync();
    }

    public async Task HuyVeAsync(int maVe, int maNguoiDung)
    {
        var ve = await _context.Ves.Include(v => v.HoaDon)
            .FirstOrDefaultAsync(v => v.MaVe == maVe)
            ?? throw new Exception("Vé không tồn tại.");

        if (ve.HoaDon.MaNguoiDung != maNguoiDung)
            throw new Exception("Bạn không có quyền hủy vé này.");

        if (ve.TrangThai == "Đã hủy")
            throw new Exception("Vé đã được hủy trước đó.");

        ve.TrangThai = "Đã hủy";
        await _context.SaveChangesAsync();
    }

    public async Task<VeResponse> CheckInTicketAsync(string code)
    {
        // 1. Try Ticket Code
        var ve = await _context.Ves
            .Include(v => v.Ghe)
            .Include(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(v => v.SuatChieu).ThenInclude(s => s.PhongChieu)
            .Include(v => v.HoaDon).ThenInclude(h => h.NguoiDung)
            .Include(v => v.HoaDon).ThenInclude(h => h.HoaDonDoAnVats).ThenInclude(hd => hd.DoAnVat)
            .FirstOrDefaultAsync(v => v.MaVaoCong == code || v.MaVe.ToString() == code);

        if (ve != null)
        {
            if (ve.TrangThai == "Đã hủy") throw new ArgumentException("Vé này đã bị hủy.");

            bool isAlreadyUsed = (ve.TrangThai == "Đã sử dụng");
            
            if (!isAlreadyUsed)
            {
                ve.TrangThai = "Đã sử dụng";
                await _context.SaveChangesAsync();
            }

            var doAn = ve.HoaDon.HoaDonDoAnVats != null 
                ? string.Join(", ", ve.HoaDon.HoaDonDoAnVats.Select(d => $"{d.DoAnVat.TenMon} (x{d.SoLuong})"))
                : "";

            return new VeResponse
            {
                MaVe = ve.MaVe, 
                MaQR = ve.MaQR, 
                MaVaoCong = ve.MaVaoCong,
                TrangThai = ve.TrangThai, 
                GiaVe = ve.GiaVe,
                TenPhim = ve.SuatChieu.Phim.TenPhim,
                ThoiGianChieu = ve.SuatChieu.ThoiGianBatDau,
                TenPhong = ve.SuatChieu.PhongChieu.TenPhong,
                ViTriGhe = $"{ve.Ghe.Hang}{ve.Ghe.SoGhe}",
                LoaiGhe = ve.Ghe.LoaiGhe,
                TenNguoiDung = ve.HoaDon.NguoiDung?.HoTen,
                DanhSachDoAn = doAn,
                DaCheckIn = isAlreadyUsed
            };
        }

        // 2. Try Bill Code
        var hd = await _context.HoaDons
            .Include(h => h.NguoiDung)
            .Include(h => h.HoaDonDoAnVats).ThenInclude(hd => hd.DoAnVat)
            .Include(h => h.Ves).ThenInclude(v => v.Ghe)
            .Include(h => h.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(h => h.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.PhongChieu)
            .FirstOrDefaultAsync(h => h.MaVaoCong == code || h.MaHoaDon.ToString() == code);

        if (hd != null)
        {
            if (hd.TrangThai == "Đã hủy") throw new ArgumentException("Hóa đơn này đã bị hủy.");

            bool isAlreadyUsed = (hd.TrangThai == "Đã sử dụng");

            if (!isAlreadyUsed)
            {
                hd.TrangThai = "Đã sử dụng";
                foreach (var v in hd.Ves) v.TrangThai = "Đã sử dụng";
                await _context.SaveChangesAsync();
            }

            var doAn = hd.HoaDonDoAnVats != null 
                ? string.Join(", ", hd.HoaDonDoAnVats.Select(d => $"{d.DoAnVat.TenMon} (x{d.SoLuong})"))
                : "";

            var firstVe = hd.Ves.First();
            return new VeResponse
            {
                MaVe = firstVe.MaVe, 
                MaQR = firstVe.MaQR, 
                MaVaoCong = hd.MaVaoCong,
                TrangThai = hd.TrangThai, 
                GiaVe = hd.TongTien,
                TenPhim = firstVe.SuatChieu.Phim.TenPhim,
                ThoiGianChieu = firstVe.SuatChieu.ThoiGianBatDau,
                TenPhong = firstVe.SuatChieu.PhongChieu.TenPhong,
                ViTriGhe = string.Join(", ", hd.Ves.Select(v => $"{v.Ghe.Hang}{v.Ghe.SoGhe}")),
                LoaiGhe = "HÓA ĐƠN",
                TenNguoiDung = hd.NguoiDung?.HoTen,
                DanhSachDoAn = doAn,
                DaCheckIn = isAlreadyUsed
            };
        }

        throw new ArgumentException("Mã vé/hóa đơn không hợp lệ hoặc không tồn tại.");
    }

    public async Task<IEnumerable<ConcessionResponse>> GetConcessionsAsync()
    {
        return await _context.DoAnVats
            .Select(d => new ConcessionResponse
            {
                MaDoAnVat = d.MaDoAnVat,
                TenMon = d.TenMon,
                MoTa = d.MoTa,
                Gia = d.Gia,
                HinhAnh = d.HinhAnh,
                Loai = d.Loai
            }).ToListAsync();
    }

    private string GenerateQRCode(string content)
    {
        // Don't use QRCoder server-side to avoid library-specific runtime errors.
        // Returning the content string instead, so frontend can generate QR image.
        return content;
    }
}
