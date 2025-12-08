using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace thongbao.be.application.MauNoiDung.Dtos.MauNoiDungEmail
{
    public class CreateMauNoiDungEmailDto
    {
        public string TenMauNoiDung { get; set; } = string.Empty;
        public object ThietKe { get; set; } = new object();
    }
}
