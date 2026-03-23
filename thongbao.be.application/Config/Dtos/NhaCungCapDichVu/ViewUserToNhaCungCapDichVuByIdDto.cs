using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaCungCapDichVu
{
    public class ViewUserToNhaCungCapDichVuByIdDto
    {
        public int Id { get; set; }
        public ViewUserNhaCungCapWithDetailsByIdDto User { get; set; } = new ViewUserNhaCungCapWithDetailsByIdDto();
        public ViewNhaCungCapDichVu NhaCungCapDichVu { get; set; } = new ViewNhaCungCapDichVu();
        public ViewBrandNameNhaCungCapDichVu BrandName { get; set; } = new ViewBrandNameNhaCungCapDichVu();
    }
    public class ViewNhaCungCapDichVu
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }
    public class ViewBrandNameNhaCungCapDichVu
    {
        public int Id { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
    }
    public class ViewUserNhaCungCapWithDetailsByIdDto
    {
        public string IdUser { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
    }
}
