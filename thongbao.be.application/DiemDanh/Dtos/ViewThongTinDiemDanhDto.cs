using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class ViewThongTinDiemDanhDto
    {
        public int Id { get; set; }
        public string MaSoSinhVien { get; set; } = String.Empty;
        public string HoVaTen { get; set; } = String.Empty;
        public string HoDem {  get; set; } = String.Empty;
        public string Ten {  get; set; } = String.Empty;
        public string Khoa {  get; set; } = String.Empty;
        public string LopQuanLy {  get; set; } = String.Empty;
        public string EmailHuce { get; set; } = String.Empty;
        public string SoDienThoai { get; set; } = String.Empty;
        public int TrangThaiDiemDanh { get; set; }
    }
}
