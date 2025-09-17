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
        public void Create(int idBrandName, int idDanhBa, CreateChienDichDto dto);

        /// <summary>
        /// Tìm kiếm chiến dịch có phân trang
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public BaseResponsePagingDto<ViewChienDichDto> Find(FindPagingChienDichDto dto);
        public void Update(int idChienDich, UpdateChienDichDto dto);
        public void Delete(int idChienDich);
        public void AddDanhBaChienDich(int idChienDich, int idDanhBa);
        public List<GetListBrandNameResponseDto> GetListBrandName();
        public void TestSendEmail();
    }
}
