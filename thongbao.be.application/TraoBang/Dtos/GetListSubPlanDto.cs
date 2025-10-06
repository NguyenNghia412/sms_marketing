using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class GetListSubPlanDto
    {
        public List<GetSubPlanItemDto> Items { get; set; } = new List<GetSubPlanItemDto>();
    }
    public class GetSubPlanItemDto
    {
        public int Id { get; set; }
        public string Ten { get; set; } = String.Empty;
        public string TienDo { get; set; } = String.Empty;
    }
}
