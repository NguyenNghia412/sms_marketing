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
    [Table(nameof(DotDiemDanh), Schema = DbSchemas.Core)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(DotDiemDanh)}"
  )]
    public class DotDiemDanh : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdCuocHop { get; set; }
        public string TenDotDiemDanh { get; set; } = String.Empty;
        public string TenMonHoc { get; set; } = String.Empty;
        public string MaMonHoc { get; set; } = String.Empty;
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; } = String.Empty;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }

    }
}

    

