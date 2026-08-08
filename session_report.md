# Báo Cáo Phiên Làm Việc — Dự Án HPParkingAPI

**Ngày lập báo cáo**: 07/08/2026  
**Phạm vi**: Tư vấn kiến trúc giải pháp & Phát triển tầng mô hình dữ liệu (Domain Modeling Layer)  
**Dự án**: HPParkingAPI — Hệ thống Kiểm soát An ninh Người & Phương tiện + Quản lý Bãi giữ xe Thông minh  
**Nền tảng công nghệ**: ASP.NET Core 8.0 · MongoDB · C# .NET 10.0

---

## I. Tổng Quan Dự Án

**HPParkingAPI** là giải pháp phần mềm tập trung cho các nhà máy, khu công nghiệp, cơ quan và bãi giữ xe thương mại, cung cấp đồng thời hai chức năng chính:

1. **Kiểm soát An ninh Người ra vào**: Xác thực công nhân, kỹ sư, khách tham quan qua thẻ RFID, nhận diện CCCD gắn chip, và nhận diện khuôn mặt FaceID.
2. **Quản lý Bãi Giữ Xe Thu Phí**: Nhận diện biển số xe tự động (ANPR/LPR), lưu vé ra/vào, tính phí linh hoạt (giờ/ngày/tháng) và tích hợp thanh toán VietQR không tiếp xúc.

---

## II. Kiến Trúc Giải Pháp Được Xác Lập

Sau quá trình phân tích nghiệp vụ thực tế, kiến trúc được thống nhất theo mô hình **Edge-Server Hybrid**, gồm 4 tầng vận hành độc lập:

```
[ Tầng 1: Thiết Bị Ngoại Vi ]
  Camera ANPR (LAN/RTSP) · Camera FaceID · Đầu đọc RFID (Wiegand)
  Đầu đọc CCCD (USB Direct) · Cảm biến Rada · Access Controller (LAN)

         ↓ (RTSP · Wiegand · USB HID · TCP/IP SDK)

[ Tầng 2: Trạm Máy Tính Bảo Vệ Tại Cổng — WinForms App (C# .NET) ]
  • AI ONNX Local (YOLOv8-Nano + CRNN OCR) — Miễn phí 0đ, 100% Offline
  • Local SQLite Cache — Hoạt động tiếp tục khi đứt mạng
  • Gate Controller Logic — Mở Barrier/Turnstile < 50ms

         ↕ (REST API + SignalR WebSocket)

[ Tầng 3: Backend Server Trung Tâm — HPParkingAPI ]
  ASP.NET Core 8.0 · MongoDB · SignalR Hub (Realtime < 50ms)
  Services: Parking · Pricing · Access · Payment · Device

         ↕ (REST API HTTPS)

[ Tầng 4: Web Admin Portal ]
  Quản lý đa công trường · Báo cáo doanh thu · Đăng ký vé tháng · Cấu hình thiết bị
```

### Các Điểm Kỹ Thuật Nổi Bật
- **Nhận diện biển số xe (ANPR) Zero-Cost**: Dùng YOLOv8-Nano + CRNN OCR chạy trực tiếp trong WinForms qua `Microsoft.ML.OnnxRuntime` — **Không phát sinh chi phí bản quyền**, hoạt động **100% offline**.
- **Chống mất mạng (Offline Fallback)**: Máy trạm cổng lưu cache danh sách công nhân/xe và vé đỗ tạm vào SQLite local, tự đồng bộ ngược về server khi có kết nối trở lại.
- **Kiến trúc mở rộng đa cổng/đa công trường**: Thiết kế `Site → Gate → Device` phân cấp rõ ràng, cho phép mở rộng tuyến tính khi bổ sung thêm bãi xe hoặc cổng kiểm soát mới.

---

## III. Kết Quả Công Việc Thực Hiện

### A. Tài Liệu Thiết Kế & Kiến Trúc

