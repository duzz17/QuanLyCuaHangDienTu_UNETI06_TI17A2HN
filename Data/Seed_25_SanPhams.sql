-- ==============================================================================
-- DỰ ÁN: CỬA HÀNG ĐIỆN TỬ - UNETI NHÓM 06
-- HỌ VÀ TÊN: PHẠM VĂN CƯỜNG - MSSV: 23103100094
-- MODULE 2: M2-13 CHUẨN BỊ 25 SẢN PHẨM MẪU TRẢI ĐỀU 5 LOẠI SẢN PHẨM
-- ==============================================================================

USE [QuanLyCuaHangDienTu];
GO

-- 1. Đảm bảo có đủ 5 loại sản phẩm nếu chưa có
IF NOT EXISTS (SELECT 1 FROM LoaiSanPhams WHERE MaLoai = 1)
    INSERT INTO LoaiSanPhams (TenLoai, MoTa) VALUES (N'Điện thoại & Máy tính bảng', N'Điện thoại thông minh, iPad, tablet');
IF NOT EXISTS (SELECT 1 FROM LoaiSanPhams WHERE MaLoai = 2)
    INSERT INTO LoaiSanPhams (TenLoai, MoTa) VALUES (N'Laptop & Máy tính xách tay', N'Laptop văn phòng, đồ họa, gaming');
IF NOT EXISTS (SELECT 1 FROM LoaiSanPhams WHERE MaLoai = 3)
    INSERT INTO LoaiSanPhams (TenLoai, MoTa) VALUES (N'Tai nghe & Âm thanh', N'Tai nghe không dây, tai nghe chụp tai, loa');
IF NOT EXISTS (SELECT 1 FROM LoaiSanPhams WHERE MaLoai = 4)
    INSERT INTO LoaiSanPhams (TenLoai, MoTa) VALUES (N'Bàn phím & Chuột', N'Bàn phím cơ, chuột công thái học, chuột gaming');
IF NOT EXISTS (SELECT 1 FROM LoaiSanPhams WHERE MaLoai = 5)
    INSERT INTO LoaiSanPhams (TenLoai, MoTa) VALUES (N'Đồng hồ thông minh', N'Smartwatch, Smartband thể thao');
GO

-- 2. Xóa các sản phẩm cũ nếu muốn làm mới (chỉ xóa khi chưa có chi tiết đơn hàng khóa ngoại hoặc an toàn)
-- Nếu đã có sản phẩm thì không chèn trùng tên
SET NOCOUNT ON;

-- ===== LOẠI 1: ĐIỆN THOẠI & MÁY TÍNH BẢNG (5 SP) =====
IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'iPhone 15 Pro Max 256GB Titan Tự Nhiên')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'iPhone 15 Pro Max 256GB Titan Tự Nhiên', N'Apple', 32990000, 15, 12, 1, 
        N'https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=400', 
        N'Khung viền Titan siêu nhẹ, chip Apple A17 Pro mạnh mẽ nhất, camera zoom 5x sắc nét, cổng USB-C tốc độ cao.', 
        DATEADD(day, -20, GETDATE()), 1);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Samsung Galaxy S24 Ultra 5G 512GB')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Samsung Galaxy S24 Ultra 5G 512GB', N'Samsung', 29990000, 20, 12, 1, 
        N'https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=400', 
        N'Tích hợp quyền năng Galaxy AI thông minh, bút S-Pen quyền năng, màn hình Dynamic AMOLED 2X 120Hz.', 
        DATEADD(day, -18, GETDATE()), 1);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Xiaomi 14 Ultra 5G 16GB/512GB')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Xiaomi 14 Ultra 5G 16GB/512GB', N'Xiaomi', 24490000, 10, 18, 1, 
        N'https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=400', 
        N'Hệ thống 4 camera quang học Leica đỉnh cao cảm biến 1-inch, Snapdragon 8 Gen 3 siêu tốc, sạc HyperCharge 90W.', 
        DATEADD(day, -15, GETDATE()), 1);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'iPad Pro 11 inch M4 Wi-Fi 256GB')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'iPad Pro 11 inch M4 Wi-Fi 256GB', N'Apple', 27990000, 8, 12, 1, 
        N'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400', 
        N'Thiết kế mỏng kỷ lục 5.3mm, chip Apple M4 thế hệ mới, màn hình Ultra Retina XDR Tandem OLED tuyệt mỹ.', 
        DATEADD(day, -12, GETDATE()), 1);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Samsung Galaxy Tab S9 Ultra 5G')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Samsung Galaxy Tab S9 Ultra 5G', N'Samsung', 22990000, 5, 12, 1, 
        N'https://images.unsplash.com/photo-1561154464-82e9adf32764?w=400', 
        N'Màn hình cực đại 14.6 inch sắc nét, chuẩn kháng nước kháng bụi IP68, xử lý đa nhiệm xuất sắc kèm bút S-Pen.', 
        DATEADD(day, -10, GETDATE()), 1);

