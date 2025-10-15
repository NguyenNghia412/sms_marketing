using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.DiemDanh
{
    [Table(nameof(GhiNhanDiemDanh), Schema = DbSchemas.Core)]
    [Index(
    nameof(Id),
    IsUnique = false,
    Name = $"IX_{nameof(GhiNhanDiemDanh)}"
)]
    public class GhiNhanDiemDanh : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdDotDiemDanh { get; set; }
        public string EmailHuce { get; set; } = String.Empty;
        public DateTime ThoiGianDiemDanh { get; set; }
        public bool TrangThaiHoanTatDiemDanh { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }


}
