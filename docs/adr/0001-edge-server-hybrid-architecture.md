# ADR 0001: Architectural Topology — Edge-Server Hybrid Architecture

## Status
Accepted

## Context
HPParkingAPI phục vụ quản lý ra/vào và đỗ xe tại các địa điểm thực tế (công trường xây dựng, tòa nhà, bãi xe thương mại). Tại các địa điểm này:
1. Kết nối Internet/mạng LAN có thể bị gián đoạn chập chờn.
2. Việc mở barie/turnstile yêu cầu độ trễ cực thấp (< 100ms) để tránh ùn tắc làn xe.
3. Việc xử lý hình ảnh camera ANPR (biển số xe) hoặc FaceID nếu gửi toàn bộ luồng video lên server trung tâm sẽ gây nghẽn băng thông.

## Decision
Áp dụng kiến trúc **Edge-Server Hybrid** 4 tầng:
- **Tầng 1 (Peripherals)**: Camera, Cảm biến, Barie, Wiegand RFID Reader.
- **Tầng 2 (Edge Gate Station - WinForms App)**: Chạy AI ONNX local (YOLOv8 + CRNN OCR) trực tiếp trên máy trạm cổng, lưu SQLite local cache để hoạt động 100% offline khi mất mạng, điều khiển relay mở barie trực tiếp.
- **Tầng 3 (Central Server - HPParkingAPI)**: Web API ASP.NET Core + MongoDB + SignalR Hub đóng vai trò trung tâm lưu trữ, tính toán giá đỗ xe, tạo mã thanh toán VietQR và phát sự kiện realtime.
- **Tầng 4 (Web Admin Portal)**: Giao diện quản trị đa địa điểm, báo cáo doanh thu.

## Consequences
### Positive
- Barie phản hồi tức thì (< 50ms) không phụ thuộc tốc độ mạng Internet.
- Tiết kiệm 100% chi phí AI Cloud / API nhận diện biển số nhờ YOLOv8 ONNX local.
- Hệ thống vẫn cho phép xe ra/vào khi đứt kết nối với server trung tâm (nhờ SQLite cache).

### Negative
- Cần triển khai ứng dụng WinForms Trạm Cổng trên từng máy trạm.
- Cần logic đồng bộ dữ liệu (Sync Engine) từ SQLite local về MongoDB server khi mạng khôi phục.
