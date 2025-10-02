using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Auth.Dtos.Role;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.Auth.Interfaces
{
    public interface IRoleService
    {
        public Task Create(CreateRoleDto dto);
        public Task Update(UpdateRoleDto dto);
        public Task<BaseResponsePagingDto<ViewRoleDto>> FindPaging(FindPagingRoleDto dto);
        public Task<ViewRoleDto> FindById(string id);
        public Task<List<ViewRoleDto>> GetList();
    }
}
