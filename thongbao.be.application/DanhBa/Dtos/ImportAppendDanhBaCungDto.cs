using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public class ImportAppendDanhBaCungDto
    {
        public string Url { get; set; } = String.Empty;
        public string SheetName { get; set; } = String.Empty;
        public int IndexRowStartImport { get; set; }
        public int IndexRowHeader {  get; set; }

    }
}
