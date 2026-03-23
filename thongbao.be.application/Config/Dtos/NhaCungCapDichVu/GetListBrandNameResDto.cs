using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaCungCapDichVu
{
    public class GetListBrandNameResDto
    {
        public int Id { get; set; }
        public string TenBrandName { get; set; } = string.Empty;
        public string MoTa { get; set; }= String.Empty;
        //public int IdNhaCungCapDichVu { get; set; }
    }
}
