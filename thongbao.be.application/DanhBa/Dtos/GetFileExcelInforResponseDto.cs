using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public class GetFileExcelInforResponseDto
    {
        public List<SheetInfoDto> Sheets { get; set; } = new List<SheetInfoDto>();
    }



    public class SheetInfoDto
    {
        public string SheetName { get; set; } = String.Empty;
        public List<string> Headers { get; set; } = new List<string>();
    }
}