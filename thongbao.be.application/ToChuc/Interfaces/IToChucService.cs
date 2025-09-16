using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.ToChuc.Dtos;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.ToChuc.Interfaces
{
    public interface  IToChucService
    {
        public void Create(CreateToChucDto dto);
        public void Update(int idToChuc, UpdateToChucDto dto);
        public void Delete(int idToChuc);
        public BaseResponsePagingDto<ViewToChucDto> Find(FindPagingToChucDto dto);
    }
}
