using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Config.Dtos.NhaCungCapDichVu;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.Config.Interfaces
{
    public interface INhaCungCapDichVuService
    {
        public void Create(CreateNhaCungCapDichVuDto dto);
        public void Update(UpdateNhaCungCapDichVuDto dto);
        public BaseResponsePagingDto<ViewNhaCungCapDichVuDto> FindPaging(FindPagingNhaCungCapDichVuDto dto);
        public void Delete(int id);
        public ViewNhaCungCapByIdDto GetById(int id);
        public List<GetDropDownNhaCungCapDichVuDto> GetDropDownNhaCungCapDichVu();
        public void AddBrandNameToNhaCungCapDichVu(AddBrandNameToNhaCungCapDichVuDto dto);
        public void DeleteBrandNameToNhaCungCapDichVu(DeleteBrandNameToNhaCungCapDichVuDto dto);

        public  Task AddUserToNhaCungCapDichVu(AddUserToNhaCungCapDichVuDto dto);
        public void UpdateUserToNhaCungCapDichVu(UpdateUserToNhaCungCapDichVuDto dto);
        public void DeleteUserToNhaCungCapDichVu(int idUserNhaCungCapDichVu);
        public BaseResponsePagingDto<ViewUserNhaCungCapDto> FindPagingUserNhaCungCapDichVu(FindPagingUserToNhaCungCapDichVuDto dto);
        public ViewUserToNhaCungCapDichVuByIdDto FindById(int idUserNhaCungCapDichVu);
        public List<GetListBrandNameResDto> GetListBrandName(int idNhaCungCapDichVu);
        public List<GetListDropDownUserNhaCungCapDichVuDto> GetListDropDownUserNhaCungCapDichVu(int idNhaCungCapDichVu);
        public List<GetListBrandNameResDto> GetListBrandNameByCurrentUser();
    }
}
