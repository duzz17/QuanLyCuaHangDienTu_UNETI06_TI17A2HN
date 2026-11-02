import sqlite3
import random
import sys
from datetime import datetime, timedelta

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"D:\cuong6b\QuanLyCuaHangDienTu_UNETI06_TI17A2HN\Data\QuanLyCuaHangDienTu.db"
conn = sqlite3.connect(db_path)
cur = conn.cursor()

print("--- BẮT ĐẦU TẠO DỮ LIỆU ---")

# 1. TẠO 20 KHÁCH HÀNG & 20 TÀI KHOẢN (khachhang1 -> khachhang20, pass: 123456)
customers_info = [
    ("Hoàng Văn Minh", "khachhang1@gmail.com", "0901234501", "Số 12 Chùa Bộc, Đống Đa, Hà Nội", "1995-03-15"),
    ("Nguyễn Thị Lan", "khachhang2@gmail.com", "0901234502", "Tòa S2.05 Vinhomes Smart City, Nam Từ Liêm, Hà Nội", "1998-07-22"),
    ("Lê Hoàng Nam", "khachhang3@gmail.com", "0901234503", "145 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh", "1992-11-05"),
    ("Phạm Mai Anh", "khachhang4@gmail.com", "0901234504", "Số 88 Cầu Giấy, Cầu Giấy, Hà Nội", "2000-01-30"),
    ("Trần Quốc Bảo", "khachhang5@gmail.com", "0901234505", "42 Lê Lợi, Hải Châu, Đà Nẵng", "1994-09-18"),
    ("Vũ Hải Đăng", "khachhang6@gmail.com", "0901234506", "Số 250 Lĩnh Nam, Hoàng Mai, Hà Nội", "1997-04-12"),
    ("Đỗ Thu Thảo", "khachhang7@gmail.com", "0901234507", "76 Hùng Vương, Hồng Bàng, Hải Phòng", "1999-12-08"),
    ("Ngô Minh Tuấn", "khachhang8@gmail.com", "0901234508", "102 Quang Trung, Hà Đông, Hà Nội", "1993-06-25"),
    ("Bùi Thảo Linh", "khachhang9@gmail.com", "0901234509", "18 Hoàng Diệu, Ba Đình, Hà Nội", "2001-08-14"),
    ("Đặng Đức Thắng", "khachhang10@gmail.com", "0901234510", "350 Nguyễn Trãi, Thanh Xuân, Hà Nội", "1996-02-19"),
    ("Trịnh Phương Thảo", "khachhang11@gmail.com", "0901234511", "55 Trần Phú, Ngô Quyền, Hải Phòng", "1998-10-03"),
    ("Đinh Văn Khoa", "khachhang12@gmail.com", "0901234512", "89 Võ Thị Sáu, Quận 3, TP. Hồ Chí Minh", "1991-05-27"),
    ("Phan Ngọc Anh", "khachhang13@gmail.com", "0901234513", "12 Bạch Đằng, Hoàn Kiếm, Hà Nội", "2002-03-09"),
    ("Dương Thanh Tùng", "khachhang14@gmail.com", "0901234514", "234 Lê Duẩn, Thanh Khê, Đà Nẵng", "1995-12-31"),
    ("Lý Mỹ Dung", "khachhang15@gmail.com", "0901234515", "67 Hai Bà Trưng, Bến Nghé, Quận 1, TP. HCM", "1997-08-20"),
    ("Mai Tiến Đạt", "khachhang16@gmail.com", "0901234516", "Số 5 ngõ 198 Xã Đàn, Đống Đa, Hà Nội", "1994-04-05"),
    ("Đoàn Thu Trang", "khachhang17@gmail.com", "0901234517", "92 Nguyễn Khánh Toàn, Cầu Giấy, Hà Nội", "2000-06-17"),
    ("Tạ Quang Huy", "khachhang18@gmail.com", "0901234518", "15 Lý Tự Trọng, Bến Nghé, Quận 1, TP. HCM", "1993-11-23"),
    ("Cao Thị Ngọc", "khachhang19@gmail.com", "0901234519", "48 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội", "1999-01-11"),
    ("Hà Văn Thịnh", "khachhang20@gmail.com", "0901234520", "312 Trường Chinh, Đống Đa, Hà Nội", "1996-09-02")
]

created_khach_hang_ids = []

