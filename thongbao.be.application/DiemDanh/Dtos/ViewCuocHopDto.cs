using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class RoleUserDto {
        public Guid Id { get; set; }
        public string? Name { get; set; } = String.Empty;
    }

    public class UserCreateCuocHopDto { 
        public Guid Id { get; set; }
        public string? UserName { get; set; } = String.Empty;
        public string? Email { get; set; } = String.Empty;
        public string? FullName { get; set; } = String.Empty;
        public RoleUserDto Item { get; set; } = new RoleUserDto();

    }
    public class ViewCuocHopDto
    {
        public int Id { get; set; }
        public string TenCuocHop { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public DateTime? ThoiGianDiemDanh { get; set; }
        public string? IdCuocHop { get; set; } = String.Empty;
        public string? LinkCuocHop { get; set; } = String.Empty;
        public string IdTinNhanChung { get; set; }  = String.Empty;
        public UserCreateCuocHopDto Item { get; set; } = new UserCreateCuocHopDto();



    }
}
