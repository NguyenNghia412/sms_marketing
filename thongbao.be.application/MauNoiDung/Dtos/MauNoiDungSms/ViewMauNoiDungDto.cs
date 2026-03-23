using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.MauNoiDung.Dtos.MauNoiDungSms
{
    public class ViewMauNoiDungDto
    {
        public int Id { get; set; }
        public string TenMauNoiDung {  get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
    }
}
