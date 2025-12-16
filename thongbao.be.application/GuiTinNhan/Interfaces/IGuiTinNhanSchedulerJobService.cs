using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.GuiTinNhan.Dtos;

namespace thongbao.be.application.GuiTinNhan.Interfaces
{
    public interface IGuiTinNhanSchedulerJobService
    {
        public Task ProcessGuiTinNhanBackgroundSchedulerJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiCoLichGuiDto> danhSachSoDienThoai, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung, string currentUserId, bool isSuperAdmin, DateTime lichGui);
        public Task SendSmsSchedulerJobLog(object smsResponse, int idChienDich, int? idDanhBa, List<ListSoDienThoaiCoLichGuiDto> danhSachSoDienThoai, int idBrandName, bool isAccented, string noiDung, string currentUserId, bool isSuperAdmin);
    }
}
