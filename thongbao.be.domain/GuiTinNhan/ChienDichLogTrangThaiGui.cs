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
    [Table(nameof(ChienDichLogTrangThaiGui), Schema = DbSchemas.Core)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(ChienDichLogTrangThaiGui)}"
    )]
    public  class ChienDichLogTrangThaiGui: ISoftDelted
    {
        public int Id { get; set; }
        public int IdChienDich {  get; set; }
        public int? IdDanhBa {  get; set; }
        public int IdBrandName { get; set; }
        public int SmsSendSuccess { get; set; }
        public int SmsSendFailed { get; set; }
        public int TongSoSms { get; set; }
        public string TrangThai {  get; set; } = String.Empty;
        public int TongChiPhi { get; set; }
        public string NoiDung { get; set; } = String.Empty;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }

    }
}
