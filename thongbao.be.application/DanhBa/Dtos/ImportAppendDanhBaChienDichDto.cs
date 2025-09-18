using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public  class ImportAppendDanhBaChienDichDto
    {
        
            public IFormFile File { get; set; } 
            public int IndexRowStartImport { get; set; }
            public int IndexRowHeader { get; set; }
            public string SheetName { get; set; } = String.Empty;
            public int IdDanhBa { get; set; } // ID danh bạ để import vào
        
    }
}
