# Báo Cáo Cấu Trúc & Kiến Trúc Dự Án — HPParkingAPI

**Ngày cập nhật**: 08/08/2026  
**Phạm vi**: Thiết kế lại cấu trúc, logic nghiệp vụ, tài liệu kiến trúc & các sơ đồ hệ thống  
**Dự án**: HPParkingAPI — Hệ thống Kiểm soát An ninh Người & Phương tiện + Quản lý Bãi giữ xe Thông minh  
**Nền tảng công nghệ**: ASP.NET Core 8.0 / .NET 10.0 · MongoDB · JWT Auth · SignalR Realtime

---

## I. Áp Dụng Hệ Thống Skills Trong Dự Án

Dự án đã thiết lập và tuân thủ các quy tắc agent trong file [`AGENTS.md`](file:///d:/ASP.NET/HPParkingAPI/.agents/AGENTS.md), áp dụng các skill tiêu chuẩn:

| Skill đã áp dụng | Mục đích & Kết quả |
| :--- | :--- |
| [`code-review`](file:///d:/ASP.NET/HPParkingAPI/.agents/skills/code-review/SKILL.md) | Phân tích 2 trục (Standards & Spec), phát hiện 5 lỗi nghiêm trọng (N+1 query full RAM, lỗ hổng ChangePassword) và khắc phục 100%. |
| [`codebase-design`](file:///d:/ASP.NET/HPParkingAPI/.agents/skills/codebase-design/SKILL.md) | Thiết kế Deep Module cho Repository layer (`IRepository<T>` với LINQ Expressions), tối ưu Seam & Adapter. |
| [`domain-modeling`](file:///d:/ASP.NET/HPParkingAPI/.agents/skills/domain-modeling/SKILL.md) | Xây dựng glossary chuẩn [`CONTEXT.md`](file:///d:/ASP.NET/HPParkingAPI/CONTEXT.md) và hệ thống quyết định kiến trúc [`docs/adr/`](file:///d:/ASP.NET/HPParkingAPI/docs/adr/). |
| [`writing-for-agents`](file:///d:/ASP.NET/HPParkingAPI/.agents/skills/writing-for-agents/SKILL.md) | Thiết lập file cấu hình quy tắc dự án [`AGENTS.md`](file:///d:/ASP.NET/HPParkingAPI/.agents/AGENTS.md) hỗ trợ đa ngôn ngữ và ánh xạ skill. |

---

## II. Các File Mô Tả & Quyết Định Kiến Trúc Mới

### 1. File Từ Vựng & Phân Hệ Bounded Context
- **[`CONTEXT.md`](file:///d:/ASP.NET/HPParkingAPI/CONTEXT.md)**: Định nghĩa từ vựng chuẩn (Ubiquitous Language) cho Multi-Tenancy (`Site`, `Gate`, `Gate Station`), Nhân sự (`Person`, `AccessCard`, `Contractor`), Bãi xe (`ParkingTicket`, `MonthlySubscription`, `PricingPolicy`), Thiết bị (`Device`) và Phân quyền (`UserRole`).

### 2. Bộ Quyết Định Kiến Trúc (Architecture Decision Records - ADRs)
- **[`ADR 0001`](file:///d:/ASP.NET/HPParkingAPI/docs/adr/0001-edge-server-hybrid-architecture.md)**: Mô hình Edge-Server Hybrid 4 tầng (Barie phản hồi < 50ms, AI ONNX Local 0đ, SQLite Offline Fallback).
- **[`ADR 0002`](file:///d:/ASP.NET/HPParkingAPI/docs/adr/0002-mongodb-repository-pattern.md)**: Lưu trữ MongoDB với Generic Repository (`IRepository<T>`) hỗ trợ LINQ Expressions.
- **[`ADR 0003`](file:///d:/ASP.NET/HPParkingAPI/docs/adr/0003-jwt-multi-tenancy-auth.md)**: Xác thực JWT Bearer phân quyền theo `SiteId` và `ClaimTypes.Role`.

### 3. File Tài Liệu Kiến Trúc & Sơ Đồ Hệ Thống
- **[`docs/solution_architecture.md`](file:///d:/ASP.NET/HPParkingAPI/docs/solution_architecture.md)**: Chứa các sơ đồ Mermaid động:
  - Sơ đồ vật lý 4 tầng (Peripherals → WinForms Edge → ASP.NET Core Central API → Web Admin).
  - Sequence diagram luồng Xe vào (Check-In) & Xe ra (Check-Out) kèm mã VietQR động.
  - Class Diagram kiến trúc layers & seam placement.

---

## III. Cải Tiến Cấu Trúc Logic Codebase

1. **Repository Layer (`IRepository<T>` & `MongoRepository<T>`)**:
   - Thêm các phương thức async: `FindOneAsync`, `FindAsync`, `ExistsAsync`.
   - Giúp các service thực hiện query trực tiếp tại CSDL MongoDB thay vì kéo toàn bộ dữ liệu về RAM.

2. **Auth Service & Controller Layer**:
   - Tối ưu hóa các thao tác Login, CreateUser, SeedAdmin dùng Expression filter.
   - Thêm endpoint `POST /api/v1/auth/users/{id}/reactivate` kích hoạt lại tài khoản.
   - Sửa lỗi bảo mật trong `ChangePassword` (chỉ chính chủ hoặc SuperAdmin mới được đổi mật khẩu).
   - Thêm `[ProducesResponseType]` đầy đủ cho Swagger UI.

---

## IV. Kiểm Tra Biên Dịch (Verification)

Thực thi lệnh `dotnet build` kiểm tra toàn bộ giải pháp:
- **Result**: `Build succeeded (0 Warning(s), 0 Error(s))`.
