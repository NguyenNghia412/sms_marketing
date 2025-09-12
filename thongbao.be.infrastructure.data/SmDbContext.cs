using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.domain.Auth;
using thongbao.be.domain.DanhBa;
using thongbao.be.domain.DiemDanh;
using thongbao.be.domain.GuiTinNhan;
using thongbao.be.shared.Constants.Db;

namespace thongbao.be.infrastructure.data
{
    public class SmDbContext : IdentityDbContext<AppUser>
    {
        public SmDbContext(DbContextOptions<SmDbContext> options) : base(options)
        {
        }

        public DbSet<ChienDich> ChienDiches { get; set; }
        public DbSet<HopTrucTuyen> HopTrucTuyens { get; set; }
        public DbSet<ThongTinDiemDanh> ThongTinDiemDanhs { get; set; }
        public DbSet<TinNhanHopTrucTuyen> TinNhanHopTrucTuyens { get; set; }
        public DbSet<DotDiemDanh> DotDiemDanhs { get; set; }

        public DbSet<GhiNhanDiemDanh> GhiNhanDiemDanhs { get; set; }
        public DbSet<DanhBaCungChiTiet> DanhBaCungChiTiets { get; set; }

        public DbSet<DanhBa> DanhBas { get; set; }
        public DbSet<DanhBaTruongData> DanhBaTruongDatas { get; set; }
        public DbSet<DanhBaChiTiet> DanhBaChiTiets { get; set; }
        public DbSet<DanhBaData> DanhBaDatas { get; set; }
        public DbSet<ChienDichDanhBa> ChienDichDanhBa { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseOpenIddict();

            modelBuilder.Entity<ChienDich>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
                entity.Property(e => e.NgayBatDau).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<HopTrucTuyen>(entity =>
            {
               entity.Property(e => e.Deleted).HasDefaultValue(0);
               entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
               entity.Property(e => e.ThoiGianBatDau).HasDefaultValueSql("getdate()");
               entity.Property(e => e.BatDauDiemDanh).HasDefaultValueSql("getdate()");
               entity.Property(e => e.KetThucDiemDanh).HasDefaultValueSql("getdate()");
               entity.Property(e => e.ThoiGianKetThuc).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<ThongTinDiemDanh>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
               
            });
            modelBuilder.Entity<TinNhanHopTrucTuyen>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");

            });
            modelBuilder.Entity<DotDiemDanh>(entity =>
            {
                entity.Property(e => e.ThoiGianBatDau).HasDefaultValueSql("getdate()");
                entity.Property(e => e.ThoiGianKetThuc).HasDefaultValueSql("getdate()");
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");

            });
            modelBuilder.Entity<GhiNhanDiemDanh>(entity =>
            {
                entity.Property(e => e.ThoiGianDiemDanh).HasDefaultValueSql("getdate()");
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<DanhBaCungChiTiet>(entity =>
            {
                entity.Property(e => e.DiaChi).HasMaxLength(1500);
                entity.Property(e => e.NgaySinh).HasDefaultValueSql("getdate()");
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<DanhBa>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<DanhBaTruongData>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<DanhBaChiTiet>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<DanhBaData>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<ChienDichDanhBa>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });

            modelBuilder.HasDefaultSchema(DbSchemas.Core);

            base.OnModelCreating(modelBuilder);
        }
    }
}
