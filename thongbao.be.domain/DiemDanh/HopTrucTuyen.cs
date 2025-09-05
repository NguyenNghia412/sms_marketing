using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.DiemDanh
{
    [Table(nameof(HopTrucTuyen), Schema = DbSchemas.Core)]
    [Index(
        nameof(Id),
        IsUnique = false,
        Name = $"IX_{nameof(HopTrucTuyen)}"
    )]
public  class HopTrucTuyen : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string TenCuocHop { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime?ThoiGianKetThuc { get; set; }
        public DateTime? ThoiGianDiemDanh{ get; set; }
        public string IdCuocHop { get; set; } = String.Empty;
        public string? LinkCuocHop { get; set; } = String.Empty;
        public string IdTinNhanChung { get; set; } = String.Empty;
        public string? UserIdCreated { get; set; } = String.Empty;
        public int? ThoiHanDiemDanh { get; set; }
        public DateTime? ThoiGianTaoCuocHop { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }

    }
}
