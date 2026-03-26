using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlphaCinema.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhuyenMai",
                columns: table => new
                {
                    ma_khuyen_mai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_khuyen_mai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ma_code_giam_gia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    mo_ta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ngay_bat_dau = table.Column<DateTime>(type: "date", nullable: false),
                    ngay_ket_thuc = table.Column<DateTime>(type: "date", nullable: false),
                    loai_giam_gia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    gia_tri_giam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    giam_toi_da = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    don_hang_toi_thieu = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMai", x => x.ma_khuyen_mai);
                });

            migrationBuilder.CreateTable(
                name: "Phim",
                columns: table => new
                {
                    ma_phim = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_phim = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    the_loai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    thoi_luong = table.Column<int>(type: "int", nullable: false),
                    poster = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    tom_tat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    trang_thai_phim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Sắp chiếu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phim", x => x.ma_phim);
                });

            migrationBuilder.CreateTable(
                name: "PhongChieu",
                columns: table => new
                {
                    ma_phong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_phong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongChieu", x => x.ma_phong);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    ma_vai_tro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_vai_tro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.ma_vai_tro);
                });

            migrationBuilder.CreateTable(
                name: "Ghe",
                columns: table => new
                {
                    ma_ghe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_phong = table.Column<int>(type: "int", nullable: false),
                    hang = table.Column<string>(type: "char(1)", nullable: false),
                    so_ghe = table.Column<int>(type: "int", nullable: false),
                    loai_ghe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ghe", x => x.ma_ghe);
                    table.ForeignKey(
                        name: "FK_Ghe_PhongChieu_ma_phong",
                        column: x => x.ma_phong,
                        principalTable: "PhongChieu",
                        principalColumn: "ma_phong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SuatChieu",
                columns: table => new
                {
                    ma_suat_chieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_phong = table.Column<int>(type: "int", nullable: false),
                    ma_phim = table.Column<int>(type: "int", nullable: false),
                    thoi_gian_bat_dau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dinh_dang = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    gia_ve_goc = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuatChieu", x => x.ma_suat_chieu);
                    table.ForeignKey(
                        name: "FK_SuatChieu_Phim_ma_phim",
                        column: x => x.ma_phim,
                        principalTable: "Phim",
                        principalColumn: "ma_phim",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuatChieu_PhongChieu_ma_phong",
                        column: x => x.ma_phong,
                        principalTable: "PhongChieu",
                        principalColumn: "ma_phong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    ma_nguoi_dung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_vai_tro = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mat_khau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ho_ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    diem_tich_luy = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.ma_nguoi_dung);
                    table.ForeignKey(
                        name: "FK_NguoiDung_VaiTro_ma_vai_tro",
                        column: x => x.ma_vai_tro,
                        principalTable: "VaiTro",
                        principalColumn: "ma_vai_tro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    ma_danh_gia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_phim = table.Column<int>(type: "int", nullable: false),
                    ma_nguoi_dung = table.Column<int>(type: "int", nullable: false),
                    noi_dung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    diem_so = table.Column<int>(type: "int", nullable: false),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGia", x => x.ma_danh_gia);
                    table.ForeignKey(
                        name: "FK_DanhGia_NguoiDung_ma_nguoi_dung",
                        column: x => x.ma_nguoi_dung,
                        principalTable: "NguoiDung",
                        principalColumn: "ma_nguoi_dung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhGia_Phim_ma_phim",
                        column: x => x.ma_phim,
                        principalTable: "Phim",
                        principalColumn: "ma_phim",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    ma_hoa_don = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_khuyen_mai = table.Column<int>(type: "int", nullable: true),
                    ma_nguoi_dung = table.Column<int>(type: "int", nullable: false),
                    tong_tien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    phuong_thuc_thanh_toan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ngay_giao_dich = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.ma_hoa_don);
                    table.ForeignKey(
                        name: "FK_HoaDon_KhuyenMai_ma_khuyen_mai",
                        column: x => x.ma_khuyen_mai,
                        principalTable: "KhuyenMai",
                        principalColumn: "ma_khuyen_mai",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HoaDon_NguoiDung_ma_nguoi_dung",
                        column: x => x.ma_nguoi_dung,
                        principalTable: "NguoiDung",
                        principalColumn: "ma_nguoi_dung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ve",
                columns: table => new
                {
                    ma_ve = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_hoa_don = table.Column<int>(type: "int", nullable: false),
                    ma_ghe = table.Column<int>(type: "int", nullable: false),
                    ma_suat_chieu = table.Column<int>(type: "int", nullable: false),
                    ma_qr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    trang_thai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    gia_ve = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ve", x => x.ma_ve);
                    table.ForeignKey(
                        name: "FK_Ve_Ghe_ma_ghe",
                        column: x => x.ma_ghe,
                        principalTable: "Ghe",
                        principalColumn: "ma_ghe",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ve_HoaDon_ma_hoa_don",
                        column: x => x.ma_hoa_don,
                        principalTable: "HoaDon",
                        principalColumn: "ma_hoa_don",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ve_SuatChieu_ma_suat_chieu",
                        column: x => x.ma_suat_chieu,
                        principalTable: "SuatChieu",
                        principalColumn: "ma_suat_chieu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "VaiTro",
                columns: new[] { "ma_vai_tro", "ten_vai_tro" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Staff" },
                    { 3, "Customer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_ma_nguoi_dung",
                table: "DanhGia",
                column: "ma_nguoi_dung");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_ma_phim",
                table: "DanhGia",
                column: "ma_phim");

            migrationBuilder.CreateIndex(
                name: "IX_Ghe_ma_phong",
                table: "Ghe",
                column: "ma_phong");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_ma_khuyen_mai",
                table: "HoaDon",
                column: "ma_khuyen_mai");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_ma_nguoi_dung",
                table: "HoaDon",
                column: "ma_nguoi_dung");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_email",
                table: "NguoiDung",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_ma_vai_tro",
                table: "NguoiDung",
                column: "ma_vai_tro");

            migrationBuilder.CreateIndex(
                name: "IX_SuatChieu_ma_phim",
                table: "SuatChieu",
                column: "ma_phim");

            migrationBuilder.CreateIndex(
                name: "IX_SuatChieu_ma_phong",
                table: "SuatChieu",
                column: "ma_phong");

            migrationBuilder.CreateIndex(
                name: "IX_Ve_ma_ghe",
                table: "Ve",
                column: "ma_ghe");

            migrationBuilder.CreateIndex(
                name: "IX_Ve_ma_hoa_don",
                table: "Ve",
                column: "ma_hoa_don");

            migrationBuilder.CreateIndex(
                name: "IX_Ve_ma_suat_chieu_ma_ghe",
                table: "Ve",
                columns: new[] { "ma_suat_chieu", "ma_ghe" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "Ve");

            migrationBuilder.DropTable(
                name: "Ghe");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "SuatChieu");

            migrationBuilder.DropTable(
                name: "KhuyenMai");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "Phim");

            migrationBuilder.DropTable(
                name: "PhongChieu");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
