using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphaCinema.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionImageAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ve_ma_suat_chieu_ma_ghe",
                table: "Ve");

            migrationBuilder.RenameColumn(
                name: "LoaiPhong",
                table: "PhongChieu",
                newName: "loai_phong");

            migrationBuilder.AlterColumn<string>(
                name: "ten_phong",
                table: "PhongChieu",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "loai_phong",
                table: "PhongChieu",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "KhuyenMai",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhanLoai",
                table: "KhuyenMai",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ve_ma_suat_chieu_ma_ghe",
                table: "Ve",
                columns: new[] { "ma_suat_chieu", "ma_ghe" },
                unique: true,
                filter: "[trang_thai] <> N'Đã hủy'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ve_ma_suat_chieu_ma_ghe",
                table: "Ve");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "KhuyenMai");

            migrationBuilder.DropColumn(
                name: "PhanLoai",
                table: "KhuyenMai");

            migrationBuilder.RenameColumn(
                name: "loai_phong",
                table: "PhongChieu",
                newName: "LoaiPhong");

            migrationBuilder.AlterColumn<string>(
                name: "ten_phong",
                table: "PhongChieu",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiPhong",
                table: "PhongChieu",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Ve_ma_suat_chieu_ma_ghe",
                table: "Ve",
                columns: new[] { "ma_suat_chieu", "ma_ghe" },
                unique: true);
        }
    }
}
