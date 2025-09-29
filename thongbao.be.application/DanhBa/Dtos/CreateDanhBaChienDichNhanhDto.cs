using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public class CreateDanhBaChienDichNhanhDto
    {
        public string TenDanhBa { get; set; } = String.Empty;
        public int Type { get; set; }
        public List<string> Truong { get; set; } = new List<string>();
        public List<string> Data { get; set; } = new List<string>();

    }
}