for idx, (name, email, phone, addr, dob) in enumerate(customers_info, 1):
    username = f"khachhang{idx}"
    
    # Check if TaiKhoan exists
    cur.execute("SELECT MaTaiKhoan, MaKhachHang FROM TaiKhoans WHERE TenDangNhap = ?", (username,))
    row = cur.fetchone()
    if row:
        print(f"  Tài khoản {username} đã tồn tại (MaTaiKhoan: {row[0]}).")
        created_khach_hang_ids.append(row[1])
    else:
        # Check if KhachHang exists by email
        cur.execute("SELECT MaKhachHang FROM KhachHangs WHERE Email = ?", (email,))
        kh_row = cur.fetchone()
        reg_date = (datetime.now() - timedelta(days=random.randint(30, 120))).strftime("%Y-%m-%d %H:%M:%S")
        if kh_row:
            kh_id = kh_row[0]
        else:
            cur.execute("""
                INSERT INTO KhachHangs (HoTen, Email, SoDienThoai, DiaChi, NgaySinh, NgayDangKy, HinhAnh)
                VALUES (?, ?, ?, ?, ?, ?, ?)
            """, (name, email, phone, addr, dob, reg_date, None))
            kh_id = cur.lastrowid
        
        # Insert TaiKhoan
        cur.execute("""
            INSERT INTO TaiKhoans (TenDangNhap, MatKhau, VaiTro, TrangThai, MaKhachHang)
            VALUES (?, ?, ?, ?, ?)
        """, (username, "123456", "KhachHang", 1, kh_id))
        tk_id = cur.lastrowid
        created_khach_hang_ids.append(kh_id)
        print(f"  Đã tạo {username} (Mã KH: {kh_id}, Mã TK: {tk_id})")

conn.commit()

