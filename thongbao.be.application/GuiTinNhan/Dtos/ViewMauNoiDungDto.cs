using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public class ViewMauNoiDungDto
    {
        public int Id { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
    }
}
