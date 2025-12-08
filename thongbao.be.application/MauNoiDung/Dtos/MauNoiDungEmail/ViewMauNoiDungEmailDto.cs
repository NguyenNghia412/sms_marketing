using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.MauNoiDung.Dtos.MauNoiDungEmail
{
    public class ViewMauNoiDungEmailDto
    {
        public int Id { get; set; }
        public string TenMauNoiDung { get; set; } = string.Empty;
        //public string NoiDung { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
    }
}
