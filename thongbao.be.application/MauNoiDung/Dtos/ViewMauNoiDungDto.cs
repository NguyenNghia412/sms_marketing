using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.MauNoiDung.Dtos
{
    public class ViewMauNoiDungDto
    {
        public int Id { get; set; }
        public string TenMauNoiDung {  get; set; } = String.Empty;
        public string NoiDung { get; set; } = String.Empty;
        public DateTime? CreatedDate { get; set; }
    }
}
