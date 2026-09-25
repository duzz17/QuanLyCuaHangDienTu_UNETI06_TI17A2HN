using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class AddSanPhamTrangThaiVaRangBuoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "SanPhams",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_Gia",
                table: "SanPhams",
                sql: "[Gia] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_SoLuong",
                table: "SanPhams",
                sql: "[SoLuong] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_Gia",
                table: "SanPhams");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_SoLuong",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "SanPhams");
        }
    }
}
