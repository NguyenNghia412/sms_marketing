using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class GetInforSubPlanDto
    {
        public string Ten { get; set; } = String.Empty;
        public int SoLuongThamGia { get; set; }
        public int SoLuongVangMat { get; set; }
        public int SoLuongDaTrao { get; set; }
        public int SoLuongConLai { get; set; }
    }
}
