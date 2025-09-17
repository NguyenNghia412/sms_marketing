using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public  class CreateNguoiNhanDto
    {
        public int IdDanhBa { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string MaSoNguoiDung { get; set; } = String.Empty;
        public string SoDienThoai { get; set; } = String.Empty;
        public string EmailHuce { get; set; } = String.Empty;
    }
}
