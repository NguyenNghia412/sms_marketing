using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaCungCapDichVu
{
    public class AddUserToNhaCungCapDichVuDto
    {
        public int IdNhaCungCapDichVu { get; set; }
        public string IdUser { get; set; } = String.Empty;
        public List<int> IdBrandName { get; set; } = new List<int>();
        public DateTime? ThoiGianBatDauSuDungDichVu { get; set; }
        public DateTime? ThoiGianKetThucSuDungDichVu { get; set; }
    }
}
