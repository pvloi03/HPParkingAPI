# ADR 0002: Persistence Mechanism — MongoDB with Generic Repository Pattern

## Status
Accepted

## Context
Dữ liệu của hệ thống đỗ xe và kiểm soát ra vào có các đặc thù:
1. Số lượng bản ghi nhật ký (PersonAccessLog, VehicleAccessLog, ParkingTicket) tăng trưởng rất nhanh theo thời gian.
2. Các cấu hình thiết bị (Device) và bảng giá (PricingPolicy) có thuộc tính linh hoạt tùy theo từng công trường/bãi xe.
3. Cần khả năng mở rộng hàng triệu lượt đỗ xe mà không làm giảm tốc độ ghi.

## Decision
Sử dụng **MongoDB** làm cơ sở dữ liệu chính kết hợp với **Generic Repository Pattern (`IRepository<T>`)**:
- Entity sử dụng `BsonId` (string ObjectId) và kế thừa từ `BaseEntity`.
- `MongoRepository<T>` cung cấp các hàm async (`FindOneAsync`, `FindAsync`, `ExistsAsync`, `InsertAsync`, `UpdateAsync`, `DeleteAsync`) nhận LINQ Expression để truy vấn trực tiếp trên MongoDB Collection.

## Consequences
### Positive
- Tốc độ Ghi (Write throughput) của MongoDB cực cao cho dữ liệu log và ticket.
- Schema linh hoạt (Schema-less), dễ mở rộng thuộc tính thiết bị ngoại vi và cấu hình bảng giá.
- Thao tác dữ liệu qua LINQ Expression giúp code sạch và dễ viết Unit Test.

### Negative
- Không hỗ trợ ACID Transactions phức tạp như RDBMS (SQL Server / PostgreSQL) trừ khi sử dụng Replica Set.
- Cần chú ý đánh Index (Compound Index) trên MongoDB cho các trường thường xuyên query như `SiteId`, `LicensePlate`, `InTime`, `IsActive`.
