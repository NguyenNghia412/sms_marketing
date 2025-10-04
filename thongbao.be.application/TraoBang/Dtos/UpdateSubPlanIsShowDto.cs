using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class UpdateSubPlanIsShowDto
    {
        public int Id { get; set; }
        public int IdPlan { get; set; }
        public bool IsShow { get; set; }
    }
}
