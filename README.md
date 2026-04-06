# 🎬 Alpha Cinema - Premium Movie Booking System

Hệ thống quản lý đặt vé xem phim hiện đại được xây dựng với **ASP.NET Core (Backend)** và **Vue.js 3 (Frontend)**.

## 🚀 Hướng dẫn cài đặt nhanh (Quick Start)

Dự án đã được cấu hình để tự động thiết lập Database và dữ liệu mẫu ngay trong lần chạy đầu tiên.

### 1. Yêu cầu hệ thống
- SQL Server (Local)
- .NET 9 SDK
- Node.js (phiên bản mới nhất)

### 2. Thiết lập Database
Mở tệp `backend/src/AlphaCinema.API/appsettings.json` và kiểm tra chuỗi kết nối:
```json
"DefaultConnection": "Server=.;Database=AlphaCinema;Trusted_Connection=True;..."
```
*(Mặc định là `Server=.`. Nếu máy bạn dùng tên instance khác, hãy điều chỉnh lại tại đây)*.

### 3. Khởi chạy Backend
```bash
cd backend/src/AlphaCinema.API
# Chỉ cần chạy lệnh này, ứng dụng sẽ tự tạo DB và nạp dữ liệu mẫu
dotnet run
```

### 4. Khởi chạy Frontend
```bash
cd frontend
npm install
npm run dev
```

## ✨ Tính năng chính
- 🎟 **Đặt vé trực tuyến**: Chọn phim, chọn suất chiếu và chọn ghế thời gian thực.
- 🍿 **Combo đồ ăn**: Tích hợp chọn bắp nước kèm theo đơn hàng.
- 💳 **Thanh toán**: Mô phỏng quy trình thanh toán và tạo hóa đơn.
- 📱 **QR Code**: Tự động tạo mã QR cho mỗi vé để check-in.
- 📊 **Admin Dashboard**: Quản lý doanh thu, suất chiếu và phòng chiếu chuyên nghiệp.

---
*Phát triển bởi Alpha Cinema Team 2026*
