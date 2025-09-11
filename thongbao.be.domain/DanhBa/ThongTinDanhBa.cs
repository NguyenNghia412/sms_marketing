using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [Table(nameof(ThongTinDanhBa), Schema = DbSchemas.Core)]
    [Index(
    nameof(Id),
    IsUnique = false,
    Name = $"IX_{nameof(ThongTinDanhBa)}"
    )]
    public class ThongTinDanhBa: ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdDanhBa { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string Mssv { get; set; } = String.Empty;
        public string SoDienThoai { get; set; } = String.Empty;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }
}
