using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public class CreateDanhBaFromTinNhanErrorDto
    {
        public int IdChienDich {get;set;}
        public List<ListTinNhanError> Items { get; set; } = new List<ListTinNhanError>();
    }
    public class ListTinNhanError
    {
        public int IdDanhBa { get; set; }
        public int IdDanhBaSms { get; set; }
    }

}
