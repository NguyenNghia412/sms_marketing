using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.shared.Constants.DanhBa
{
   public enum LoaiDanhBaEnum
    {
        DanhBaCung = 1,
        DanhBaChienDich = 2,
    }
    public enum GioiTinhEnum
    {
        ChuaXacDinh = 0,
        Nu = 1,
        Nam = 2,
        Khac = 3
    }
    public enum LoaiNguoiDungEnum
    {
        SinhVien = 1,
        NhanVien = 2,
        Khac = 3,
    }
    public enum TrangThaiHoatDongEnum
    {
        DangHoatDong = 1,
        NgungHoatDong = 0,

    }
    public static class TypeDanhBa
    {
        public const int Sms = 1;
        public const int Email = 2;
    }
}
