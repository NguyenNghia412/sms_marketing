using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.Config.Dtos.NhaCungCapDichVu
{
    public class FindPagingUserToNhaCungCapDichVuDto : BaseRequestPagingDto
    {
        public int IdNhaCungCapDichVu { get; set; }
    }
}
