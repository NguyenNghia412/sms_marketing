using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public class GetTruongDataDanhBaSmsResponseDto
    {
        public List<TruongDataItem> TruongData { get; set; } = new List<TruongDataItem>();
    }

    public class TruongDataItem
    {
        public int Id { get; set; }
        public string TenTruong { get; set; } = string.Empty;

    }
}