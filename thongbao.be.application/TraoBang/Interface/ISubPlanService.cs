using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.application.TraoBang.Dtos;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.TraoBang.Interface
{
    public interface ISubPlanService
    {
        public void Create(int idPlan, CreateSubPlanDto dto);
        public Task Update(UpdateSubPlanDto dto);
        public BaseResponsePagingDto<ViewSubPlanDto> FindPaging(FindPagingSubPlanDto dto);
        public void UpdateIsShow(UpdateSubPlanIsShowDto dto);
        public void Delete(int idPlan, int idSubPlan);
        public  Task<List<UpdateOrderSubPlanResponseDto>> MoveOrder(MoveOrderSubPlanDto dto);
        public  Task<List<GetListSubPlanResponseDto>> GetListSubPlan(int idPlan);
        public  Task<ImportDanhSachSinhVienNhanBangResponseDto> ImportDanhSachNhanBang(ImportDanhSachSinhVienNhanBangDto dto);
        public BaseResponsePagingDto<ViewSinhVienNhanBangDto> PagingSinhVienNhanBang(FindPagingSinhVienNhanBangDto dto);
        public void CreateSinhVienNhanBang(CreateSinhVienNhanBangDto dto);
        public  Task UpdateSinhVienNhanBang(UpdateSinhVienNhanBangDto dto);
        public void DeleteSinhVienNhanBang(int idSubPlan, int id);
        public  Task<ViewSinhVienNhanBangDto> ShowSinhVienNhanBangInfor(string mssv);
    }
}
