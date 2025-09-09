using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.DiemDanh.Dtos;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.DiemDanh.Interfaces
{
    public interface IHopTrucTuyenService
    {
        public void Create(CreateCuocHopDto dto);
        public BaseResponsePagingDto<ViewCuocHopDto> Find(FindPagingCuocHopDto dto);
        public void Update(int idCuocHop,UpdateCuochopDto dto);
        public void Delete(int idCuocHop);
        public BaseResponsePagingDto<ViewThongTinDiemDanhDto> ThongTinDiemDanhPaging(int idCuocHop,FindPagingThongTinDiemDanhDto dto);
        // public GraphApiAuthUrlResponseDto GenerateMicrosoftAuthUrl();
        // public  Task<GraphApiTokenResponseDto> HandleMicrosoftCallback(GraphApiCallbackDto dto);
        // public  Task<GraphApiUserInforResponseDto> GetUserInfo(string accessToken);
        public Task<MettingIdDto> GetThongTinCuocHop(GraphApiGetThongTinCuocHopDto dto,string userId);
        public Task<MettingIdDto> GetAndSaveMeetingInfo(GraphApiGetThongTinCuocHopDto dto, string userId);
        public  Task<string> GetUserIdByEmailAsync(string email);
        public Task UpdateTrangThaiDiemDanh(int idCuocHop,UpdateTrangThaiDiemDanhDto dto);
        public Task<byte[]> ExportDanhSachDiemDanhToExcel(int idCuocHop);
        public ViewThongKeDiemDanhResponseDto ThongKeDiemDanh(ViewThongKeDiemDanhRequestDto dto);
        public void CreateDotDiemDanh(CreateDotDiemDanhDto dto);

    }
}
