using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public class VerifyImportDanhBaChienDichResponseDto
    {
        //public string? Warning { get; set; } = String.Empty;
        public IFormFile FileFailed { get; set; } 
        public int TotalRowsImported { get; set; }
        public int TotalDataImported { get; set; }
    }
}
