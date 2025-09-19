using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.GuiTinNhan
{
    [Table(nameof(ChienDich), Schema = DbSchemas.Core)]
    [Index(
        nameof(Id),
        IsUnique = false,
        Name = $"IX_{nameof(ChienDich)}"
    )]
    public class ChienDich : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdBrandName { get; set; }
        public int? IdMauNoiDung { get; set; }

        [Required]
        [MaxLength(500)]
        public string TenChienDich { get; set; } = String.Empty;

        [MaxLength(500)]
        public string MoTa { get; set; } = String.Empty;

        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        [MaxLength(4000)]
        public string? NoiDung { get; set; } = String.Empty;

        public bool IsFlashSms { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }
}
