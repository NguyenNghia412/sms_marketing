using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.UserCredits
{
    public class GetToiDaHanMucCreditsGiaHanDto
    {
        public int Id { get; set; }
        public string ToiDaHanMucCreditGiaHan { get; set; } = String.Empty;
        public string DonVi { get; set; } = "VND";
    }
}
