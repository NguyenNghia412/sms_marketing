using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class MettingIdDto
    {
        //public string? ODataContext { get; set; }
        public List<OnlineMeetingDto> Value { get; set; } = new();
    }
}
