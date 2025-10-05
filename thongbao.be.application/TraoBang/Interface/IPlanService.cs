using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.TraoBang.Dtos;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.TraoBang.Interface
{
    public interface IPlanService
    {
        public void Create(CreatePlanDto dto);
        public void Update(int id, UpdatePlanDto dto);
        public BaseResponsePagingDto<ViewPlanDto> FindPaging(FindPagingPlanDto dto);
        public void Delete(int id);
        public  Task<List<GetListPlanResponseDto>> GetListPlan();
    }
}
