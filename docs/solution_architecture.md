# HPParkingAPI — Solution Architecture Document

## 1. Physical Architecture Topology (Mô hình Kiến trúc Vật lý 4 Tầng)

```mermaid
graph TD
    subgraph Layer1["Tầng 1: Thiết Bị Ngoại Vi (Hardware Devices)"]
        CamANPR["Camera ANPR (LAN/RTSP)"]
        CamFace["Camera FaceID (LAN/RTSP)"]
        RFIDReader["Đầu đọc RFID (Wiegand/RS485)"]
        CCCDReader["Đầu đọc CCCD gắn chip (USB HID)"]
        BarieController["Controller Barie / Turnstile (Relay)"]
    end

    subgraph Layer2["Tầng 2: Trạm Máy Tính Cổng (WinForms App - Edge)"]
        ONNXEngine["AI Local ONNX Engine (YOLOv8 + CRNN OCR)"]
        SQLiteCache["Local SQLite Cache (Offline Fallback)"]
        GateLogic["Gate Controller Logic (Mở Barie < 50ms)"]
        SyncEngine["Background Sync Engine"]
    end

    subgraph Layer3["Tầng 3: Backend Server Trung Tâm (ASP.NET Core API)"]
        AuthModule["Auth & Security Module (JWT + Role)"]
        ParkingService["Parking Ticket Service"]
        PricingService["Pricing Engine Service"]
        PaymentService["VietQR Payment Service"]
        SignalRHub["SignalR Realtime Hub (< 50ms)"]
        MongoDB[(MongoDB Central Database)]
    end

    subgraph Layer4["Tầng 4: Web Admin Portal"]
        AdminDashboard["Dashboard & Analytics"]
        SiteMgmt["Multi-Site Management"]
        ReportService["Doanh thu & Báo cáo Ra/Vào"]
    end

    CamANPR -->|RTSP Stream| ONNXEngine
    CamFace -->|RTSP Stream| ONNXEngine
    RFIDReader -->|Wiegand Protocol| GateLogic
    CCCDReader -->|USB Serial/HID| GateLogic
    GateLogic -->|Relay Command| BarieController

    ONNXEngine --> GateLogic
    GateLogic <--> SQLiteCache
    GateLogic <--> SyncEngine

    SyncEngine <-->|REST API HTTPS| CentralAPI[HPParkingAPI Controller]
    SignalRHub <-->|WebSocket Realtime| GateLogic
    SignalRHub <-->|WebSocket Realtime| AdminDashboard

    CentralAPI --> AuthModule
    CentralAPI --> ParkingService
    CentralAPI --> PricingService
    CentralAPI --> PaymentService
    ParkingService <--> MongoDB
    AuthModule <--> MongoDB
    AdminDashboard <-->|REST API HTTPS| CentralAPI
```

---

## 2. Dynamic Sequence Flows (Sơ đồ Tiến trình Ra / Vào)

### 2.1. Luồng Xe Vào (Check-In Sequence)

```mermaid
sequenceDiagram
    autonumber
    actor Driver as Lái xe / Cư dân
    participant Cam as Camera ANPR
    participant Station as WinForms Trạm Cổng
    participant AI as Local ONNX AI
    participant Relay as Barie Relay
    participant API as HPParkingAPI (Server)
    participant Hub as SignalR Hub

    Driver->>Cam: Xe tiến vào làn
    Cam->>Station: Gửi luồng ảnh RTSP
    Station->>AI: Nhận diện biển số (YOLOv8 + CRNN)
    AI-->>Station: Trả kết quả: Biển số "30F-123.45" (Độ tin cậy 98%)
    Driver->>Station: Quét thẻ RFID
    Station->>API: Gọi POST /api/v1/parking/check-in
    API->>API: Kiểm tra vé tháng / Tạo vé vãng lai mới
    API-->>Station: Trả về: Success (TicketId, CardNumber)
    Station->>Relay: Gửi lệnh mở Barie (< 50ms)
    Station->>Hub: Broadcast GateAccessEvent (Realtime)
    Hub-->>WebAdmin: Hiển thị xe vào trên Dashboard
```

### 2.2. Luồng Xe Ra & Tính Phí (Check-Out Sequence)

```mermaid
sequenceDiagram
    autonumber
    actor Driver as Lái xe
    participant Station as WinForms Trạm Cổng
    participant API as HPParkingAPI (Server)
    participant VietQR as VietQR API Service

    Driver->>Station: Quét thẻ RFID / Nhận diện biển số ra
    Station->>API: Gọi POST /api/v1/parking/check-out
    API->>API: Tra cứu Ticket vào, Tính tiền theo PricingPolicy
    API->>VietQR: Sinh mã VietQR chuyển khoản động
    API-->>Station: Trả về CheckOutResponseDto (Số tiền, VietQR Image Payload, Ảnh vào/ra)
    Station-->>Driver: Hiển thị màn hình phụ: Số tiền + Mã VietQR
    Driver->>API: Thanh toán thành công (Webhook / Xác nhận thủ công)
    API-->>Station: Xác nhận thanh toán đủ
    Station->>Station: Gửi lệnh mở Barie xe ra
```

---

## 3. Project Class Architecture & Seam Placement

```mermaid
classDiagram
    class BaseEntity {
        +string Id
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    class AppUser {
        +string Username
        +string PasswordHash
        +UserRole Role
        +string SiteId
        +bool IsActive
    }

    class Person {
        +string FullName
        +string IdentityNumber
        +string CardNumber
        +string ContractorId
    }

    class ParkingTicket {
        +string SiteId
        +string LicensePlate
        +VehicleCategory Category
        +TicketType Type
        +DateTime InTime
        +DateTime OutTime
        +decimal Amount
        +PaymentStatus Status
    }

    class IRepository~T~ {
        <<interface>>
        +GetAllAsync() Task~List~T~~
        +GetByIdAsync(id) Task~T~
        +FindOneAsync(predicate) Task~T~
        +FindAsync(predicate) Task~List~T~~
        +ExistsAsync(predicate) Task~bool~
        +InsertAsync(entity) Task
        +UpdateAsync(entity) Task
        +DeleteAsync(id) Task~bool~
    }

    class MongoRepository~T~ {
        -IMongoCollection~T~ _collection
    }

    class IAuthService {
        <<interface>>
        +LoginAsync(dto) Task~LoginResponseDto~
        +CreateUserAsync(dto) Task~UserResponseDto~
        +ChangePasswordAsync(id, current, new) Task~bool~
        +DeactivateUserAsync(id) Task~bool~
        +ReactivateUserAsync(id) Task~bool~
    }

    class AuthService {
        -IRepository~AppUser~ _userRepo
        -IConfiguration _config
    }

    BaseEntity <|-- AppUser
    BaseEntity <|-- Person
    BaseEntity <|-- ParkingTicket
    IRepository <|.. MongoRepository
    IAuthService <|.. AuthService
    AuthService --> IRepository
```
