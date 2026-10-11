using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class ThemXuatXuVaBaTrangThaiSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_ThoiGianBaoHanh",
                table: "SanPhams");

            migrationBuilder.AlterColumn<int>(
                name: "TrangThai",
                table: "SanPhams",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "XuatXu",
                table: "SanPhams",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [SanPhams]
                SET [XuatXu] = CASE [ThuongHieu]
                    WHEN N'Apple' THEN N'Mỹ'
                    WHEN N'Samsung' THEN N'Hàn Quốc'
                    WHEN N'Xiaomi' THEN N'Trung Quốc'
                    WHEN N'Dell' THEN N'Mỹ'
                    WHEN N'Asus' THEN N'Đài Loan'
                    WHEN N'Lenovo' THEN N'Trung Quốc'
                    WHEN N'Acer' THEN N'Đài Loan'
                    WHEN N'Sony' THEN N'Nhật Bản'
                    WHEN N'Marshall' THEN N'Anh'
                    WHEN N'JBL' THEN N'Mỹ'
                    WHEN N'Logitech' THEN N'Thụy Sĩ'
                    WHEN N'Akko' THEN N'Trung Quốc'
                    WHEN N'Razer' THEN N'Singapore'
                    WHEN N'Corsair' THEN N'Mỹ'
                    WHEN N'Garmin' THEN N'Mỹ'
                    WHEN N'Huawei' THEN N'Trung Quốc'
                    ELSE N'Chưa cập nhật'
                END;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "XuatXu",
                table: "SanPhams",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [SanPhams]
                SET [ThoiGianBaoHanh] = CASE
                    WHEN [ThoiGianBaoHanh] < 0 THEN 0
                    WHEN [ThoiGianBaoHanh] > 120 THEN 120
                    ELSE [ThoiGianBaoHanh]
                END;
                """);

            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_ThoiGianBaoHanh",
                table: "SanPhams",
                sql: "[ThoiGianBaoHanh] BETWEEN 0 AND 120");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_TrangThai",
                table: "SanPhams",
                sql: "[TrangThai] IN (0, 1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_XuatXu",
                table: "SanPhams",
                sql: "LEN(LTRIM(RTRIM([XuatXu]))) > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_ThoiGianBaoHanh",
                table: "SanPhams");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_TrangThai",
                table: "SanPhams");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SanPhams_XuatXu",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "XuatXu",
                table: "SanPhams");

            migrationBuilder.Sql(
                "UPDATE [SanPhams] SET [TrangThai] = 0 WHERE [TrangThai] = 2;");

            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "SanPhams",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SanPhams_ThoiGianBaoHanh",
                table: "SanPhams",
                sql: "[ThoiGianBaoHanh] >= 0");
        }
    }
}
