using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.MauNoiDung.Dtos.MauNoiDungSms
{
    public class CreateChienDichByMauNoiDungDto
    {
        private string _tenChienDich = string.Empty;
        private string _moTa = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Không được bỏ trống")]
        required public string TenChienDich { get => _tenChienDich; set => _tenChienDich = value?.Trim()!; }

        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string MoTa { get => _moTa; set => _moTa = value?.Trim()!; }
        public string MauNoiDung { get; set; } = string.Empty;
        public bool IsFlashSms { get; set; }
    }
}