-- ===== LOẠI 2: LAPTOP & MÁY TÍNH XÁCH TAY (5 SP) =====
IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'MacBook Pro 14 inch M3 Pro 18GB/512GB')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'MacBook Pro 14 inch M3 Pro 18GB/512GB', N'Apple', 49990000, 12, 12, 1, 
        N'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400', 
        N'Chip Apple M3 Pro 11-core CPU, 14-core GPU, màn hình Liquid Retina XDR 120Hz ProMotion, pin đến 18 giờ.', 
        DATEADD(day, -25, GETDATE()), 2);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Laptop Dell XPS 15 9530 Core i7-13700H')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Laptop Dell XPS 15 9530 Core i7-13700H', N'Dell', 45500000, 8, 24, 1, 
        N'https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=400', 
        N'Intel Core i7-13700H thế hệ 13, RAM 16GB DDR5, SSD 512GB, card rời NVIDIA RTX 4050 6GB GDDR6.', 
        DATEADD(day, -22, GETDATE()), 2);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Laptop Asus ROG Zephyrus G16 OLED Gaming')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Laptop Asus ROG Zephyrus G16 OLED Gaming', N'Asus', 52990000, 6, 24, 1, 
        N'https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=400', 
        N'Laptop Gaming mỏng nhẹ cao cấp, màn hình ROG Nebula OLED 2.5K 240Hz, Intel Core Ultra 9, RTX 4070.', 
        DATEADD(day, -19, GETDATE()), 2);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Lenovo ThinkPad X1 Carbon Gen 11 Core i7')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Lenovo ThinkPad X1 Carbon Gen 11 Core i7', N'Lenovo', 38900000, 10, 36, 1, 
        N'https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=400', 
        N'Vỏ sợi carbon siêu bền chuẩn quân đội, bàn phím gõ êm nhất thế giới, trọng lượng chỉ 1.12kg.', 
        DATEADD(day, -14, GETDATE()), 2);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Laptop Acer Swift Go 14 AI OLED Intel Core Ultra 5')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Laptop Acer Swift Go 14 AI OLED Intel Core Ultra 5', N'Acer', 18990000, 15, 12, 1, 
        N'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400', 
        N'Màn hình OLED 2.8K 90Hz rực rỡ, tích hợp AI Intel NPU, thiết kế vỏ nhôm mỏng nhẹ, pin dùng cả ngày.', 
        DATEADD(day, -8, GETDATE()), 2);

