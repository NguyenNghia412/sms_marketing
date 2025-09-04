using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.DiemDanh
{
    [Table(nameof(ThongTinDiemDanh), Schema = DbSchemas.Core)]
    [Index(
        nameof(Id),
        IsUnique = false,
        Name = $"IX_{nameof(ThongTinDiemDanh)}"
    )]
    public class ThongTinDiemDanh : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdCuocHop { get; set; }
        public int MaSoSinhVien { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string HoDem { get; set; } = String.Empty;
        public string Ten { get; set; } = String.Empty;
        public string Khoa { get; set; } = String.Empty;
        public string LopQuanLy { get; set; } = String.Empty;
        public string EmailHuce { get; set; } = String.Empty;
        public string SoDienThoai { get; set; } = String.Empty;
        public List<string> TinNhan { get; set; } = new List<string>();
        public int TrangThaiDiemDanh { get; set; } 
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }

    }
}
