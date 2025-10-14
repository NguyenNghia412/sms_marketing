using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public class FindPagingGuiTinNhanLogDto : BaseRequestPagingDto
    {
        public int? idDanhBa { get; set; }
    }
}
