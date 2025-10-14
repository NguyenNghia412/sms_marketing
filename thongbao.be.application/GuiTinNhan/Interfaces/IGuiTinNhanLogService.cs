using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.GuiTinNhan.Interfaces
{
    public interface IGuiTinNhanLogService
    {
        public BaseResponsePagingDto<ViewChienDichLogDto> PagingChienDichLog(FindPagingChienDichLogDto dto);
        public BaseResponsePagingDto<ViewDanhBaSmsLogDto> PagingGuiTinNhanLog(int idChienDich, FindPagingGuiTinNhanLogDto dto);
        public  Task<byte[]> ExportThongKeTheoChienDich(ExportSmsLogTheoChienDichDto dto);
        public  Task<byte[]> ExportThongKeTheoThang(ExportSmsLogTheoThangDto dto);

    }
}
