using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class HoanTatChuanHoaTiengViet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [SanPhams] SET [MoTa] = N'Tích hợp quyền năng Galaxy AI thông minh, bút S-Pen quyền năng, màn hình phẳng Dynamic AMOLED 2X 120Hz chống chói đỉnh cao.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Hệ thống 4 camera quang học Leica đỉnh cao cảm biến 1-inch, Snapdragon 8 Gen 3 siêu tốc, sạc HyperCharge 90W nhanh ấn tượng.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Thiết kế mỏng kỷ lục 5.3mm, chip Apple M4 thế hệ mới, màn hình Ultra Retina XDR Tandem OLED tuyệt mỹ, tương thích Apple Pencil Pro.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Màn hình cực đại 14.6 inch sắc nét, chuẩn kháng nước kháng bụi IP68, xử lý đa nhiệm xuất sắc thay thế máy tính bảng thông thường.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1561154464-82e9adf32764?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Chip Apple M3 Pro 11-core CPU, 14-core GPU, màn hình Liquid Retina XDR 120Hz ProMotion, thời lượng pin sử dụng liên tục đến 18 giờ.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Vi xử lý Intel Core i7-13700H thế hệ 13, RAM 16GB DDR5, SSD 512GB, card đồ họa rời NVIDIA RTX 4050 6GB GDDR6, màn hình OLED viền mỏng.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Laptop Gaming siêu mỏng cao cấp, màn hình ROG Nebula OLED 2.5K 240Hz, vi xử lý Intel Core Ultra 9 185H, đồ họa NVIDIA GeForce RTX 4070.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Vỏ sợi carbon siêu bền chuẩn quân sự Mỹ, bàn phím gõ êm nhất thế giới, trọng lượng chỉ 1.12kg, bảo hành chính hãng tận nơi 3 năm.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Màn hình OLED 2.8K 90Hz rực rỡ, tích hợp bộ xử lý AI Intel NPU, thiết kế vỏ nhôm mỏng nhẹ sang trọng, pin dùng cả ngày.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Chip xử lý Apple H2 nâng cao khả năng chống ồn gấp 2 lần, âm thanh thích ứng Adaptive Audio, chuẩn kháng nước IP54 tiện lợi.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1600294037681-c80b4cb5b434?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Vỏ Titan chuẩn quân sự chống va đập, màn hình sáng kỷ lục 3000 nits, định vị GPS tần số kép L1/L5 cực chuẩn cho leo núi và lặn biển.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?w=400';
                UPDATE [SanPhams] SET [MoTa] = N'Vòng xoay bezel xoay cơ học độc đáo, kính Sapphire chống trầy, đo điện tâm đồ ECG, phân tích thành phần cơ thể BIA và giấc ngủ.' WHERE [HinhAnh] = N'https://images.unsplash.com/photo-1579586337278-3befd40fd17a?w=400';

                UPDATE [KhachHangs]
                SET [HoTen] = N'Nguyễn Văn A',
                    [DiaChi] = N'218 Lĩnh Nam, Hoàng Mai, Hà Nội'
                WHERE [Email] = N'nguyenvana@gmail.com';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
