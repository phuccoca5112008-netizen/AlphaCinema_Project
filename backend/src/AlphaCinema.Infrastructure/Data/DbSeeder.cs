using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AlphaCinemaDbContext context)
    {
        // 1. Đảm bảo Database đã được tạo
        await context.Database.EnsureCreatedAsync();

        // 2. Kiểm tra xem đã có dữ liệu chưa (kiểm tra tài khoản Admin)
        var hasAdmin = await context.NguoiDungs.AnyAsync(u => u.Email == "admin@alpha.com");
        
        if (!hasAdmin)
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
                    var sqlCommands = sql.Split(new[] { "GO", "go", "Go", "gO" }, StringSplitOptions.RemoveEmptyEntries);

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

        // 4. LUÔN LUÔN đảm bảo mật khẩu Admin là 'admin123' với Hash BCrypt mới nhất
        // Điều này khắc phục lỗi 'không đăng nhập được' khi copy giữa các máy khác nhau
        var admin = await context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == "admin@alpha.com");
        if (admin != null)
        {
            admin.MatKhau = BCrypt.Net.BCrypt.HashPassword("admin123");
            await context.SaveChangesAsync();
            Console.WriteLine("[System] Admin verified: admin@alpha.com / admin123");
        }
    }
}
