using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public  class MoveOrderSubPlanDto
    {
        public int IdPlan { get; set; }
        public int IdSubPlan { get; set; }
        public int NewOrder { get; set; }
    }
}
