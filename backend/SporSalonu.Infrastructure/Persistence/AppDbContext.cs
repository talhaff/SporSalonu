using Microsoft.EntityFrameworkCore;
using SporSalonu.Domain.Entities;
using SporSalonu.Domain.Enums;

namespace SporSalonu.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<MembershipPackage> MembershipPackages => Set<MembershipPackage>();
    public DbSet<MembershipSubscription> MembershipSubscriptions => Set<MembershipSubscription>();
    public DbSet<TransactionLog> TransactionLogs => Set<TransactionLog>();
    public DbSet<CheckInLog> CheckInLogs => Set<CheckInLog>();
    public DbSet<FreezeLog> FreezeLogs => Set<FreezeLog>();
    public DbSet<SystemNotification> SystemNotifications => Set<SystemNotification>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Member ────────────────────────────────────────────
        modelBuilder.Entity<Member>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Ad).HasMaxLength(100).IsRequired();
            e.Property(m => m.Soyad).HasMaxLength(100).IsRequired();
            e.Property(m => m.Telefon).HasMaxLength(20).IsRequired();
            e.HasIndex(m => m.Telefon).IsUnique();
            e.Property(m => m.Email).HasMaxLength(150);
        });

        // ── MembershipPackage ──────────────────────────────────
        modelBuilder.Entity<MembershipPackage>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Ad).HasMaxLength(100).IsRequired();
            e.Property(p => p.Fiyat).HasPrecision(18, 2);
        });

        // ── MembershipSubscription ─────────────────────────────
        modelBuilder.Entity<MembershipSubscription>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.ToplamTutar).HasPrecision(18, 2);
            e.Property(s => s.IndirimTutari).HasPrecision(18, 2);
            e.Property(s => s.NetTutar).HasPrecision(18, 2);
            e.Property(s => s.KalanBakiye).HasPrecision(18, 2);
            e.Property(s => s.Durum).HasConversion<string>();

            e.HasOne(s => s.Member)
             .WithMany(m => m.Subscriptions)
             .HasForeignKey(s => s.MemberId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(s => s.Package)
             .WithMany(p => p.Subscriptions)
             .HasForeignKey(s => s.MembershipPackageId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── TransactionLog ─────────────────────────────────────
        modelBuilder.Entity<TransactionLog>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Tutar).HasPrecision(18, 2);
            e.Property(t => t.Tip).HasConversion<string>();
            e.Property(t => t.Aciklama).HasMaxLength(500);

            e.HasOne(t => t.Member)
             .WithMany(m => m.TransactionLogs)
             .HasForeignKey(t => t.MemberId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(t => t.Subscription)
             .WithMany(s => s.TransactionLogs)
             .HasForeignKey(t => t.SubscriptionId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── CheckInLog ─────────────────────────────────────────
        modelBuilder.Entity<CheckInLog>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.RedSebebi).HasMaxLength(300);

            e.HasOne(c => c.Member)
             .WithMany(m => m.CheckInLogs)
             .HasForeignKey(c => c.MemberId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── FreezeLog ──────────────────────────────────────────
        modelBuilder.Entity<FreezeLog>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.Sebep).HasMaxLength(300);

            e.HasOne(f => f.Subscription)
             .WithMany(s => s.FreezeLogs)
             .HasForeignKey(f => f.SubscriptionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── SystemNotification ─────────────────────────────────
        modelBuilder.Entity<SystemNotification>(e =>
        {
            e.HasKey(n => n.Id);
            e.Property(n => n.Mesaj).HasMaxLength(500).IsRequired();
            e.Property(n => n.Tip).HasConversion<string>();

            e.HasOne(n => n.Member)
             .WithMany()
             .HasForeignKey(n => n.MemberId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── AppUser ────────────────────────────────────────────
        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.KullaniciAdi).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Rol).HasMaxLength(50);
            e.HasIndex(u => u.KullaniciAdi).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();

            // Seed: İlk admin kullanıcısı
            e.HasData(new AppUser
            {
                Id = 1,
                KullaniciAdi = "admin",
                Email = "admin@sporsalonu.com",
                // BCrypt hash of "Admin1234!" — Infrastructure'da hashlenecek
                PasswordHash = "$2a$11$rBhEVJbLqjDU4pBZg8LdWOh.fqGEH4xR/LkQKLxVp9a4Mfz4e1IK2",
                Rol = "Admin",
                IsActive = true,
                OlusturmaTarihi = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        });

        // ── Seed: Üyelik Paketleri ─────────────────────────────
        modelBuilder.Entity<MembershipPackage>().HasData(
            new MembershipPackage { Id = 1, Ad = "1 Aylık Üyelik",  AySayisi = 1,  GunSayisi = 0, Fiyat = 500m,  IsActive = true, OlusturmaTarihi = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new MembershipPackage { Id = 2, Ad = "3 Aylık Üyelik",  AySayisi = 3,  GunSayisi = 0, Fiyat = 1350m, IsActive = true, OlusturmaTarihi = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new MembershipPackage { Id = 3, Ad = "6 Aylık Üyelik",  AySayisi = 6,  GunSayisi = 0, Fiyat = 2400m, IsActive = true, OlusturmaTarihi = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new MembershipPackage { Id = 4, Ad = "12 Aylık Üyelik", AySayisi = 12, GunSayisi = 0, Fiyat = 4200m, IsActive = true, OlusturmaTarihi = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new MembershipPackage { Id = 5, Ad = "Günlük Giriş",     AySayisi = 0,  GunSayisi = 1, Fiyat = 100m,  IsActive = true, OlusturmaTarihi = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new MembershipPackage { Id = 6, Ad = "Haftalık Üyelik",   AySayisi = 0,  GunSayisi = 7, Fiyat = 400m,  IsActive = true, OlusturmaTarihi = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
