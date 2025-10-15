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
    [Table(nameof(TinNhanHopTrucTuyen), Schema = DbSchemas.Core)]
    [Index(
        nameof(Id),
        IsUnique = false,
        Name = $"IX_{nameof(TinNhanHopTrucTuyen)}"
    )]
    public  class TinNhanHopTrucTuyen : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CuocHopId { get; set; }
        public int ThongTinDiemDanhId { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public DateTime ThoiGianGui { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
