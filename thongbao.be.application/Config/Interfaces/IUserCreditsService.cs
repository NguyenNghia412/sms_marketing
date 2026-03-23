using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Config.Dtos.NhaMang;
using thongbao.be.application.Config.Dtos.UserCredits;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.Config.Interfaces
{
    public interface IUserCreditsService
    {
        public void AddUserCredits(AddUserCreditsDto dto);

        public void UpdateUserCredits(UpdateUserCreditsDto dto);
        public void DeleteUserCredits(int id);

        public BaseResponsePagingDto<ViewUserCreditsDto> Find(FindPagingDto dto);

        public ViewUserCreditsDto FindById(int id);
        public GetDonViDto GetDonVi(int id);
        public BaseResponsePagingDto<ViewUserCreditsDto> FindPagingByUserId(FindPagingByUserIdDto dto);
        public  Task AddCreditToUserCredits();
        public ViewUserCreditsByUserDto? GetCurrentUserCredits();

        public void UpdateToiDaHanMucCreditsGiaHan(UpdateToiDaHanMucCreditsGiaHanDto dto);
        public GetToiDaHanMucCreditsGiaHanDto GetToiDaHanMucCreditsGiaHan(int id);
    }
}
