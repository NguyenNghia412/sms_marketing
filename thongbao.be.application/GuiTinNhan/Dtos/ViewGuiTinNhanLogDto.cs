using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public class ViewDanhBaSmsLogDto
    {
        public int Id { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        //public string MaSoNguoiDung { get; set; } = String.Empty;
        public string SoDienThoai { get; set; } = String.Empty;

        public BrandNameDto BrandName { get; set; } = new BrandNameDto();
        public ViewGuiTinNhanLogDto Log { get; set; } = new ViewGuiTinNhanLogDto();
    }
    public class BrandNameDto
    {
        public int Id { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
    }
    public class ViewGuiTinNhanLogDto
    {
        public string SoDienThoai { get; set; } = String.Empty;
        public string NoiDungChiTiet { get; set; } = String.Empty;
        public int Price { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = String.Empty;
        public DateTime? NgayGui { get; set; }

    }
}
