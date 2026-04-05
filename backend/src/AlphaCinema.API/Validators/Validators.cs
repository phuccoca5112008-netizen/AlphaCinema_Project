using AlphaCinema.Core.DTOs.Auth;
using AlphaCinema.Core.DTOs.DanhGia;
using AlphaCinema.Core.DTOs.KhuyenMai;
using AlphaCinema.Core.DTOs.Phim;
using AlphaCinema.Core.DTOs.SuatChieu;
using AlphaCinema.Core.DTOs.Ve;
using FluentValidation;

namespace AlphaCinema.API.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Tài khoản/Email không được để trống.");
        RuleFor(x => x.MatKhau).NotEmpty().MinimumLength(6).WithMessage("Mật khẩu tối thiểu 6 ký tự.");
        RuleFor(x => x.HoTen).NotEmpty().MaximumLength(100).WithMessage("Họ tên không được để trống.");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email/Tài khoản không được để trống.");
        RuleFor(x => x.MatKhau).NotEmpty().WithMessage("Mật khẩu không được để trống.");
    }
}

public class CreatePhimRequestValidator : AbstractValidator<CreatePhimRequest>
{
    public CreatePhimRequestValidator()
    {
        RuleFor(x => x.TenPhim).NotEmpty().MaximumLength(255).WithMessage("Tên phim không được để trống.");
        RuleFor(x => x.ThoiLuong).GreaterThan(0).WithMessage("Thời lượng phải lớn hơn 0.");
        RuleFor(x => x.TrangThaiPhim)
            .Must(s => new[] { "Đang chiếu", "Sắp chiếu", "Ngừng chiếu" }.Contains(s))
            .WithMessage("Trạng thái phim không hợp lệ.");
    }
}

public class CreateSuatChieuRequestValidator : AbstractValidator<CreateSuatChieuRequest>
{
    public CreateSuatChieuRequestValidator()
    {
        RuleFor(x => x.MaPhong).GreaterThan(0);
        RuleFor(x => x.MaPhim).GreaterThan(0);
        RuleFor(x => x.ThoiGianBatDau).GreaterThan(DateTime.Now).WithMessage("Thời gian chiếu phải là tương lai.");
        RuleFor(x => x.DinhDang).Must(d => new[] { "2D", "3D", "IMAX" }.Contains(d)).WithMessage("Định dạng không hợp lệ (Chỉ chấp nhận 2D, 3D, hoặc IMAX).");
        RuleFor(x => x.GiaVeGoc).GreaterThan(0).WithMessage("Giá vé phải lớn hơn 0.");
    }
}

public class DatVeRequestValidator : AbstractValidator<DatVeRequest>
{
    public DatVeRequestValidator()
    {
        RuleFor(x => x.MaSuatChieu).GreaterThan(0);
        RuleFor(x => x.MaGheIds).NotEmpty().WithMessage("Phải chọn ít nhất 1 ghế.");
        RuleFor(x => x.MaGheIds.Count).LessThanOrEqualTo(8).WithMessage("Tối đa 8 ghế mỗi lần đặt.");
        RuleFor(x => x.PhuongThucThanhToan).NotEmpty().WithMessage("Vui lòng chọn phương thức thanh toán.");
    }
}

public class CreateDanhGiaRequestValidator : AbstractValidator<CreateDanhGiaRequest>
{
    public CreateDanhGiaRequestValidator()
    {
        RuleFor(x => x.MaPhim).GreaterThan(0);
        RuleFor(x => x.DiemSo).InclusiveBetween(1, 5).WithMessage("Điểm số phải từ 1 đến 5.");
    }
}

public class CreateKhuyenMaiRequestValidator : AbstractValidator<CreateKhuyenMaiRequest>
{
    public CreateKhuyenMaiRequestValidator()
    {
        RuleFor(x => x.TenKhuyenMai).NotEmpty().MaximumLength(255);
        RuleFor(x => x.MaCodeGiamGia).NotEmpty().MaximumLength(50);
        RuleFor(x => x.NgayKetThuc).GreaterThan(x => x.NgayBatDau).WithMessage("Ngày kết thúc phải sau ngày bắt đầu.");
        RuleFor(x => x.LoaiGiamGia).Must(l => new[] { "PhanTram", "CoDinh" }.Contains(l)).WithMessage("Loại giảm giá không hợp lệ.");
        RuleFor(x => x.GiaTriGiam).GreaterThan(0).WithMessage("Giá trị giảm phải lớn hơn 0.");
    }
}
