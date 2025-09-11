using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.DanhBa.Interfaces
{
    public interface  IDanhBaService
    {
        public void Create(CreateDanhBaDto dto);
        public void Update(int idDanhBa, UpdateDanhBaDto dto);
        public BaseResponsePagingDto<ViewDanhBaDto> Find(FindPagingDanhBaDto dto);
    }
}
