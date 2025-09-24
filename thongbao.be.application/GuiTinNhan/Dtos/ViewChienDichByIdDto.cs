using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public class ViewChienDichByIdDto
    {
        public string TenChienDich { get; set; } = String.Empty;
        public string TenDanhBa { get; set; } = String.Empty;
        public int IdBrandName  { get; set; }
        public string TenBrandName { get; set; } = String.Empty ;
        public bool IsFlashSms { get; set; }
        public bool IsAccented { get; set; }
        public string NoiDung { get; set; } = String.Empty ;
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get;set; }
    }
}
