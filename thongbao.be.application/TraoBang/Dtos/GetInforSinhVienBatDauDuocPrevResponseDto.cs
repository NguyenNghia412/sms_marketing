using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class GetInforSinhVienBatDauDuocPrevResponseDto
    {

        public GetInforSinhVienChuanBiDuocTraoBangResponseDto SvBatDauLui { get; set; } = new GetInforSinhVienChuanBiDuocTraoBangResponseDto();
        public GetInforSinhVienChuanBiDuocTraoBangResponseDto? SvChuanBiTiepTheo { get; set; }
    }
}
