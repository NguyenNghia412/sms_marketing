using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Auth.Dtos.User;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.domain.Auth;
using thongbao.be.domain.GuiTinNhan;

namespace thongbao.be.application.Base
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<Source, Destination>();
            CreateMap<ChienDich, ViewChienDichDto>();
            CreateMap<AppUser, ViewUserDto>();
        }
    }
}
