using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class ChuanHoaDuLieuTiengVietSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @SeedImages TABLE ([HinhAnh] nvarchar(500) NOT NULL PRIMARY KEY);
                INSERT INTO @SeedImages ([HinhAnh]) VALUES
                    (N'https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=400'),
                    (N'https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=400'),
                    (N'https://images.unsplash.com/photo-1545454675-3531b543be5d?w=400'),
                    (N'https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=400'),
                    (N'https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=400'),
                    (N'https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=400'),
                    (N'https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?w=400'),
                    (N'https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?w=400'),
                    (N'https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=400'),
                    (N'https://images.unsplash.com/photo-1595225476474-87563907a212?w=400'),
                    (N'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400'),
                    (N'https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=400'),
                    (N'https://images.unsplash.com/photo-1575311373937-040b8e1fd5b6?w=400');

                ;WITH ProductCopies AS
                (
                    SELECT s.[MaSanPham], s.[HinhAnh],
                           MIN(s.[MaSanPham]) OVER (PARTITION BY s.[HinhAnh]) AS [KeepId],
                           MAX(s.[MaSanPham]) OVER (PARTITION BY s.[HinhAnh]) AS [CleanId]
                    FROM [SanPhams] s
                    INNER JOIN @SeedImages i ON i.[HinhAnh] = s.[HinhAnh]
                )
                UPDATE target
                SET target.[TenSanPham] = clean.[TenSanPham],
                    target.[MoTa] = clean.[MoTa]
                FROM [SanPhams] target
                INNER JOIN ProductCopies copies ON copies.[MaSanPham] = copies.[KeepId]
                                                  AND copies.[KeepId] <> copies.[CleanId]
                                                  AND target.[MaSanPham] = copies.[KeepId]
                INNER JOIN [SanPhams] clean ON clean.[MaSanPham] = copies.[CleanId];

                ;WITH ProductCopies AS
                (
                    SELECT s.[MaSanPham],
                           MIN(s.[MaSanPham]) OVER (PARTITION BY s.[HinhAnh]) AS [KeepId]
                    FROM [SanPhams] s
                    INNER JOIN @SeedImages i ON i.[HinhAnh] = s.[HinhAnh]
                )
                UPDATE detail
                SET detail.[MaSanPham] = copies.[KeepId]
                FROM [ChiTietDonHangs] detail
                INNER JOIN ProductCopies copies ON copies.[MaSanPham] = detail.[MaSanPham]
                WHERE copies.[MaSanPham] <> copies.[KeepId];

                ;WITH ProductCopies AS
                (
                    SELECT s.[MaSanPham],
                           MIN(s.[MaSanPham]) OVER (PARTITION BY s.[HinhAnh]) AS [KeepId]
                    FROM [SanPhams] s
                    INNER JOIN @SeedImages i ON i.[HinhAnh] = s.[HinhAnh]
                )
                DELETE product
                FROM [SanPhams] product
                INNER JOIN ProductCopies copies ON copies.[MaSanPham] = product.[MaSanPham]
                WHERE copies.[MaSanPham] <> copies.[KeepId];
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
