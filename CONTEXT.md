# HPParkingAPI — Domain Context & Ubiquitous Language

## Overview
HPParkingAPI là hệ thống tích hợp đa phân hệ cho 2 mục đích cốt lõi:
1. **Kiểm soát An ninh Nhân sự (Personnel Access Control)**: Giám sát công nhân, nhân viên, kỹ sư, khách tham quan ra/vào công trường, tòa nhà, khu chế xuất.
2. **Quản lý Bãi đỗ xe Thông minh (Smart Parking System)**: Nhận diện biển số tự động (ANPR), tính phí đỗ xe theo bảng giá linh hoạt, thu phí qua VietQR và vé tháng.

---

## Glossary (Từ vựng Domain Chuẩn)

### Multi-Tenancy & Phân cấp Địa điểm
- **Site (Địa điểm / Công trường / Bãi xe)**: Đơn vị quản lý độc lập cao nhất (ví dụ: Công trường A, Tòa nhà B, Bãi xe C).
- **Gate (Cổng kiểm soát)**: Điểm vào/ra cụ thể gắn với 1 Site (ví dụ: Cổng 1 - Làn xe máy, Cổng chính - Làn công nhân).
- **Gate Station (Trạm máy tính tại Cổng)**: Máy tính chạy ứng dụng trạm (WinForms) đặt tại Cổng để kết nối thiết bị ngoại vi và điều khiển barie.

### Phân hệ Nhân sự & An ninh (Personnel Access Control)
- **Person (Người / Nhân sự)**: Thực thể người được đăng ký trong hệ thống (bao gồm nhân viên chính thức, công nhân nhà thầu, kỹ sư, khách).
- **Contractor (Nhà thầu)**: Đơn vị chủ quản của công nhân/kỹ sư ngoài.
- **Access Card (Thẻ ra vào)**: Thẻ RFID (Mifare/Proximity) được cấp phát cho một Person hoặc Vehicle.
- **Identity Number (Mã định danh)**: Số CCCD (gắn chip) hoặc Mã công nhân.
- **PersonAccessLog (Lịch sử ra vào người)**: Bản ghi sự kiện quét thẻ / xác thực FaceID / quét CCCD tại cổng.

### Phân hệ Bãi xe (Smart Parking)
- **Vehicle (Phương tiện)**: Xe máy, ô tô, xe tải, xe đạp điện ra vào bãi.
- **VehicleCategory (Loại phương tiện)**: Phân loại xe (Motorbike, Car, Truck, ElectricBicycle) để áp dụng giá đỗ và phân làn.
- **ParkingTicket (Vé đỗ xe)**: Bản ghi lượt gửi xe (bao gồm giờ vào, giờ ra, ảnh ANPR vào/ra, mã thẻ, số tiền phải trả, trạng thái thanh toán).
- **TicketType (Loại vé)**: Vé vãng lai (Casual) hoặc Vé tháng (Monthly).
- **MonthlySubscription (Vé tháng / Đăng ký tháng)**: Đăng ký gửi xe theo chu kỳ (tháng/quý) của một Person cho một Vehicle cụ thể.
- **PricingPolicy (Bảng giá đỗ xe)**: Quy tắc tính tiền dựa trên loại xe, thời gian gửi (block giờ, ngày/đêm, miễn phí n phút đầu).
- **PaymentTransaction (Giao dịch thanh toán)**: Bản ghi thanh toán tiền vé đỗ xe (VietQR, Tiền mặt).

### Phân hệ Thiết bị Ngoại vi (Peripheral Devices)
- **Device (Thiết bị phần cứng)**: Thiết bị ngoại vi kết nối với Trạm cổng (Camera ANPR, Camera FaceID, Đầu đọc RFID Wiegand, Đầu đọc CCCD USB, Rada cảm biến, Controller điều khiển Barie).
- **DeviceType**: Phân loại thiết bị phần cứng.
- **ConnectionMode**: Phương thức kết nối (LAN IP, USB Direct, Wiegand RS485/TCP).

### Phân hệ Quản trị & Xác thực (Auth & Identity)
- **AppUser (Tài khoản người dùng)**: Người vận hành hoặc quản trị viên đăng nhập vào Web Admin hoặc WinForms App.
- **UserRole (Vai trò người dùng)**:
  - `SuperAdmin`: Quản trị toàn hệ thống (mọi Site).
  - `SiteAdmin`: Quản lý riêng 1 Site cụ thể.
  - `Operator`: Nhân viên trực trạm cổng (thu phí, mở barie).
  - `Viewer`: Người xem báo cáo, dashboard.

---

## Context Boundaries (Các Phân hệ Nghiệp vụ)

```
┌────────────────────────────────────────────────────────────────────────┐
│                          SYSTEM BOUNDED CONTEXT                        │
├───────────────────┬────────────────────┬───────────────────────────────┤
│    Identity &     │  Personnel Access  │        Smart Parking          │
│    Auth Context   │      Context       │           Context             │
│                   │                    │                               │
│ • AppUser         │ • Person           │ • ParkingTicket               │
│ • UserRole        │ • Contractor       │ • MonthlySubscription         │
│ • JWT Tokens      │ • AccessCard       │ • PersonAccessLog             │
│ • Multi-Site Auth │ • PersonAccessLog  │ • PricingPolicy               │
│                   │ • FaceID / CCCD    │ • PaymentTransaction          │
│                   │                    │ • Vehicle & ANPR Log          │
├───────────────────┴────────────────────┴───────────────────────────────┤
│                   Hardware & Device Integration Context                │
│ • Site & Gate Topology                                                 │
│ • Device Status & Heartbeat Ping                                       │
│ • Realtime Gate Access Event (SignalR)                                 │
└────────────────────────────────────────────────────────────────────────┘
```
