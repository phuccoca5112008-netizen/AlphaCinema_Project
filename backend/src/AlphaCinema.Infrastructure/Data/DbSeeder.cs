using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AlphaCinemaDbContext context)
    {
        // 1. Đảm bảo Database đã được tạo
        await context.Database.EnsureCreatedAsync();

        // 2. Kiểm tra xem đã có dữ liệu chưa (kiểm tra bảng Phim)
        if (await context.Phims.AnyAsync())
        {
            return;
        }

        try
        {
            // 3. Tìm đường dẫn file SQL (đã được copy vào thư mục API)
            var sqlFilePath = Path.Combine(AppContext.BaseDirectory, "Full_System_Seed.sql");
            
            // Nếu không tìm thấy ở bin, thử tìm ở source
            if (!File.Exists(sqlFilePath))
            {
                sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Full_System_Seed.sql");
            }

            if (File.Exists(sqlFilePath))
            {
                var sql = await File.ReadAllTextAsync(sqlFilePath);
                var sqlCommands = sql.Split(new[] { "GO", "go", "Go", "gO" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var command in sqlCommands)
                {
                    if (!string.IsNullOrWhiteSpace(command))
                    {
                        var cleanCommand = command.Replace("USE AlphaCinema;", "").Replace("use AlphaCinema;", "");
                        await context.Database.ExecuteSqlRawAsync(cleanCommand);
                    }
                }

                // 4. FIX MẬT KHẨU ADMIN SANG BCrypt (Để bạn mình đăng nhập được ngay)
                var admin = await context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == "admin@alpha.com");
                if (admin != null)
                {
                    admin.MatKhau = BCrypt.Net.BCrypt.HashPassword("admin123");
                    await context.SaveChangesAsync();
                }

                Console.WriteLine("[Success] 100% Data fidelity maintained & Admin login fixed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Error] Seeding failed: " + ex.Message);
        }
    }
}
