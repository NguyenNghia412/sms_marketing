using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class GhiNhanDiemDanhRequestDto
    {
        public int IdDotDiemDanh { get; set; }
        public string EmailHuce { get; set; } = String.Empty;
    }
}