# 2. TẠO THÊM 25 SẢN PHẨM MỚI PHONG PHÚ (Tổng cộng sẽ có 50 sản phẩm)
new_products = [
    # MaLoai: 1 - Điện thoại & Máy tính bảng
    ("iPhone 16 Pro Max 256GB Sa Mạc", "Apple", "Mỹ", "Khung titan sa mạc tuyệt đẹp, chip Apple A18 Pro 3nm vượt bậc, nút Camera Control chụp ảnh chuyên nghiệp, pin cực trâu.", "34990000.0", 25, 12, 1, "/images/iphone-16-pro-max.png", 1),
    ("Samsung Galaxy Z Fold6 5G 256GB", "Samsung", "Hàn Quốc", "Điện thoại gập cao cấp nhất, mỏng nhẹ hơn, trang bị Galaxy AI thông minh, chip Snapdragon 8 Gen 3 for Galaxy đỉnh cao.", "41990000.0", 15, 12, 1, "/images/galaxy-z-fold6.png", 1),
    ("Samsung Galaxy Z Flip6 5G 256GB", "Samsung", "Hàn Quốc", "Thiết kế gập vỏ sò thời thượng, màn hình ngoài FlexWindow đa năng, camera 50MP sắc nét, pin nâng cấp 4000mAh.", "26990000.0", 18, 12, 1, "/images/galaxy-z-flip6.png", 1),
    ("iPad Air 11 inch M2 Wi-Fi 128GB", "Apple", "Mỹ", "Trang bị chip Apple M2 siêu mạnh, màn hình Liquid Retina tuyệt đẹp, hỗ trợ Apple Pencil Pro và Magic Keyboard mới.", "16490000.0", 20, 12, 1, "/images/ipad-air-m2.png", 1),
    ("Xiaomi Pad 6S Pro 12.4 inch 8GB/256GB", "Xiaomi", "Trung Quốc", "Màn hình 3K 144Hz siêu mượt, chip Snapdragon 8 Gen 2, sạc siêu tốc 120W HyperCharge, 6 loa stereo sống động.", "14990000.0", 12, 18, 1, "/images/xiaomi-pad-6s-pro.png", 1),

    # MaLoai: 2 - Laptop & Máy tính xách tay
    ("MacBook Air 13 inch M3 16GB/256GB", "Apple", "Mỹ", "Thiết kế nhôm nguyên khối siêu mỏng 11.3mm, chip M3 mạnh mẽ, hỗ trợ xuất 2 màn hình ngoài, thời lượng pin 18 giờ.", "27990000.0", 22, 12, 1, "/images/macbook-air-m3.png", 2),
    ("Laptop Gaming Asus TUF Gaming A15 FA506NC", "Asus", "Đài Loan", "Chiến game đỉnh cao với AMD Ryzen 5 7535HS, card đồ họa rời RTX 3050 4GB, màn hình 144Hz chuẩn eSports, độ bền quân đội.", "17490000.0", 14, 24, 1, "/images/asus-tuf-a15.png", 2),
    ("Laptop Lenovo Legion 5 16IRX9 Core i7-14650HX", "Lenovo", "Trung Quốc", "Cỗ máy gaming đỉnh cao Core i7 thế hệ 14, RTX 4060 8GB GDDR6, màn hình 16 inch 2.5K 165Hz chuẩn màu 100% sRGB.", "36990000.0", 8, 36, 1, "/images/lenovo-legion-5.png", 2),
    ("Laptop Dell Inspiron 14 5440 Intel Core 5-120U", "Dell", "Mỹ", "Văn phòng mỏng nhẹ sang trọng, vi xử lý Intel Core 5 thế hệ mới, RAM 16GB đa nhiệm mượt, vỏ nhôm cao cấp chống bám vân tay.", "18990000.0", 16, 12, 1, "/images/dell-inspiron-14.png", 2),
    ("Laptop HP Pavilion 14 Core i5-1335U 16GB", "HP", "Mỹ", "Kiểu dáng thanh lịch nhỏ gọn 1.4kg, màn hình IPS viền siêu mỏng, âm thanh B&O cao cấp, sạc nhanh 50% trong 30 phút.", "15290000.0", 19, 12, 1, "/images/hp-pavilion-14.png", 2),

    # MaLoai: 3 - Tai nghe & Âm thanh
    ("Tai nghe Chụp tai Sony WH-CH720N Chống ồn", "Sony", "Nhật Bản", "Chip xử lý V1 đỉnh cao lọc tiếng ồn hiệu quả, trọng lượng nhẹ nhất chỉ 192g, thời lượng pin lên đến 35 giờ nghe nhạc liên tục.", "2490000.0", 30, 12, 1, "/images/sony-wh-ch720n.png", 3),
    ("Loa Bluetooth Marshall Emberton II Chính Hãng", "Marshall", "Anh", "Âm thanh đa hướng True Stereophonic 360 độ, chuẩn chống nước bụi IP67 bền bỉ, hơn 30 giờ chơi nhạc di động không nghỉ.", "3990000.0", 25, 12, 1, "/images/marshall-emberton-2.png", 3),
    ("Tai nghe Samsung Galaxy Buds3 Pro Chống ồn AI", "Samsung", "Hàn Quốc", "Thiết kế Blade Lights ấn tượng, âm thanh Hi-Fi 24bit sống động, công nghệ chống ồn thích ứng Blade ANC theo môi trường.", "4990000.0", 20, 12, 1, "/images/galaxy-buds3-pro.png", 3),
    ("Loa Thanh Soundbar Sony HT-S20R 5.1 Kênh 400W", "Sony", "Nhật Bản", "Hệ thống âm thanh vòm thực 5.1 kênh công suất 400W uy lực, loa sau và subwoofer riêng biệt, kết nối Bluetooth và HDMI ARC.", "5490000.0", 10, 12, 1, "/images/sony-ht-s20r.png", 3),
    ("Tai nghe Thể thao Dẫn truyền xương Shokz OpenRun Pro", "Shokz", "Mỹ", "Công nghệ truyền âm thanh qua xương Shokz TurboPitch thế hệ 9, giữ thông thoáng đôi tai an toàn khi chạy bộ, chuẩn chống mồ hôi IP55.", "3790000.0", 15, 24, 1, "/images/shokz-openrun-pro.png", 3),

    # MaLoai: 4 - Bàn phím & Chuột
    ("Chuột Không dây Logitech Lift Vertical Công thái học", "Logitech", "Thụy Sĩ", "Thiết kế góc nghiêng 57 độ giảm áp lực cổ tay, phím bấm êm Quiet Clicks, cuộn SmartWheel thông minh, pin dùng 2 năm.", "1490000.0", 35, 24, 1, "/images/logitech-lift.png", 4),
    ("Bàn phím cơ ASUS ROG Falchion RX Low Profile Wireless", "Asus", "Đài Loan", "Switch quang học ROG RX Red phản hồi siêu tốc, phím mỏng thời thượng, bảng điều khiển cảm ứng tương tác bên hông độc đáo.", "3690000.0", 12, 24, 1, "/images/rog-falchion-rx.png", 4),
    ("Chuột Gaming Siêu nhẹ Razer DeathAdder V3 Pro", "Razer", "Singapore", "Form công thái học huyền thoại eSports, trọng lượng chỉ 63g, cảm biến quang học Focus Pro 30K DPI chính xác từng milimet.", "3190000.0", 18, 24, 1, "/images/razer-deathadder-v3-pro.png", 4),
    ("Bàn phím cơ DareU EK87 Pro White Black Multi-LED", "DareU", "Trung Quốc", "Bàn phím TKL gọn gàng, Dream Switch gõ mượt mà êm ái, cấu trúc foam tiêu âm cao cấp, keycap PBT chất lượng vượt tầm giá.", "890000.0", 40, 24, 1, "/images/dareu-ek87-pro.png", 4),
    ("Chuột Gaming Không dây Logitech G502 X PLUS RGB", "Logitech", "Thụy Sĩ", "Huyền thoại tái sinh với switch lai quang-cơ LIGHTFORCE, cảm biến HERO 25K đỉnh cao, LED RGB LIGHTSYNC 8 vùng rực rỡ.", "3290000.0", 15, 24, 1, "/images/logitech-g502x-plus.png", 4),

    # MaLoai: 5 - Đồng hồ thông minh
    ("Apple Watch Series 9 GPS 45mm Viền nhôm Thể thao", "Apple", "Mỹ", "Chip S9 SiP đột phá, thao tác chạm hai lần Double Tap kỳ diệu, màn hình sáng 2000 nits, theo dõi nồng độ oxy và điện tâm đồ.", "9890000.0", 20, 12, 1, "/images/apple-watch-s9.png", 5),
    ("Samsung Galaxy Watch Ultra 47mm LTE Khung Titan", "Samsung", "Hàn Quốc", "Đồng hồ phiêu lưu cao cấp nhất, khung vỏ Titanium chuẩn quân sự, chuẩn kháng nước 10 ATM, nút Quick Button kích hoạt còi khẩn cấp.", "14990000.0", 10, 12, 1, "/images/galaxy-watch-ultra.png", 5),
    ("Đồng hồ Thể thao Garmin Forerunner 165 Music", "Garmin", "Mỹ", "Màn hình cảm ứng AMOLED rực rỡ, tải nhạc trực tiếp không cần điện thoại, kế hoạch luyện tập thích ứng Garmin Coach chuyên sâu.", "6990000.0", 14, 24, 1, "/images/garmin-forerunner-165.png", 5),
    ("Vòng đeo tay Thông minh Xiaomi Smart Band 8 Pro", "Xiaomi", "Trung Quốc", "Màn hình lớn AMOLED 1.74 inch sắc nét, tích hợp định vị GNSS độc lập chạy bộ không cần điện thoại, pin bền bỉ 14 ngày.", "1490000.0", 50, 12, 1, "/images/xiaomi-band-8-pro.png", 5),
    ("Đồng hồ Thông minh Amazfit T-Rex 2 Quân đội Siêu bền", "Amazfit", "Trung Quốc", "Vượt qua 15 bài kiểm tra độ bền chuẩn quân sự, định vị 5 hệ thống vệ tinh băng tần kép, pin cực khủng 24 ngày sử dụng.", "3890000.0", 16, 12, 1, "/images/amazfit-t-rex-2.png", 5)
]

