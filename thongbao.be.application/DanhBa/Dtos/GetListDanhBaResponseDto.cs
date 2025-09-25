using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public  class GetListDanhBaResponseDto
    {
        public int Id { get; set; }
        public string TenDanhBa { get; set; } = String.Empty;
        public List<GetTruongDanhBaDto> TruongData { get; set; } = new List<GetTruongDanhBaDto>();
    }
    public class GetTruongDanhBaDto
    {
        public int Id { get; set; }
        public string TenTruong { get; set; } = String.Empty ;
    }
}
