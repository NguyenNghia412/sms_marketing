using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Interfaces
{
    public  interface IGuiTinNhanJobService
    {
        public Task<List<object>> StartGuiTinNhanJob(int idChienDich, int idDanhBa, bool IsFlashSms, int idBrandName, string textNoiDung);
        public  Task SaveThongTinChienDich(int idChienDich, int idDanhBa, int idBrandName, bool IsFlashSms, string textNoiDung);
    }
}
