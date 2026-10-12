using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class TangCuongValidationSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [SanPhams] SET [MoTa] = LEFT([MoTa], 2000), [HinhAnh] = LEFT([HinhAnh], 500);");
            migrationBuilder.Sql(
                "UPDATE [SanPhams] SET [TenSanPham] = N'Sản phẩm chưa đặt tên' WHERE LEN(LTRIM(RTRIM([TenSanPham]))) = 0;");
            migrationBuilder.Sql(
                "UPDATE [SanPhams] SET [ThuongHieu] = N'Chưa cập nhật' WHERE LEN(LTRIM(RTRIM([ThuongHieu]))) = 0;");
            migrationBuilder.Sql(
                "UPDATE [SanPhams] SET [NgayNhap] = '2000-01-01' WHERE [NgayNhap] < '2000-01-01';");

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "SanPhams",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "SanPhams",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_NgayNhap",
                table: "SanPhams",
                sql: "[NgayNhap] >= '2000-01-01'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_TenSanPham",
                table: "SanPhams",
                sql: "LEN(LTRIM(RTRIM([TenSanPham]))) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_ThuongHieu",
                table: "SanPhams",
                sql: "LEN(LTRIM(RTRIM([ThuongHieu]))) > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_NgayNhap",
                table: "SanPhams");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_TenSanPham",
                table: "SanPhams");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_ThuongHieu",
                table: "SanPhams");

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "SanPhams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "SanPhams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
