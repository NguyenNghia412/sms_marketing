using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.domain.GuiTinNhan;
using thongbao.be.shared.Constants.Db;

namespace thongbao.be.infrastructure.data
{
    public class SmDbContext : DbContext
    {
        public SmDbContext() {}

        public SmDbContext(
            DbContextOptions<SmDbContext> options
        ) : base(options) { }

        public DbSet<ChienDich> ChienDiches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChienDich>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
                entity.Property(e => e.NgayBatDau).HasDefaultValueSql("getdate()");
            });

            modelBuilder.HasDefaultSchema(DbSchemas.Core);

            base.OnModelCreating(modelBuilder);
        }
    }
}
