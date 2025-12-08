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
        public  Task ProcessGuiTinNhanBackground(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung, string currentUserId, bool isSuperAdmin);
        public Task SendSmsLog(object smsResponse, int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool isAccented, string noiDung, string currentUserId, bool isSuperAdmin);
        
    }
}
