using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.GuiTinNhan
{
    [Table(nameof(BrandName), Schema = DbSchemas.Core)]
    public  class BrandName :ISoftDelted
    {
        public int Id { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
        public string? Mota { get; set; } = String.Empty;
        public DateTime? ThoiGianTao {  get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
