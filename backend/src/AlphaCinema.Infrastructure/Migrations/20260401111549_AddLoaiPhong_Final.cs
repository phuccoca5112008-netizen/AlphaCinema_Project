using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlphaCinema.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoaiPhong_Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AddColumn<string>(
                name: "ma_vao_cong",
                table: "Ve",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiPhong",
                table: "PhongChieu",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ma_vao_cong",
                table: "HoaDon",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trang_thai",
                table: "HoaDon",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Đã thanh toán");

            migrationBuilder.CreateTable(
                name: "DoAnVat",
                columns: table => new
                {
                    ma_do_an_vat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_mon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mo_ta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    hinh_anh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    loai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoAnVat", x => x.ma_do_an_vat);
                });

            migrationBuilder.CreateTable(
                name: "HoaDonDoAnVat",
                columns: table => new
                {
                    ma_hoa_don_do_an_vat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_hoa_don = table.Column<int>(type: "int", nullable: false),
                    ma_do_an_vat = table.Column<int>(type: "int", nullable: false),
                    so_luong = table.Column<int>(type: "int", nullable: false),
                    don_gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonDoAnVat", x => x.ma_hoa_don_do_an_vat);
                    table.ForeignKey(
                        name: "FK_HoaDonDoAnVat_DoAnVat_ma_do_an_vat",
                        column: x => x.ma_do_an_vat,
                        principalTable: "DoAnVat",
                        principalColumn: "ma_do_an_vat",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDonDoAnVat_HoaDon_ma_hoa_don",
                        column: x => x.ma_hoa_don,
                        principalTable: "HoaDon",
                        principalColumn: "ma_hoa_don",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DoAnVat",
                columns: new[] { "ma_do_an_vat", "gia", "hinh_anh", "loai", "mo_ta", "ten_mon" },
                values: new object[,]
                {
                    { 1, 45000m, "/assets/concessions/bap_m.png", "Bắp", "Bắp rang vị caramel size vừa", "Bắp rang ngọt Caramel M" },
                    { 2, 55000m, "/assets/concessions/bap_l.png", "Bắp", "Bắp rang vị caramel size lớn", "Bắp rang ngọt Caramel L" },
                    { 3, 35000m, "/assets/concessions/coca_l.png", "Nước", "Nước ngọt Coca Cola size lớn", "Coca Cola L" },
                    { 4, 35000m, "/assets/concessions/pepsi_l.png", "Nước", "Nước ngọt Pepsi size lớn", "Pepsi L" },
                    { 5, 75000m, "/assets/concessions/combo_1.png", "Combo", "1 Bắp M + 1 Nước L", "Combo Đơn Alpha" },
                    { 6, 115000m, "/assets/concessions/combo_2.png", "Combo", "1 Bắp L + 2 Nước L", "Combo Đôi Alpha" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonDoAnVat_ma_do_an_vat",
                table: "HoaDonDoAnVat",
                column: "ma_do_an_vat");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonDoAnVat_ma_hoa_don",
                table: "HoaDonDoAnVat",
                column: "ma_hoa_don");
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropTable(
                name: "HoaDonDoAnVat");

            migrationBuilder.DropTable(
                name: "DoAnVat");

            migrationBuilder.DropColumn(
                name: "ma_vao_cong",
                table: "Ve");

            migrationBuilder.DropColumn(
                name: "LoaiPhong",
                table: "PhongChieu");

            migrationBuilder.DropColumn(
                name: "ma_vao_cong",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "trang_thai",
                table: "HoaDon");
            */
        }
    }
}
