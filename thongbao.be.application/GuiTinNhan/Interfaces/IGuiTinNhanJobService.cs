using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.GuiTinNhan.Dtos;

namespace thongbao.be.application.GuiTinNhan.Interfaces
{
    public  interface IGuiTinNhanJobService
    {
        public Task<List<object>> StartGuiTinNhanJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, bool IsFlashSms, int idBrandName, bool IsAccented, string noiDung);
        public  Task SaveThongTinChienDich(int idChienDich, int? idDanhBa, int? idBrandName, bool IsFlashSms, bool IsAccented, string noiDung);
        public  Task<object> GetPreviewMessage(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, bool IsFlashSms, int idBrandName, bool IsAccented, string noiDung, int currentIndex);
        public  Task<object> GetChiPhiDuTruChienDich(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool isFlashSms, bool isAccented, string noiDung);
        public  Task SendSmsLog(object smsResponse, int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool isAccented, string noiDung);
        public  Task<object> GetSoLuongNguoiNhanVaTinNhan(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool isFlashSms, bool isAccented, string noiDung);
    }
}
