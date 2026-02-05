using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaCungCapDichVu
{
    public class ViewUserNhaCungCapDto
    {
        public int Id { get; set; }
        public ViewUserNhaCungCapWithDetailsDto User { get; set; } = new ViewUserNhaCungCapWithDetailsDto();
        public NhaCungCapDichVu NhaCungCapDichVu { get; set; } = new NhaCungCapDichVu();
        public ViewBrandName BrandName { get; set; } = new ViewBrandName();
    }
    public class NhaCungCapDichVu
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }
    public class ViewBrandName
    {
        public int Id { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
    }
    public class ViewUserNhaCungCapWithDetailsDto
    {
        public string IdUser { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
    }

}
