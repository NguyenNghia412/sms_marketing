using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.DanhBa.Dtos
{
    public class FindPagingDanhBaChiTietDto: BaseRequestPagingDto
    {
        public List<ListFieldIsHidden>? Items {  get; set; } 
    }

    public class ListFieldIsHidden
    {
        public int IdDanhBaTruongData { get; set; }
    }
}
