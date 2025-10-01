using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public  class ImportDanhBaSmsDto
    {
        public string TenDanhBa { get; set; } = String.Empty;
        public int Type { get; set; }
        public required IFormFile File { get; set; }
        public int InDexColumnSoDienThoai { get; set; }
        public int IndexColumnHoTen { get; set; }
        public int IndexRowStartImport { get; set; }
        public int IndexRowHeader { get; set; }
        public string SheetName { get; set; } = String.Empty;
        public string? Mota { get; set; } 

    }
}
