using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using thongbao.be.domain.DiemDanh;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.DanhBa
{
    [Table(nameof(DanhBaChienDichData), Schema = DbSchemas.Core)]
    [Index(
     nameof(Id),
     IsUnique = false,
     Name = $"IX_{nameof(DanhBaChienDichData)}"
    )]
    public class DanhBaChienDichData : ISoftDelted
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Data { get; set; } = string.Empty;
        public int IdTruongData { get; set; }
        public int IdThongTinDanhBa { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    
    }
}
