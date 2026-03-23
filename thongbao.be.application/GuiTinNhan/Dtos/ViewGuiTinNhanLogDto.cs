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
        //public string SoDienThoai { get; set; } = String.Empty;
        public int IdDanhBa { get; set; }
        public int IdDanhBaSms { get; set; }
        public BrandNameDto BrandName { get; set; } = new BrandNameDto();
        public ViewGuiTinNhanLogDto Log { get; set; } = new ViewGuiTinNhanLogDto();
        public CreatedByGuiTinNhanLogDto Users { get; set; } = new CreatedByGuiTinNhanLogDto();
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
        public int? Price { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = String.Empty;
        public DateTime? NgayGui { get; set; }
        public int SoLuongTinNhan { get; set; }


    }

    public class CreatedByGuiTinNhanLogDto
    {
        public string Id { get; set; } = String.Empty;
        //public string UserName { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
        //public string SoDienThoai { get; set; } = String.Empty;
        //public string Email { get; set; } = String.Empty;
    }
}
