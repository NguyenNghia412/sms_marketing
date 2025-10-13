using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public class ViewChienDichLogDto
    {
        public int IdChienDich { get; set; }
        public string TenChienDich { get; set; } = String.Empty;
        public ViewDanhBaLogDto? danhBa { get; set; } = new ViewDanhBaLogDto();
        public int TongSoSms { get; set; }
        public int SmsSentSuccess { get; set; }
        public int SmsSentFailed { get; set; }
        public string NoiDung { get; set; } = String.Empty;
        public string TrangThai { get; set; } = String.Empty;
        public int TongChiPhi { get; set; }
        public DateTime? NgayGui { get; set; } 

    }
    public class ViewDanhBaLogDto
    {
        public int IdDanhBa {  get; set; }
        public string TenDanhBa { get; set; } = String.Empty ;
    }
    
}
