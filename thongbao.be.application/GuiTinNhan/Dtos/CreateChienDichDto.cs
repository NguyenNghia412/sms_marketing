using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    
    public class CreateChienDichDto
    {
        private string _tenChienDich = String.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Không được bỏ trống")]
        required public string TenChienDich { get => _tenChienDich; set => _tenChienDich = value?.Trim()!; }
        public DateTime?  NgayBatDau { get; set; }
        public DateTime?  NgayKetThuc { get; set; }
        


    }
}
