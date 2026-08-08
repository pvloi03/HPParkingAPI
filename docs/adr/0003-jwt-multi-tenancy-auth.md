# ADR 0003: Authentication & Multi-Tenancy — JWT Bearer with SiteId Claims

## Status
Accepted

## Context
Hệ thống HPParkingAPI phục vụ đồng thời nhiều Site (công trường/bãi xe) khác nhau:
- **SuperAdmin**: Cần quản lý tất cả các Site.
- **SiteAdmin**: Chỉ được phép xem và quản lý trong phạm vi Site được phân công.
- **Operator**: Nhân viên vận hành trạm cổng tại 1 Site.

## Decision
Sử dụng **JWT Bearer Token** tích hợp claim `SiteId` và `ClaimTypes.Role`:
- Token chứa thông tin `sub` (UserId), `name` (FullName), `ClaimTypes.Role` (SuperAdmin | SiteAdmin | Operator | Viewer), và `siteId` (ID của địa điểm, hoặc `null` đối với SuperAdmin).
- Tích hợp Middleware kiểm tra Authorization Header và Swagger UI `SecurityRequirement` với biểu tượng 🔒 để tiện lợi cho việc debug và kiểm thử API.

## Consequences
### Positive
- Stateless Authentication: Server không cần duy trì session trong memory, dễ dàng scale horizontal.
- Phân quyền đa địa điểm (Multi-tenancy) rõ ràng và minh bạch thông qua claim `siteId`.

### Negative
- Token đã cấp không thể thu hồi lập tức trước thời hạn hết hạn ngoại trừ việc vô hiệu hóa tài khoản (`IsActive = false`) hoặc kiểm tra token blacklist.
