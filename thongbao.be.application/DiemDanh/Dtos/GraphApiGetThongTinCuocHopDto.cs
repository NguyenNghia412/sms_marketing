using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public  class GraphApiGetThongTinCuocHopDto
    {
        public string EmailOrganizer { get; set; } = String.Empty;
        public string JoinWebUrl { get; set; } = String.Empty;
        public int IdCuocHop { get; set; }


    }
}
