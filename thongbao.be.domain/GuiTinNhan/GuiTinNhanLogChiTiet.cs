using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.GuiTinNhan
{

    [Table(nameof(GuiTinNhanLogChiTiet), Schema = DbSchemas.Core)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(GuiTinNhanLogChiTiet)}"
    )]
    public class GuiTinNhanLogChiTiet : ISoftDelted
    {
        public int Id { get; set; }
        public int IdChienDich { get; set; }
        public int? IdDanhBa { get; set; }
        public int IdBrandName { get; set; }
        public int? IdDanhBaSms { get; set; }
        public string SoDienThoai { get; set; } = String.Empty;
        public int Price { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = String.Empty;
        public string TrangThai { get; set; } = String.Empty;
        public string NoiDungChiTiet { get; set; } = String.Empty;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }
}
