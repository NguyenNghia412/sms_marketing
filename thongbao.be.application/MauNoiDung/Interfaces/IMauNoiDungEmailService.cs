using thongbao.be.application.MauNoiDung.Dtos.MauNoiDungEmail;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.MauNoiDung.Interfaces
{
    public interface IMauNoiDungEmailService
    {
        public CreateResultEmailTemplateDto Create(CreateMauNoiDungEmailDto dto);
        public void Update(UpdateMauNoiDungEmailDto dto);
        public BaseResponsePagingDto<ViewMauNoiDungEmailDto> Find(FindPagingMauNoiDungEmailDto dto);
        public void Delete(int id);

        public List<GetListMauNoiDungEmailResponseDto> GetListMauNoiDung();
        public ViewMauNoiDungEmailByIdDto FindById(int idMnd);
    }
}
