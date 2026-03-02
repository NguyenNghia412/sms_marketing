using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public class ViewChiTietThueBaoNguoiNhanDto
    {
        //public int IdDanhBa { get; set; }
        public List<ViewChiTietThueBaoNguoiNhanDataByIdDto> Items { get; set; } = new List<ViewChiTietThueBaoNguoiNhanDataByIdDto>();
    }

    
    public class ViewChiTietThueBaoNguoiNhanDataByIdDto
    {
        public int IdTruong { get; set; }
        public string TenTruong { get; set; } = String.Empty;
        public int IdData { get; set; }
        public string Data { get; set; } = String.Empty;
    }
}
