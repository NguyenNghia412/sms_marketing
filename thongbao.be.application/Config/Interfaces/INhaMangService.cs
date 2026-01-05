using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Config.Dtos.NhaMang;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.Config.Interfaces
{
    public interface INhaMangService
    {
        public void AddNhaMang(AddNhaMangDto dto);
        public void UpdateNhaMang(UpdateNhaMangDto dto);

        public void DeleteNhaMang(int id);
        public BaseResponsePagingDto<ViewNhaMangDto> FindPaging(FindPagingNhaMangDto dto);
        public ViewNhaMangDto GetById(int id);
    }
}