| Tài liệu | Đường dẫn | Nội dung |
| :--- | :--- | :--- |
| Giải pháp Kiến trúc | [solution_architecture.md](file:///C:/Users/ADMIN/.gemini/antigravity-ide/brain/1fb9a00c-b3cd-4c34-a3c9-1016b069b6ad/solution_architecture.md) | Sơ đồ kiến trúc tổng thể, luồng nghiệp vụ Mermaid, bảng công nghệ, Gantt Roadmap |
| Sơ đồ Hệ thống Draw.io | [architecture.drawio](file:///d:/ASP.NET/HPParkingAPI/architecture.drawio) | Sơ đồ đồ họa 4 tầng hệ thống, có thể mở và chỉnh sửa trực tiếp bằng Draw.io |

---

### B. Phân Hệ Domain Model Entities (Tầng Dữ Liệu)

Toàn bộ Entity kế thừa từ `BaseEntity` (Id ObjectId MongoDB, CreatedAt, UpdatedAt).

#### 🟢 Đã có từ trước & được Nâng cấp

| File | Thay đổi Chính |
| :--- | :--- |
| [ParkingTicket.cs](file:///d:/ASP.NET/HPParkingAPI/Models/Entities/Parking/ParkingTicket.cs) | + `VehicleCategory`, `TicketType` (Vãng lai/Vé tháng), `MonthlySubscriptionId`, `InImageUrl`, `OutImageUrl` (ảnh ANPR vào/ra) |
| [PricingPolicy.cs](file:///d:/ASP.NET/HPParkingAPI/Models/Entities/Parking/PricingPolicy.cs) | + `ApplicableCategory` (áp giá theo loại xe), `FreeGraceMinutes` (N phút miễn phí đầu), `OvernightPrice` (phụ thu qua đêm) |
| [PaymentTransaction.cs](file:///d:/ASP.NET/HPParkingAPI/Models/Entities/Parking/PaymentTransaction.cs) | + `PaymentStatus` (Pending/Success/Failed/Refunded), `TransactionCode` (mã VietQR/ngân hàng), `QrPayload` (chuỗi mã QR động) |
| [Vehicle.cs](file:///d:/ASP.NET/HPParkingAPI/Models/Entities/Vehicles/Vehicle.cs) | + `IsVIP`, `IsBlacklisted`, `BlacklistReason`, `MonthlyExpiryDate` (hạn vé tháng) |
| [Worker.cs](file:///d:/ASP.NET/HPParkingAPI/Models/Entities/Personnel/Worker.cs) | + `Department` (phòng ban/đội thi công), `IsBlacklisted`, `BlacklistReason` |
| [VehicleAccessLog.cs](file:///d:/ASP.NET/HPParkingAPI/Models/Entities/AccessLogs/VehicleAccessLog.cs) | + `Category`, `FullImageUrl` (ảnh toàn cảnh), `PlateCropImageUrl` (ảnh crop biển số), `ConfidenceScore` (độ tin cậy AI) |
| [PersonAccessLog.cs](file:///d:/ASP.NET/HPParkingAPI/Models/Entities/AccessLogs/PersonAccessLog.cs) | + `AuthMethod.CitizenId` (CCCD), `IdentityNumber`, `SnapshotUrl` (ảnh FaceID), `FaceMatchScore` |

#### 🔵 Mới Hoàn Toàn

| File | Chức Năng |
| :--- | :--- |
| [Device.cs](file:///d:/ASP.NET/HPParkingAPI/Models/Entities/Devices/Device.cs) | Quản lý chi tiết thiết bị ngoại vi phần cứng: `DeviceType` (7 loại), `ConnectionMode` (LAN IP / USB Direct / Wiegand), IP/RTSP, `ParentControllerId` + `WiegandFormat` (cho RFID Wiegand), `ComPort`/`UsbDevicePath` (cho CCCD USB), `Status` (Online/Offline/Error), `LastPingAt` |

---

### C. Phân Hệ DTOs (Tầng Giao Tiếp API)

> Folder `Models/DTOs/` đã được xây dựng hoàn toàn từ trống.

| File | Chức năng |
| :--- | :--- |
| [CheckInRequestDto.cs](file:///d:/ASP.NET/HPParkingAPI/Models/DTOs/Parking/CheckInRequestDto.cs) | WinForms → API: Gửi thông tin xe vào (SiteId, GateId, LicensePlate, CardNumber, ảnh ANPR, ConfidenceScore) |
| [CheckOutRequestDto.cs](file:///d:/ASP.NET/HPParkingAPI/Models/DTOs/Parking/CheckOutRequestDto.cs) | WinForms → API: Gửi thông tin xe ra (SiteId, OutGateId, LicensePlate, ảnh ANPR ra) |
| [CheckOutResponseDto.cs](file:///d:/ASP.NET/HPParkingAPI/Models/DTOs/Parking/CheckOutResponseDto.cs) | API → WinForms: Trả kết quả khớp vé (TotalMinutes, Amount, VietQrUrl, ảnh vào/ra để bảo vệ đối chiếu) |
| [ParkingTicketDto.cs](file:///d:/ASP.NET/HPParkingAPI/Models/DTOs/Parking/ParkingTicketDto.cs) | DTO đọc danh sách vé đỗ xe cho Web Admin Dashboard |
| [PricingPolicyDtos.cs](file:///d:/ASP.NET/HPParkingAPI/Models/DTOs/Parking/PricingPolicyDtos.cs) | Create/Response DTOs cấu hình bảng giá linh hoạt |
| [PaymentDtos.cs](file:///d:/ASP.NET/HPParkingAPI/Models/DTOs/Payment/PaymentDtos.cs) | DTOs giao dịch thanh toán + `VietQrGenerateDto` sinh mã QR động |
| [PersonnelDtos.cs](file:///d:/ASP.NET/HPParkingAPI/Models/DTOs/Personnel/PersonnelDtos.cs) | DTOs quản lý Worker (công nhân) và Contractor (nhà thầu) |
| [GateAccessEventDto.cs](file:///d:/ASP.NET/HPParkingAPI/Models/DTOs/Realtime/GateAccessEventDto.cs) | DTO sự kiện SignalR Hub đẩy xuống WinForms và Admin Dashboard theo thời gian thực |
| [DeviceDtos.cs](file:///d:/ASP.NET/HPParkingAPI/Models/DTOs/Devices/DeviceDtos.cs) | Create/Update/Response DTOs quản lý thiết bị + `DeviceHeartbeatDto` nhận tín hiệu ping trạng thái |

---

### D. Kiểm Tra Biên Dịch (Build Verification)

Sau mỗi lần thay đổi mã nguồn, lệnh `dotnet build` đã được thực thi tự động để xác minh tính toàn vẹn:

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## IV. Lộ Trình Tiếp Theo (Next Steps)

| Giai đoạn | Nội dung | Trạng thái |
| :--- | :--- | :--- |
| **Phase 1: Domain Models** | Entities, DTOs, Enums | ✅ **Hoàn thành** |
| **Phase 2: Services Layer** | `IParkingTicketService`, `IPricingService`, `IAccessLogService`, `IPaymentService`, `IDeviceService` | ⏳ Tiếp theo |
| **Phase 3: Controllers & API** | REST Endpoints cho từng Service, Swagger documentation | ⏳ Chờ Phase 2 |
| **Phase 4: SignalR Hub** | Realtime event push lên WinForms & Web Admin | ⏳ Chờ Phase 3 |
| **Phase 5: WinForms App** | Gate Station App (Camera RTSP, ONNX AI, COM/USB SDK, SQLite Fallback) | ⏳ Song song Phase 3-4 |
| **Phase 6: Web Admin Portal** | Dashboard, Báo cáo, Quản lý thiết bị & nhân sự | ⏳ Chờ Phase 3 |
