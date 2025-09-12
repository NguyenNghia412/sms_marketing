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
    [Table(nameof(DanhBaChienDichTruongData), Schema = DbSchemas.Core)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(DanhBaChienDichTruongData)}"
    )]
    public  class DanhBaChienDichTruongData : ISoftDelted
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdDanhBa { get; set; }
        public string TenTruong { get; set; } = string.Empty;
        public string Type { get; set; } = "string";
        public string TruongImport { get; set; } = string.Empty;
        //các cột dùng để kéo thả làm sau 


        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }
}

