using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public  class ViewDanhBaDto
    {
        public int Id { get; set; }
        public string TenDanhBa { get; set; } = String.Empty;
        public string? Mota { get; set; }
        public string? GhiChu { get; set; }
        public int SoLuongNguoiNhan { get; set; }
        public int Type { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
