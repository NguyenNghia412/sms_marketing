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
        public bool HasError { get; set; }
        public string? FileKey { get; set; } = String.Empty;
        public List<SoLuongLoiDto>? Data { get; set; } = new List<SoLuongLoiDto>();
        public int TotalRowsImported { get; set; }
        public int TotalDataImported { get; set; }
    }
    public class SoLuongLoiDto
    {
        public int? SoLuongLoi { get; set; }
        public string? NguyenNhanLoi { get; set; } = String.Empty;
    }
    public class FileImportFailedCache
    {
        public MemoryStream Stream { get; set; } = new   MemoryStream();
        public string FileName { get; set; } = String.Empty;
        public string ContentType { get; set; } = String.Empty;
    }
}