-- ===== LOẠI 3: TAI NGHE & ÂM THANH (5 SP) =====
IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Tai nghe Chống ồn Sony WH-1000XM5 Hi-Res')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Tai nghe Chống ồn Sony WH-1000XM5 Hi-Res', N'Sony', 7490000, 25, 12, 1, 
        N'https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=400', 
        N'Công nghệ chống ồn chủ động kép 8 micro lọc âm, chất âm chuẩn Hi-Res Audio, thời lượng pin 30 giờ.', 
        DATEADD(day, -16, GETDATE()), 3);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Tai nghe Apple AirPods Pro 2 USB-C MagSafe')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Tai nghe Apple AirPods Pro 2 USB-C MagSafe', N'Apple', 5690000, 30, 12, 1, 
        N'https://images.unsplash.com/photo-1600294037681-c80b4cb5b434?w=400', 
        N'Chip Apple H2 nâng cao khả năng chống ồn gấp 2 lần, âm thanh thích ứng Adaptive Audio, chuẩn kháng nước IP54.', 
        DATEADD(day, -13, GETDATE()), 3);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Loa Bluetooth Marshall Stanmore III Chính Hãng')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Loa Bluetooth Marshall Stanmore III Chính Hãng', N'Marshall', 9290000, 10, 12, 1, 
        N'https://images.unsplash.com/photo-1545454675-3531b543be5d?w=400', 
        N'Âm trường rộng đặc trưng phong cách Rock Marshall, núm vặn analog mạ vàng cổ điển, hỗ trợ Bluetooth 5.2.', 
        DATEADD(day, -11, GETDATE()), 3);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Loa Di Động Chống Nước JBL Charge 5')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Loa Di Động Chống Nước JBL Charge 5', N'JBL', 3490000, 20, 12, 1, 
        N'https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=400', 
        N'Chất âm sống động JBL Original Pro Sound, bass uy lực, chống nước bụi IP67, pin trâu 20 giờ có sạc dự phòng.', 
        DATEADD(day, -9, GETDATE()), 3);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Tai nghe Gaming Không dây Logitech G Pro X 2 LIGHTSPEED')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Tai nghe Gaming Không dây Logitech G Pro X 2 LIGHTSPEED', N'Logitech', 5490000, 12, 24, 1, 
        N'https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=400', 
        N'Màng loa Graphene 50mm cách mạng, định vị âm thanh không gian chuẩn xác, kết nối LIGHTSPEED siêu nhanh.', 
        DATEADD(day, -7, GETDATE()), 3);

-- ===== LOẠI 4: BÀN PHÍM & CHUỘT (5 SP) =====
IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Bàn phím cơ không dây Logitech MX Keys S')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Bàn phím cơ không dây Logitech MX Keys S', N'Logitech', 2890000, 30, 24, 1, 
        N'https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=400', 
        N'Phím bấm thiết kế lõm công thái học gõ cực êm, đèn nền cảm biến thông minh, kết nối mượt 3 thiết bị.', 
        DATEADD(day, -21, GETDATE()), 4);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Chuột công thái học cao cấp Logitech MX Master 3S')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Chuột công thái học cao cấp Logitech MX Master 3S', N'Logitech', 2290000, 35, 24, 1, 
        N'https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?w=400', 
        N'Cuộn bánh xe từ tính MagSpeed 1000 dòng/giây, nút bấm Quiet Clicks êm ái, cảm biến 8000 DPI lướt mượt trên kính.', 
        DATEADD(day, -17, GETDATE()), 4);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Bàn phím cơ Custom Akko 5075B Plus v2 Multi-Modes')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Bàn phím cơ Custom Akko 5075B Plus v2 Multi-Modes', N'Akko', 1950000, 18, 12, 1, 
        N'https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?w=400', 
        N'Cấu trúc Gasket Mount êm ái đàn hồi, mạch Hotswap 5 pin thay switch nhanh chóng, đèn LED RGB từng phím kèm dải viền.', 
        DATEADD(day, -15, GETDATE()), 4);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Chuột Gaming Không dây Siêu nhẹ Razer Viper V2 Pro')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Chuột Gaming Không dây Siêu nhẹ Razer Viper V2 Pro', N'Razer', 3190000, 14, 24, 1, 
        N'https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=400', 
        N'Trọng lượng siêu nhẹ chỉ 58g tối ưu cho eSports, cảm biến quang học Focus Pro 30K, switch quang Gen 3 cực bền.', 
        DATEADD(day, -12, GETDATE()), 4);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Bàn phím cơ Gaming Corsair K70 RGB PRO Cherry MX Red')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Bàn phím cơ Gaming Corsair K70 RGB PRO Cherry MX Red', N'Corsair', 3790000, 7, 24, 1, 
        N'https://images.unsplash.com/photo-1595225476474-87563907a212?w=400', 
        N'Khung nhôm phay xước bền bỉ, công nghệ xử lý siêu tốc AXON 8000Hz, keycap PBT double-shot chống mờ phím.', 
        DATEADD(day, -6, GETDATE()), 4);

