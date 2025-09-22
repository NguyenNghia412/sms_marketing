using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public class DanhBaDataInfoDto
    {
        public int IdDanhBaChiTiet { get; set; }
        public int IdTruongData { get; set; }
        public string Data { get; set; } = String.Empty;
    }
}
