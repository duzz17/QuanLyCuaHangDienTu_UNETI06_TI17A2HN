using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;

// Họ và tên: Phạm Văn Cường - MSSV: 23103100094 (Module 2: M2-13 Chuẩn bị 25 sản phẩm mẫu trải đều 5 loại sản phẩm)
// Phối hợp cùng TV1 (Module 1 - M1-02: Quản lý loại sản phẩm và tài khoản)
public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Tự động tạo cơ sở dữ liệu và áp dụng các Migration mới nhất
        context.Database.Migrate();

        // 1. Tạo Khách hàng và Tài khoản ban đầu nếu chưa có
        if (!context.TaiKhoans.Any())
        {
            var adminKhachHang = new KhachHang
            {
                HoTen = "Quản trị viên Hệ thống",
                Email = "admin@uneti.edu.vn",
                SoDienThoai = "0988888888",
                DiaChi = "Đại học UNETI - Hà Nội",
                NgayDangKy = DateTime.Now.AddMonths(-3)
            };

            var userKhachHang1 = new KhachHang
            {
                HoTen = "Nguyễn Văn A",
                Email = "nguyenvana@gmail.com",
                SoDienThoai = "0912345678",
                DiaChi = "218 Lĩnh Nam, Hoàng Mai, Hà Nội",
                NgayDangKy = DateTime.Now.AddMonths(-2)
            };

            var userKhachHang2 = new KhachHang
            {
                HoTen = "Trần Thị B",
                Email = "tranthib@gmail.com",
                SoDienThoai = "0976543210",
                DiaChi = "456 Minh Khai, Hai Bà Trưng, Hà Nội",
                NgayDangKy = DateTime.Now.AddMonths(-1)
            };

            context.KhachHangs.AddRange(adminKhachHang, userKhachHang1, userKhachHang2);
            context.SaveChanges();

            var adminTaiKhoan = new TaiKhoan
            {
                TenDangNhap = "admin",
                MatKhau = "admin123",
                VaiTro = "Admin",
                TrangThai = true,
                MaKhachHang = adminKhachHang.MaKhachHang
            };

            var khachHangTaiKhoan = new TaiKhoan
            {
                TenDangNhap = "khachhang",
                MatKhau = "123456",
                VaiTro = "KhachHang",
                TrangThai = true,
                MaKhachHang = userKhachHang1.MaKhachHang
            };

            context.TaiKhoans.AddRange(adminTaiKhoan, khachHangTaiKhoan);
            context.SaveChanges();
        }

        // 2. Đảm bảo tối thiểu 5 Loại sản phẩm (M2-13 phối hợp TV1)
        var loaiDienThoai = context.LoaiSanPhams.FirstOrDefault(l => l.TenLoai.Contains("Điện thoại") || l.TenLoai.Contains("Dien thoai"));
        if (loaiDienThoai == null)
        {
            loaiDienThoai = new LoaiSanPham { TenLoai = "Điện thoại & Máy tính bảng", MoTa = "Điện thoại thông minh, iPad, tablet chính hãng" };
            context.LoaiSanPhams.Add(loaiDienThoai);
            context.SaveChanges();
        }

        var loaiLaptop = context.LoaiSanPhams.FirstOrDefault(l => l.TenLoai.Contains("Laptop"));
        if (loaiLaptop == null)
        {
            loaiLaptop = new LoaiSanPham { TenLoai = "Laptop & Máy tính xách tay", MoTa = "Laptop văn phòng, đồ họa, laptop gaming cao cấp" };
            context.LoaiSanPhams.Add(loaiLaptop);
            context.SaveChanges();
        }

        var loaiTaiNghe = context.LoaiSanPhams.FirstOrDefault(l => l.TenLoai.Contains("Tai nghe") || l.TenLoai.Contains("Âm thanh") || l.TenLoai.Contains("Am thanh"));
        if (loaiTaiNghe == null)
        {
            loaiTaiNghe = new LoaiSanPham { TenLoai = "Tai nghe & Âm thanh", MoTa = "Tai nghe True Wireless, tai nghe chụp tai, loa Bluetooth" };
            context.LoaiSanPhams.Add(loaiTaiNghe);
            context.SaveChanges();
        }

        var loaiBanPhim = context.LoaiSanPhams.FirstOrDefault(l => l.TenLoai.Contains("Bàn phím") || l.TenLoai.Contains("Ban phim") || l.TenLoai.Contains("Chuột") || l.TenLoai.Contains("Chuot"));
        if (loaiBanPhim == null)
        {
            loaiBanPhim = new LoaiSanPham { TenLoai = "Bàn phím & Chuột", MoTa = "Bàn phím cơ, bàn phím không dây, chuột công thái học, chuột gaming" };
            context.LoaiSanPhams.Add(loaiBanPhim);
            context.SaveChanges();
        }

        var loaiDongHo = context.LoaiSanPhams.FirstOrDefault(l => l.TenLoai.Contains("Đồng hồ") || l.TenLoai.Contains("Dong ho") || l.TenLoai.Contains("Smartwatch"));
        if (loaiDongHo == null)
        {
            loaiDongHo = new LoaiSanPham { TenLoai = "Đồng hồ thông minh", MoTa = "Smartwatch, Smartband theo dõi sức khỏe và thể thao chuyên nghiệp" };
            context.LoaiSanPhams.Add(loaiDongHo);
            context.SaveChanges();
        }

        // 3. M2-13: Chuẩn bị 25 sản phẩm mẫu trải đều 5 loại sản phẩm nếu chưa có sản phẩm
        if (!context.SanPhams.Any())
        {
            var dsSanPham = new List<SanPham>
            {
                // ===== LOẠI 1: ĐIỆN THOẠI & MÁY TÍNH BẢNG (5 SP) =====
                new SanPham
                {
                    TenSanPham = "iPhone 15 Pro Max 256GB Titan Tự Nhiên",
                    ThuongHieu = "Apple",
                    Gia = 32990000m,
                    SoLuong = 15,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=400",
                    MoTa = "Khung viền Titan siêu nhẹ, chip Apple A17 Pro mạnh mẽ nhất thế giới, camera tiềm vọng zoom 5x sắc nét, cổng USB-C tốc độ cao.",
                    NgayNhap = DateTime.Now.AddDays(-20),
                    MaLoai = loaiDienThoai.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Samsung Galaxy S24 Ultra 5G 512GB",
                    ThuongHieu = "Samsung",
                    Gia = 29990000m,
                    SoLuong = 20,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=400",
                    MoTa = "Tích hợp quyền năng Galaxy AI thông minh, bút S-Pen quyền năng, màn hình phẳng Dynamic AMOLED 2X 120Hz chống chói đỉnh cao.",
                    NgayNhap = DateTime.Now.AddDays(-18),
                    MaLoai = loaiDienThoai.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Xiaomi 14 Ultra 5G 16GB/512GB",
                    ThuongHieu = "Xiaomi",
                    Gia = 24490000m,
                    SoLuong = 10,
                    ThoiGianBaoHanh = 18,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=400",
                    MoTa = "Hệ thống 4 camera quang học Leica đỉnh cao cảm biến 1-inch, Snapdragon 8 Gen 3 siêu tốc, sạc HyperCharge 90W nhanh ấn tượng.",
                    NgayNhap = DateTime.Now.AddDays(-15),
                    MaLoai = loaiDienThoai.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "iPad Pro 11 inch M4 Wi-Fi 256GB",
                    ThuongHieu = "Apple",
                    Gia = 27990000m,
                    SoLuong = 8,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400",
                    MoTa = "Thiết kế mỏng kỷ lục 5.3mm, chip Apple M4 thế hệ mới, màn hình Ultra Retina XDR Tandem OLED tuyệt mỹ, tương thích Apple Pencil Pro.",
                    NgayNhap = DateTime.Now.AddDays(-12),
                    MaLoai = loaiDienThoai.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Samsung Galaxy Tab S9 Ultra 5G",
                    ThuongHieu = "Samsung",
                    Gia = 22990000m,
                    SoLuong = 5,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1561154464-82e9adf32764?w=400",
                    MoTa = "Màn hình cực đại 14.6 inch sắc nét, chuẩn kháng nước kháng bụi IP68, xử lý đa nhiệm xuất sắc thay thế máy tính bảng thông thường.",
                    NgayNhap = DateTime.Now.AddDays(-10),
                    MaLoai = loaiDienThoai.MaLoai
                },

                // ===== LOẠI 2: LAPTOP & MÁY TÍNH XÁCH TAY (5 SP) =====
                new SanPham
                {
                    TenSanPham = "MacBook Pro 14 inch M3 Pro 18GB/512GB",
                    ThuongHieu = "Apple",
                    Gia = 49990000m,
                    SoLuong = 12,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400",
                    MoTa = "Chip Apple M3 Pro 11-core CPU, 14-core GPU, màn hình Liquid Retina XDR 120Hz ProMotion, thời lượng pin sử dụng liên tục đến 18 giờ.",
                    NgayNhap = DateTime.Now.AddDays(-25),
                    MaLoai = loaiLaptop.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Laptop Dell XPS 15 9530 Core i7-13700H",
                    ThuongHieu = "Dell",
                    Gia = 45500000m,
                    SoLuong = 8,
                    ThoiGianBaoHanh = 24,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=400",
                    MoTa = "Vi xử lý Intel Core i7-13700H thế hệ 13, RAM 16GB DDR5, SSD 512GB, card đồ họa rời NVIDIA RTX 4050 6GB GDDR6, màn hình OLED viền mỏng.",
                    NgayNhap = DateTime.Now.AddDays(-22),
                    MaLoai = loaiLaptop.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Laptop Asus ROG Zephyrus G16 OLED Gaming",
                    ThuongHieu = "Asus",
                    Gia = 52990000m,
                    SoLuong = 6,
                    ThoiGianBaoHanh = 24,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=400",
                    MoTa = "Laptop Gaming siêu mỏng cao cấp, màn hình ROG Nebula OLED 2.5K 240Hz, vi xử lý Intel Core Ultra 9 185H, đồ họa NVIDIA GeForce RTX 4070.",
                    NgayNhap = DateTime.Now.AddDays(-19),
                    MaLoai = loaiLaptop.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Lenovo ThinkPad X1 Carbon Gen 11 Core i7",
                    ThuongHieu = "Lenovo",
                    Gia = 38900000m,
                    SoLuong = 10,
                    ThoiGianBaoHanh = 36,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=400",
                    MoTa = "Vỏ sợi carbon siêu bền chuẩn quân sự Mỹ, bàn phím gõ êm nhất thế giới, trọng lượng chỉ 1.12kg, bảo hành chính hãng tận nơi 3 năm.",
                    NgayNhap = DateTime.Now.AddDays(-14),
                    MaLoai = loaiLaptop.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Laptop Acer Swift Go 14 AI OLED Intel Core Ultra 5",
                    ThuongHieu = "Acer",
                    Gia = 18990000m,
                    SoLuong = 15,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400",
                    MoTa = "Màn hình OLED 2.8K 90Hz rực rỡ, tích hợp bộ xử lý AI Intel NPU, thiết kế vỏ nhôm mỏng nhẹ sang trọng, pin dùng cả ngày.",
                    NgayNhap = DateTime.Now.AddDays(-8),
                    MaLoai = loaiLaptop.MaLoai
                },

                // ===== LOẠI 3: TAI NGHE & ÂM THANH (5 SP) =====
                new SanPham
                {
                    TenSanPham = "Tai nghe Chống ồn Sony WH-1000XM5 Hi-Res",
                    ThuongHieu = "Sony",
                    Gia = 7490000m,
                    SoLuong = 25,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=400",
                    MoTa = "Công nghệ chống ồn chủ động kép với 8 micro lọc âm, chất âm chuẩn Hi-Res Audio, thời lượng pin 30 giờ, kết nối đa thiết bị.",
                    NgayNhap = DateTime.Now.AddDays(-16),
                    MaLoai = loaiTaiNghe.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Tai nghe Apple AirPods Pro 2 USB-C MagSafe",
                    ThuongHieu = "Apple",
                    Gia = 5690000m,
                    SoLuong = 30,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1600294037681-c80b4cb5b434?w=400",
                    MoTa = "Chip xử lý Apple H2 nâng cao khả năng chống ồn gấp 2 lần, âm thanh thích ứng Adaptive Audio, chuẩn kháng nước IP54 tiện lợi.",
                    NgayNhap = DateTime.Now.AddDays(-13),
                    MaLoai = loaiTaiNghe.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Loa Bluetooth Marshall Stanmore III Chính Hãng",
                    ThuongHieu = "Marshall",
                    Gia = 9290000m,
                    SoLuong = 10,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1545454675-3531b543be5d?w=400",
                    MoTa = "Âm trường rộng hơn bao giờ hết đặc trưng phong cách Rock Marshall, núm vặn analog mạ vàng cổ điển, hỗ trợ Bluetooth 5.2 và AUX 3.5mm.",
                    NgayNhap = DateTime.Now.AddDays(-11),
                    MaLoai = loaiTaiNghe.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Loa Di Động Chống Nước JBL Charge 5",
                    ThuongHieu = "JBL",
                    Gia = 3490000m,
                    SoLuong = 20,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=400",
                    MoTa = "Chất âm sống động JBL Original Pro Sound, màng loa bass kép uy lực, chuẩn chống nước bụi IP67, pin trâu 20 giờ có tính năng sạc dự phòng.",
                    NgayNhap = DateTime.Now.AddDays(-9),
                    MaLoai = loaiTaiNghe.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Tai nghe Gaming Không dây Logitech G Pro X 2 LIGHTSPEED",
                    ThuongHieu = "Logitech",
                    Gia = 5490000m,
                    SoLuong = 12,
                    ThoiGianBaoHanh = 24,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=400",
                    MoTa = "Màng loa Graphene 50mm cách mạng, định vị âm thanh không gian chuẩn xác trong trận đấu, kết nối LIGHTSPEED siêu nhanh không độ trễ.",
                    NgayNhap = DateTime.Now.AddDays(-7),
                    MaLoai = loaiTaiNghe.MaLoai
                },

                // ===== LOẠI 4: BÀN PHÍM & CHUỘT (5 SP) =====
                new SanPham
                {
                    TenSanPham = "Bàn phím cơ không dây Logitech MX Keys S",
                    ThuongHieu = "Logitech",
                    Gia = 2890000m,
                    SoLuong = 30,
                    ThoiGianBaoHanh = 24,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=400",
                    MoTa = "Phím bấm thiết kế lõm công thái học gõ cực êm và chính xác, đèn nền tự động cảm biến tiệm cận bàn tay, kết nối mượt 3 thiết bị cùng lúc.",
                    NgayNhap = DateTime.Now.AddDays(-21),
                    MaLoai = loaiBanPhim.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Chuột công thái học cao cấp Logitech MX Master 3S",
                    ThuongHieu = "Logitech",
                    Gia = 2290000m,
                    SoLuong = 35,
                    ThoiGianBaoHanh = 24,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?w=400",
                    MoTa = "Cuộn bánh xe từ tính MagSpeed 1000 dòng/giây, công nghệ Quiet Clicks giảm 90% tiếng ồn, cảm biến quang học 8000 DPI lướt mượt trên mặt kính.",
                    NgayNhap = DateTime.Now.AddDays(-17),
                    MaLoai = loaiBanPhim.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Bàn phím cơ Custom Akko 5075B Plus v2 Multi-Modes",
                    ThuongHieu = "Akko",
                    Gia = 1950000m,
                    SoLuong = 18,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?w=400",
                    MoTa = "Cấu trúc Gasket Mount êm ái đàn hồi, mạch Hotswap 5 pin thay switch nhanh chóng, đèn LED RGB từng phím kèm dải viền thời thượng.",
                    NgayNhap = DateTime.Now.AddDays(-15),
                    MaLoai = loaiBanPhim.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Chuột Gaming Không dây Siêu nhẹ Razer Viper V2 Pro",
                    ThuongHieu = "Razer",
                    Gia = 3190000m,
                    SoLuong = 14,
                    ThoiGianBaoHanh = 24,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=400",
                    MoTa = "Trọng lượng siêu nhẹ chỉ 58g tối ưu cho tuyển thủ eSports, cảm biến quang học Focus Pro 30K, switch bấm quang học Gen 3 không lo double-click.",
                    NgayNhap = DateTime.Now.AddDays(-12),
                    MaLoai = loaiBanPhim.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Bàn phím cơ Gaming Corsair K70 RGB PRO Cherry MX Red",
                    ThuongHieu = "Corsair",
                    Gia = 3790000m,
                    SoLuong = 7,
                    ThoiGianBaoHanh = 24,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1595225476474-87563907a212?w=400",
                    MoTa = "Khung nhôm phay xước chuẩn hàng không vũ trụ, công nghệ xử lý siêu tốc AXON 8000Hz, keycap PBT double-shot chống mài mòn bóng mờ.",
                    NgayNhap = DateTime.Now.AddDays(-6),
                    MaLoai = loaiBanPhim.MaLoai
                },

                // ===== LOẠI 5: ĐỒNG HỒ THÔNG MINH (5 SP) =====
                new SanPham
                {
                    TenSanPham = "Apple Watch Ultra 2 GPS + Cellular 49mm Titan",
                    ThuongHieu = "Apple",
                    Gia = 20990000m,
                    SoLuong = 8,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?w=400",
                    MoTa = "Vỏ Titan chuẩn quân sự chống va đập, màn hình sáng kỷ lục 3000 nits, định vị GPS tần số kép L1/L5 cực chuẩn cho leo núi và lặn biển.",
                    NgayNhap = DateTime.Now.AddDays(-14),
                    MaLoai = loaiDongHo.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Samsung Galaxy Watch 6 Classic 47mm LTE",
                    ThuongHieu = "Samsung",
                    Gia = 7990000m,
                    SoLuong = 15,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?w=400",
                    MoTa = "Vòng xoay bezel xoay cơ học độc đáo, kính Sapphire chống trầy, đo điện tâm đồ ECG, phân tích thành phần cơ thể BIA và giấc ngủ.",
                    NgayNhap = DateTime.Now.AddDays(-11),
                    MaLoai = loaiDongHo.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Đồng hồ Thể thao Garmin Fenix 7 Pro Sapphire Solar",
                    ThuongHieu = "Garmin",
                    Gia = 21490000m,
                    SoLuong = 5,
                    ThoiGianBaoHanh = 24,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400",
                    MoTa = "Kính sạc năng lượng mặt trời Sapphire cao cấp, đèn pin LED tích hợp, bản đồ địa hình màu TOPO, thời lượng pin lên đến 37 ngày.",
                    NgayNhap = DateTime.Now.AddDays(-9),
                    MaLoai = loaiDongHo.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Đồng hồ thông minh Xiaomi Watch 2 Pro Khung thép",
                    ThuongHieu = "Xiaomi",
                    Gia = 4990000m,
                    SoLuong = 20,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=400",
                    MoTa = "Chạy hệ điều hành Google Wear OS mượt mà, chip Snapdragon W5+ Gen 1 tiến trình 4nm hiện đại, hơn 150 chế độ thể thao chuyên sâu.",
                    NgayNhap = DateTime.Now.AddDays(-5),
                    MaLoai = loaiDongHo.MaLoai
                },
                new SanPham
                {
                    TenSanPham = "Vòng đeo tay thông minh Huawei Band 9 Siêu nhẹ",
                    ThuongHieu = "Huawei",
                    Gia = 890000m,
                    SoLuong = 40,
                    ThoiGianBaoHanh = 12,
                    TrangThai = true,
                    HinhAnh = "https://images.unsplash.com/photo-1575311373937-040b8e1fd5b6?w=400",
                    MoTa = "Trọng lượng siêu nhẹ chỉ 14g mỏng 8.99mm, theo dõi giấc ngủ khoa học HUAWEI TruSleep 4.0, pin bền bỉ đến 14 ngày không lo hết pin.",
                    NgayNhap = DateTime.Now.AddDays(-3),
                    MaLoai = loaiDongHo.MaLoai
                }
            };

            context.SanPhams.AddRange(dsSanPham);
            context.SaveChanges();

            // 4. Tạo 1 Đơn hàng mẫu ban đầu để liên kết dữ liệu
            var userKhachHang = context.KhachHangs.FirstOrDefault(k => k.Email.Contains("nguyenvana")) ?? context.KhachHangs.FirstOrDefault();
            if (userKhachHang != null && !context.DonHangs.Any())
            {
                var donHangMau = new DonHang
                {
                    MaKhachHang = userKhachHang.MaKhachHang,
                    NgayDat = DateTime.Now.AddDays(-2),
                    TongTien = dsSanPham[0].Gia + dsSanPham[10].Gia,
                    TrangThai = "ChoXacNhan",
                    SoDienThoaiGiaoHang = userKhachHang.SoDienThoai,
                    DiaChiGiaoHang = userKhachHang.DiaChi
                };
                context.DonHangs.Add(donHangMau);
                context.SaveChanges();

                var chiTiet1 = new ChiTietDonHang
                {
                    MaDonHang = donHangMau.MaDonHang,
                    MaSanPham = dsSanPham[0].MaSanPham,
                    SoLuong = 1,
                    DonGia = dsSanPham[0].Gia
                };
                var chiTiet2 = new ChiTietDonHang
                {
                    MaDonHang = donHangMau.MaDonHang,
                    MaSanPham = dsSanPham[10].MaSanPham,
                    SoLuong = 1,
                    DonGia = dsSanPham[10].Gia
                };

                context.ChiTietDonHangs.AddRange(chiTiet1, chiTiet2);
                context.SaveChanges();
            }
        }
    }
}
