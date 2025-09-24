using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Interfaces
{
    public  interface IGuiTinNhanJobService
    {
        public Task<List<object>> StartGuiTinNhanJob(int idChienDich, int idDanhBa, bool IsFlashSms, int idBrandName, bool IsAccented, string textNoiDung);
        public  Task SaveThongTinChienDich(int idChienDich, int idDanhBa, int idBrandName, bool IsFlashSms, bool IsAccented, string textNoiDung);
        public  Task<object> GetPreviewMessage(int idChienDich, int idDanhBa, bool IsFlashSms, int idBrandName, bool IsAccented, string textNoiDung, int currentDanhBaSmsId);
        public  Task<object> GetChiPhiDuTruChienDich(int idChienDich, int idDanhBa, int idBrandName, bool isFlashSms, bool isAccented, string textNoiDung);
        public  Task SendSmsLog(object smsResponse, int idChienDich, int idDanhBa, int idBrandName, bool isAccented, string textNoiDung);
    }
}
