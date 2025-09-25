using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Auth.Dtos.Role;
using thongbao.be.application.Auth.Dtos.User;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.application.DiemDanh.Dtos;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.MauNoiDung.Dtos;
using thongbao.be.application.ToChuc.Dtos;
using thongbao.be.domain.Auth;
using thongbao.be.domain.DanhBa;
using thongbao.be.domain.DiemDanh;
using thongbao.be.domain.GuiTinNhan;
using thongbao.be.domain.MauNoiDung;

namespace thongbao.be.application.Base
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<Source, Destination>();
            CreateMap<ChienDich, ViewChienDichDto>();
            CreateMap<AppUser, ViewUserDto>();
            CreateMap<IdentityRole, ViewRoleDto>();
            CreateMap<HopTrucTuyen, ViewCuocHopDto>();
            CreateMap<ThongTinDiemDanh, ViewThongTinDiemDanhDto>();
            CreateMap<DotDiemDanh, ViewDotDiemDanhDto>();
            CreateMap<domain.DanhBa.DanhBa, ViewDanhBaDto>();
            CreateMap<domain.ToChuc.ToChuc, ViewToChucDto>();
            CreateMap<domain.GuiTinNhan.BrandName, GetListBrandNameResponseDto>();
            CreateMap<domain.DanhBa.DanhBa, GetListDanhBaResponseDto>();
            CreateMap<domain.DanhBa.DanhBaSms, ViewDanhBaChiTietDto>();
            CreateMap<domain.MauNoiDung.MauNoiDung, ViewMauNoiDungDto>();
            CreateMap<domain.MauNoiDung.MauNoiDung, GetListMauNoiDungResponseDto>();
            CreateMap<ChienDichLogTrangThaiGui, ViewChienDichLogDto>();
        }
    }
}
