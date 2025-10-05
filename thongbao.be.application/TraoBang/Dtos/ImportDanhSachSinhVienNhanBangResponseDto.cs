using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class ImportDanhSachSinhVienNhanBangResponseDto
    {
        public int TotalRowsImported { get; set; }
        public int TotalDataImported { get; set; }
        public int ImportTimeInSeconds { get; set; }
    }
}
