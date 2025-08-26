using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.GuiTinNhan.Interfaces
{
    public interface IChienDichService
    {
        /// <summary>
        /// Tạo chiến dịch
        /// </summary>
        /// <param name="dto"></param>
        public void Create(CreateChienDichDto dto);

        /// <summary>
        /// Tìm kiếm chiến dịch có phân trang
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public BaseResponsePagingDto<ViewChienDichDto> Find(FindPagingChienDichDto dto);
    }
}
