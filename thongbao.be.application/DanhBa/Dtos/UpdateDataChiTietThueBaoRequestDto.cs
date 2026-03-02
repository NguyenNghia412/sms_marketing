using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public class UpdateDataChiTietThueBaoRequestDto
    {
        public int IdDanhBa { get; set; }
        public int IdThueBao { get; set; }
        public List<DataChiTietThueBao> Items { get; set; } = new List<DataChiTietThueBao>();
    }

    public class DataChiTietThueBao
    {
        public int IdData { get; set; }
        public string Data { get; set; } = String.Empty;
    }
}
