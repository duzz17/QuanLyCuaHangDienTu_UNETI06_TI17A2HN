using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckConstraintBaoHanh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_ThoiGianBaoHanh",
                table: "SanPhams",
                sql: "[ThoiGianBaoHanh] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_ThoiGianBaoHanh",
                table: "SanPhams");
        }
    }
}