-- ===== LOẠI 5: ĐỒNG HỒ THÔNG MINH (5 SP) =====
IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Apple Watch Ultra 2 GPS + Cellular 49mm Titan')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Apple Watch Ultra 2 GPS + Cellular 49mm Titan', N'Apple', 20990000, 8, 12, 1, 
        N'https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?w=400', 
        N'Vỏ Titan chuẩn quân sự chống va đập, màn hình sáng kỷ lục 3000 nits, định vị GPS tần số kép L1/L5 cực chuẩn.', 
        DATEADD(day, -14, GETDATE()), 5);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Samsung Galaxy Watch 6 Classic 47mm LTE')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Samsung Galaxy Watch 6 Classic 47mm LTE', N'Samsung', 7990000, 15, 12, 1, 
        N'https://images.unsplash.com/photo-1579586337278-3befd40fd17a?w=400', 
        N'Vòng xoay bezel xoay cơ học độc đáo, kính Sapphire chống trầy, đo điện tâm đồ ECG, đo thành phần cơ thể BIA.', 
        DATEADD(day, -11, GETDATE()), 5);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Đồng hồ Thể thao Garmin Fenix 7 Pro Sapphire Solar')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Đồng hồ Thể thao Garmin Fenix 7 Pro Sapphire Solar', N'Garmin', 21490000, 5, 24, 1, 
        N'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400', 
        N'Kính sạc năng lượng mặt trời Sapphire cao cấp, đèn pin LED tích hợp, bản đồ địa hình TOPO, pin đến 37 ngày.', 
        DATEADD(day, -9, GETDATE()), 5);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Đồng hồ thông minh Xiaomi Watch 2 Pro Khung thép')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Đồng hồ thông minh Xiaomi Watch 2 Pro Khung thép', N'Xiaomi', 4990000, 20, 12, 1, 
        N'https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=400', 
        N'Chạy hệ điều hành Google Wear OS mượt mà, chip Snapdragon W5+ Gen 1 4nm hiện đại, hơn 150 chế độ thể thao.', 
        DATEADD(day, -5, GETDATE()), 5);

IF NOT EXISTS (SELECT 1 FROM SanPhams WHERE TenSanPham = N'Vòng đeo tay thông minh Huawei Band 9 Siêu nhẹ')
INSERT INTO SanPhams (TenSanPham, ThuongHieu, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, MoTa, NgayNhap, MaLoai)
VALUES (N'Vòng đeo tay thông minh Huawei Band 9 Siêu nhẹ', N'Huawei', 890000, 40, 12, 1, 
        N'https://images.unsplash.com/photo-1575311373937-040b8e1fd5b6?w=400', 
        N'Trọng lượng siêu nhẹ chỉ 14g mỏng 8.99mm, theo dõi giấc ngủ khoa học HUAWEI TruSleep 4.0, pin bền bỉ đến 14 ngày.', 
        DATEADD(day, -3, GETDATE()), 5);
GO

-- 3. Kiểm tra số lượng sản phẩm sau khi chèn
SELECT 
    l.TenLoai, 
    COUNT(s.MaSanPham) AS SoLuongSanPham
FROM LoaiSanPhams l
LEFT JOIN SanPhams s ON l.MaLoai = s.MaLoai
GROUP BY l.MaLoai, l.TenLoai;

SELECT COUNT(*) AS TongSoSanPhamTrongCuaHang FROM SanPhams;
GO
