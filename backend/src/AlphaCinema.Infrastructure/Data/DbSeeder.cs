using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AlphaCinemaDbContext context)
    {
        // 1. Đảm bảo Database đã được tạo
        await context.Database.EnsureCreatedAsync();

        // 2. Kiểm tra xem đã có dữ liệu phim hoàn thiện nhất chưa
        var isWickedFixed = await context.Phims.AnyAsync(p => p.TenPhim != null && p.TenPhim.Contains("Wicked") && p.Poster != null && p.Poster.Contains("j5AVKd"));
        
        if (!isWickedFixed)
        {
            try
            {
                // 3. Tìm đường dẫn file SQL (đã được copy vào thư mục API)
                var sqlFilePath = Path.Combine(AppContext.BaseDirectory, "Full_System_Seed.sql");
                
                // Nếu không tìm thấy ở bin, thử tìm ở source (trong trường hợp chạy dotnet run)
                if (!File.Exists(sqlFilePath))
                {
                    sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Full_System_Seed.sql");
                }

                if (File.Exists(sqlFilePath))
                {
                    // Đọc file với Encoding.UTF8 để tránh lỗi font chữ tiếng Việt
                    var sql = await File.ReadAllTextAsync(sqlFilePath, Encoding.UTF8);
                    
                    // Tách các lệnh GO để thực thi từng phần
                    // Dùng cách thủ công để đảm bảo không bị tách nhầm từ "ngon" trong tiếng Việt
                    var sqlCommands = sql.Split(new[] { "\r\nGO\r\n", "\r\ngo\r\n", "\nGO\n", "\ngo\n", "\rGO\r", "\rgo\r" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var command in sqlCommands)
                    {
                        if (!string.IsNullOrWhiteSpace(command))
                        {
                            var cleanCommand = command.Replace("USE AlphaCinema;", "").Replace("use AlphaCinema;", "");
                            await context.Database.ExecuteSqlRawAsync(cleanCommand);
                        }
                    }
                    Console.WriteLine("[Seeder] Database structure and seed data applied successfully.");
                }
                else
                {
                    Console.WriteLine("[Warning] Seed SQL file not found at: " + sqlFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] Seeding process failed: " + ex.Message);
            }
        }

        // 4. Đảm bảo mật khẩu tất cả tài khoản seed là 'admin123' với Hash BCrypt mới nhất
        var emails = new[] { "admin@alpha.com", "staff@alpha.com", "customer@alpha.com" };
        foreach (var email in emails)
        {
            var user = await context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.MatKhau = BCrypt.Net.BCrypt.HashPassword("admin123");
                
                // Thêm 1000 điểm cho tài khoản admin để test
                if (email == "admin@alpha.com")
                {
                    user.DiemTichLuy += 1000;
                }
            }
        }
        await context.SaveChangesAsync();
        Console.WriteLine("[System] Accounts verified: admin, staff, customer / admin123");
    }
}