for p in new_products:
    cur.execute("SELECT MaSanPham FROM SanPhams WHERE TenSanPham = ?", (p[0],))
    if not cur.fetchone():
        now_str = (datetime.now() - timedelta(days=random.randint(2, 20))).strftime("%Y-%m-%d %H:%M:%S")
        cur.execute("""
            INSERT INTO SanPhams (TenSanPham, ThuongHieu, XuatXu, MoTa, Gia, SoLuong, ThoiGianBaoHanh, TrangThai, HinhAnh, NgayNhap, MaLoai)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7], p[8], now_str, p[9]))
        print(f"  Đã thêm sản phẩm: {p[0]}")

conn.commit()

# Lấy danh sách tất cả sản phẩm hiện có
cur.execute("SELECT MaSanPham, TenSanPham, Gia FROM SanPhams")
all_products = cur.fetchall()
print(f"\nTổng số sản phẩm trong hệ thống: {len(all_products)}")

# Lấy danh sách khách hàng vừa tạo (MaKhachHang, HoTen, SoDienThoai, DiaChi)
cur.execute("SELECT MaKhachHang, HoTen, SoDienThoai, DiaChi FROM KhachHangs WHERE Email LIKE 'khachhang%@gmail.com'")
customer_records = cur.fetchall()
print(f"Tổng số khách hàng khachhang1-20: {len(customer_records)}")

# 3. TẠO 35 ĐƠN HÀNG MỚI ĐA DẠNG TRẠNG THÁI & NGÀY ĐẶT
statuses = ["ChoXacNhan", "DaXacNhan", "DangGiao", "DaGiao", "DaHuy"]
status_weights = [0.20, 0.20, 0.25, 0.30, 0.05] # Phân bổ thực tế: 30% Đã giao, 25% Đang giao, 20% Chờ XN, 20% Đã XN, 5% Hủy

# Tạo các mốc thời gian trải đều từ 50 ngày trước đến hôm nay
# để phục vụ thống kê Dashboard 7 ngày, 30 ngày, 90 ngày
now = datetime.now()

# Xóa đơn hàng cũ nếu cần hoặc kiểm tra số đơn hiện có
cur.execute("SELECT count(*) FROM DonHangs")
current_orders_count = cur.fetchone()[0]
print(f"Số đơn hàng hiện có: {current_orders_count}")

num_new_orders = 35
print(f"Đang tạo thêm {num_new_orders} đơn hàng mới...")

for i in range(num_new_orders):
    # Chọn ngẫu nhiên khách hàng từ khachhang1..20
    cust = random.choice(customer_records)
    kh_id, kh_name, kh_phone, kh_addr = cust
    
    # Chọn trạng thái theo trọng số
    st = random.choices(statuses, weights=status_weights)[0]
    
    # Ngày đặt: 8 đơn trong 7 ngày gần nhất, 18 đơn trong 8-30 ngày, 9 đơn trong 31-60 ngày
    if i < 10:
        days_ago = random.randint(0, 6)
    elif i < 25:
        days_ago = random.randint(7, 29)
    else:
        days_ago = random.randint(30, 55)
    
    order_date = (now - timedelta(days=days_ago, hours=random.randint(1, 15), minutes=random.randint(0, 59))).strftime("%Y-%m-%d %H:%M:%S")
    
    # Chọn 1 đến 3 sản phẩm ngẫu nhiên
    num_items = random.choices([1, 2, 3], weights=[0.55, 0.35, 0.10])[0]
    chosen_products = random.sample(all_products, num_items)
    
    # Tính tổng tiền và chuẩn bị chi tiết
    order_items = []
    total_amount = 0.0
    for prod in chosen_products:
        p_id, p_name, p_price_str = prod
        price = float(p_price_str)
        qty = random.choices([1, 2], weights=[0.85, 0.15])[0]
        total_amount += price * qty
        order_items.append((qty, price, p_id))
    
    # Insert DonHang
    cur.execute("""
        INSERT INTO DonHangs (NgayDat, TongTien, TrangThai, DiaChiGiaoHang, SoDienThoaiGiaoHang, MaKhachHang)
        VALUES (?, ?, ?, ?, ?, ?)
    """, (order_date, total_amount, st, kh_addr, kh_phone, kh_id))
    order_id = cur.lastrowid
    
    # Insert ChiTietDonHang
    for qty, price, p_id in order_items:
        cur.execute("""
            INSERT INTO ChiTietDonHangs (SoLuong, DonGia, MaDonHang, MaSanPham)
            VALUES (?, ?, ?, ?)
        """, (qty, price, order_id, p_id))

conn.commit()

# Kiểm tra tổng kết sau khi tạo
print("\n--- HOÀN TẤT! KIỂM TRA TỔNG KẾT ---")
cur.execute("SELECT count(*) FROM TaiKhoans")
print(f"Tổng số tài khoản: {cur.fetchone()[0]}")
cur.execute("SELECT count(*) FROM KhachHangs")
print(f"Tổng số khách hàng: {cur.fetchone()[0]}")
cur.execute("SELECT count(*) FROM SanPhams")
print(f"Tổng số sản phẩm: {cur.fetchone()[0]}")
cur.execute("SELECT count(*) FROM DonHangs")
print(f"Tổng số đơn hàng: {cur.fetchone()[0]}")
cur.execute("SELECT count(*) FROM ChiTietDonHangs")
print(f"Tổng số chi tiết đơn hàng: {cur.fetchone()[0]}")

print("\nThống kê đơn hàng theo trạng thái:")
cur.execute("SELECT TrangThai, count(*), sum(TongTien) FROM DonHangs GROUP BY TrangThai")
for r in cur.fetchall():
    print(f"  - {r[0]}: {r[1]} đơn, tổng tiền: {r[2]:,.0f} ₫")

conn.close()
