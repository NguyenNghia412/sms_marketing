using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public class StartGuiTinNhanJobDto
    {
        public int IdChienDich { get; set; }
        public int IdDanhBa { get; set; }
        public string TextNoiDung { get; set; } = string.Empty;
    }
}
