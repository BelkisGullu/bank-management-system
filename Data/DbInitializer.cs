using BankManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Veritabanının var olduğunu kontrol et
            try
            {
                if (!context.Database.CanConnect())
                {
                    return; // Veritabanı yok, migration çalıştırılmalı
                }
            }
            catch
            {
                return; // Veritabanına bağlanılamıyor
            }

            // Şubeler zaten var mı kontrol et
            if (context.Branches.Any())
            {
                return; // Veritabanı zaten seed edilmiş
            }

            // Örnek Şubeler
            var branches = new Branch[]
            {
                new Branch
                {
                    Name = "Batıkent Şubesi",
                    Address = "Ankara, Batıkent",
                    Phone = "0312 123 45 67"
                },
                new Branch
                {
                    Name = "Çankaya Şubesi",
                    Address = "Ankara, Çankaya",
                    Phone = "0312 234 56 78"
                },
                new Branch
                {
                    Name = "Altındağ Şubesi",
                    Address = "Ankara, Altındağ",
                    Phone = "0312 345 67 89"
                },
                new Branch
                {
                    Name = "Mamak Şubesi",
                    Address = "Ankara, Mamak",
                    Phone = "0312 456 78 90"
                }
            };

            foreach (var branch in branches)
            {
                context.Branches.Add(branch);
            }
            context.SaveChanges();
        }
    }
}

