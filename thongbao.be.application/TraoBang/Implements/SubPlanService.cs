using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using EFCore.BulkExtensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.application.MauNoiDung.Dtos;
using thongbao.be.application.TraoBang.Dtos;
using thongbao.be.application.TraoBang.Interface;
using thongbao.be.domain.TraoBang;
using thongbao.be.infrastructure.data;
using thongbao.be.infrastructure.data.Migrations;
using thongbao.be.infrastructure.external.SignalR.Service.Interfaces;
using thongbao.be.shared.Constants.TraoBang;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.TraoBang.Implements
{
    public class SubPlanService:BaseService, ISubPlanService
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly ITraoBangService _traoBangService;
        private readonly IConfiguration _configuration;
        public SubPlanService(
            SmDbContext smDbContext,
            ILogger<SubPlanService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IConfiguration configuration,
            ITraoBangService traoBangService
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _configuration = configuration;
            _traoBangService = traoBangService;
        }
        public void Create(int idPlan, CreateSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}, dto = {JsonSerializer.Serialize(dto)}");

            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == idPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);

            var maxOrder = _smDbContext.SubPlans
                .Where(x => x.IdPlan == idPlan && !x.Deleted)
                .Max(x => (int?)x.Order) ?? 0;

            var subplan = new domain.TraoBang.SubPlan
            {
                Ten = dto.Ten,
                MoTa = dto.MoTa,
                TruongKhoa = dto.TruongKhoa,
                MoBai = dto.MoBai ?? "",
                KetBai = dto.KetBai ?? "",
                Note = dto.Note,
                IsShow = true,
                Order = maxOrder + 1,
                IdPlan = idPlan,
                CreatedDate = GetVietnamTime(),
                Deleted = false
            };

            _smDbContext.SubPlans.Add(subplan);
            _smDbContext.SaveChanges();
        }
        public async Task Update(UpdateSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}, dto = {JsonSerializer.Serialize(dto)}");

            var plan = await _smDbContext.Plans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.IdPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);

            var subplan = await _smDbContext.SubPlans
                .FirstOrDefaultAsync(x => x.Id == dto.IdSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var subPlans = await _smDbContext.SubPlans
                .Where(x => x.IdPlan == dto.IdPlan && !x.Deleted)
                .OrderBy(x => x.Order)
                .ToListAsync();

            var movingSubPlan = subPlans.FirstOrDefault(x => x.Id == dto.IdSubPlan && !x.Deleted);
            if (movingSubPlan == null)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            }

            var currentOrder = movingSubPlan.Order;
            var newOrder = dto.NewOrder;
            if( newOrder < 1 || newOrder > subPlans.Count)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanOrderInvalid);
            }
            if (currentOrder == newOrder)
            {
                return;
            }

            if (newOrder < currentOrder)
            {
                foreach (var sp in subPlans)
                {
                    if (sp.Order >= newOrder && sp.Order < currentOrder)
                    {
                        sp.Order++;
                    }
                }
            }
            else
            {
                foreach (var sp in subPlans)
                {
                    if (sp.Order <= newOrder && sp.Order > currentOrder)
                    {
                        sp.Order--;
                    }
                }
            }

            subplan.Ten = dto.Ten;
            subplan.MoTa = dto.MoTa;
            subplan.TruongKhoa = dto.TruongKhoa;
            subplan.MoBai = dto.MoBai ?? "";
            subplan.KetBai = dto.KetBai ?? "";
            subplan.Note = dto.Note;
            subplan.Order = dto.NewOrder;
            subplan.IsShow = dto.IsShow;

            _smDbContext.SubPlans.Update(subplan);
            await _smDbContext.SaveChangesAsync();
        }
        public BaseResponsePagingDto<ViewSubPlanDto> FindPaging (FindPagingSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(FindPaging)}, dto = {JsonSerializer.Serialize(dto)}");
            var query = from sp in _smDbContext.SubPlans
                        where !sp.Deleted
                        orderby sp.IdPlan, sp.Order ascending
                        select sp;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewSubPlanDto>>(data);
            return new BaseResponsePagingDto<ViewSubPlanDto>
            {
                TotalItems = query.Count(),
                Items = items
            };
        }
        public void UpdateIsShow (UpdateSubPlanIsShowDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateIsShow)}, dto= {JsonSerializer.Serialize(dto)} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == dto.IdPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            subplan.IsShow = dto.IsShow;
            _smDbContext.SubPlans.Update(subplan);
            _smDbContext.SaveChanges();
        }
        public void Delete (int idPlan, int idSubPlan)
        {
            _logger.LogInformation($"{nameof(Delete)}, idPlan= {idPlan}, idSubPlan= {idSubPlan} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == idPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == idSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            var vietnamNow = GetVietnamTime();
            subplan.Deleted = true;
            subplan.DeletedDate = vietnamNow;
            _smDbContext.SubPlans.Update(subplan);
            _smDbContext.SaveChanges();
        }
        public async Task<List<UpdateOrderSubPlanResponseDto>> MoveOrder (MoveOrderSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(MoveOrder)}, dto= {JsonSerializer.Serialize(dto)} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == dto.IdPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == dto.IdSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            var subPlans = _smDbContext.SubPlans
                .Where(x => x.IdPlan == dto.IdPlan && !x.Deleted)
                .OrderBy(x => x.Order)
                .ToList();
            var movingSubPlan = subPlans.FirstOrDefault(x => x.Id == dto.IdSubPlan);
            if(movingSubPlan == null)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            }
            var currentOrder = movingSubPlan.Order;
            var newOrder = dto.NewOrder;
            if(currentOrder == newOrder)
            {
                return subPlans.Select(x => new UpdateOrderSubPlanResponseDto
                {
                    Id = x.Id,
                    Ten = x.Ten,
                    Order = x.Order
                })
                .OrderBy(x => x.Order)
                .ToList();
            }

            if (newOrder < currentOrder)
            {
                foreach (var sp in subPlans)
                {
                    if (sp.Order >= newOrder && sp.Order < currentOrder)
                    {
                        sp.Order++;
                    }

                }
            }
            else
            {
                foreach (var sp in subPlans)
                {
                    if (sp.Order <= newOrder && sp.Order > currentOrder)
                    {
                        sp.Order--;
                    }
                }
            }
            movingSubPlan.Order = newOrder;
            _smDbContext.SubPlans.UpdateRange(subPlans);
            await _smDbContext.SaveChangesAsync();
            return subPlans.Select(x => new UpdateOrderSubPlanResponseDto
            {
                Id = x.Id,
                Ten = x.Ten,
                Order = x.Order
            })
            .OrderBy(x => x.Order)
            .ToList();

        }
        public BaseResponsePagingDto<ViewSinhVienNhanBangDto> PagingSinhVienNhanBang(FindPagingSinhVienNhanBangDto dto)
        {
            _logger.LogInformation($"{nameof(PagingSinhVienNhanBang)}, dto= {JsonSerializer.Serialize(dto)} ");

            var query = from sv in _smDbContext.DanhSachSinhVienNhanBangs
                        where !sv.Deleted && sv.IdSubPlan == dto.IdSubPlan && sv.TrangThai == TraoBangConstants.ThamGiaTraoBang
                        orderby sv.Order ascending
                        select sv;

            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewSinhVienNhanBangDto>>(data);

            return new BaseResponsePagingDto<ViewSinhVienNhanBangDto>
            {
                TotalItems = query.Count(),
                Items = items
            };
        }
        public async Task<List<GetListSubPlanResponseDto>> GetListSubPlan(int idPlan)
        {
            _logger.LogInformation($"{nameof(GetListSubPlan)}, idPlan= {idPlan} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == idPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subPlans = await _smDbContext.SubPlans
                                .AsNoTracking()
                                .Where(x => x.IdPlan == idPlan && !x.Deleted)
                                .OrderBy(x => x.Order)
                                .Select(x => new GetListSubPlanResponseDto
                                        {
                                             Id = x.Id,
                                             Ten = x.Ten
                                        }) .ToListAsync();

            return subPlans;
        }
        public void CreateSinhVienNhanBang(CreateSinhVienNhanBangDto dto)
        {
            _logger.LogInformation($"{nameof(CreateSinhVienNhanBang)}, dto= {JsonSerializer.Serialize(dto)} ");

            var subPlan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == dto.IdSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var existingSinhVien = _smDbContext.DanhSachSinhVienNhanBangs
                .FirstOrDefault(x => !x.Deleted && x.MaSoSinhVien.ToLower() == dto.MaSoSinhVien.ToLower());

            if (existingSinhVien != null)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienDaTonTai);
            }

            var maxOrder = _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => x.IdSubPlan == dto.IdSubPlan && !x.Deleted)
                .Max(x => (int?)x.Order) ?? 0;

            var vietnamNow = GetVietnamTime();

            var sinhvien = new domain.TraoBang.DanhSachSinhVienNhanBang
            {
                IdSubPlan = dto.IdSubPlan,
                HoVaTen = dto.HoVaTen,
                MaSoSinhVien = dto.MaSoSinhVien,
                Lop = dto.Lop,
                NgaySinh = dto.NgaySinh,
                CapBang = dto.CapBang,
                TenNganhDaoTao = dto.TenNganhDaoTao,
                XepHang = dto.XepHang,
                ThanhTich = dto.ThanhTich,
                Email = dto.Email,
                EmailSinhVien = $"{dto.MaSoSinhVien}@st.huce.edu.vn",
                KhoaQuanLy = dto.KhoaQuanLy,
                SoQuyetDinhTotNghiep = dto.SoQuyetDinhTotNghiep,
                NgayQuyetDinh = dto.NgayQuyetDinh,
                Note = dto.Note,
                IsShow = true,
                Order = maxOrder + 1,
                TrangThai = dto.TrangThai,
                LinkQR = dto.LinkQR,
                CreatedDate = vietnamNow,
                Deleted = false
            };

            _smDbContext.DanhSachSinhVienNhanBangs.Add(sinhvien);
            _smDbContext.SaveChanges();
        }

        public async Task UpdateSinhVienNhanBang(UpdateSinhVienNhanBangDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateSinhVienNhanBang)}, dto= {JsonSerializer.Serialize(dto)} ");

            var subPlan = await _smDbContext.SubPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.IdSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var sinhVien = await _smDbContext.DanhSachSinhVienNhanBangs
                .FirstOrDefaultAsync(x => !x.Deleted && x.Id == dto.Id)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);

            if (!string.Equals(sinhVien.MaSoSinhVien, dto.MaSoSinhVien, StringComparison.OrdinalIgnoreCase))
            {
                var existingSinhVien = await _smDbContext.DanhSachSinhVienNhanBangs
                    .FirstOrDefaultAsync(x => !x.Deleted && x.MaSoSinhVien.ToLower() == dto.MaSoSinhVien.ToLower());

                if (existingSinhVien != null)
                {
                    throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienDaTonTai);
                }
            }

            var danhSachSinhViens = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => x.IdSubPlan == dto.IdSubPlan && !x.Deleted)
                .OrderBy(x => x.Order)
                .ToListAsync();

            var currentOrder = sinhVien.Order;
            var newOrder = dto.Order;

            if (newOrder < 1 || newOrder > danhSachSinhViens.Count)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienOrderInvalid);
            }

            if (currentOrder != newOrder)
            {
                if (newOrder < currentOrder)
                {
                    foreach (var sv in danhSachSinhViens)
                    {
                        if (sv.Order >= newOrder && sv.Order < currentOrder)
                        {
                            sv.Order++;
                        }
                    }
                }
                else
                {
                    foreach (var sv in danhSachSinhViens)
                    {
                        if (sv.Order <= newOrder && sv.Order > currentOrder)
                        {
                            sv.Order--;
                        }
                    }
                }
            }

            sinhVien.HoVaTen = dto.HoVaTen;
            sinhVien.MaSoSinhVien = dto.MaSoSinhVien;
            sinhVien.Lop = dto.Lop;
            sinhVien.NgaySinh = dto.NgaySinh;
            sinhVien.CapBang = dto.CapBang;
            sinhVien.TenNganhDaoTao = dto.TenNganhDaoTao;
            sinhVien.XepHang = dto.XepHang;
            sinhVien.ThanhTich = dto.ThanhTich;
            sinhVien.Email = dto.Email;
            sinhVien.EmailSinhVien = $"{dto.MaSoSinhVien}@st.huce.edu.vn";
            sinhVien.KhoaQuanLy = dto.KhoaQuanLy;
            sinhVien.SoQuyetDinhTotNghiep = dto.SoQuyetDinhTotNghiep;
            sinhVien.NgayQuyetDinh = dto.NgayQuyetDinh;
            sinhVien.Note = dto.Note;
            sinhVien.TrangThai = dto.TrangThai;
            sinhVien.LinkQR = dto.LinkQR;
            sinhVien.IsShow = dto.IsShow;
            sinhVien.Order = newOrder;

            _smDbContext.DanhSachSinhVienNhanBangs.Update(sinhVien);
            await _smDbContext.SaveChangesAsync();
        }
        public void DeleteSinhVienNhanBang (int idSubPlan, int id)
        {
            _logger.LogInformation($"{nameof(DeleteSinhVienNhanBang)}, idSubPlan= {idSubPlan}, id= {id} ");
            var subPlan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == idSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            var sinhVien = _smDbContext.DanhSachSinhVienNhanBangs.FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);
            var vietnamNow = GetVietnamTime();
            sinhVien.Deleted = true;
            sinhVien.DeletedDate = vietnamNow;
            _smDbContext.DanhSachSinhVienNhanBangs.Update(sinhVien);
            _smDbContext.SaveChanges();
        }
        public async Task<ViewSinhVienNhanBangDto> ShowSinhVienNhanBangInfor(string mssv)
        {
            _logger.LogInformation($"{nameof(ShowSinhVienNhanBangInfor)}, mssv= {mssv} ");

            var sinhVien = await _smDbContext.DanhSachSinhVienNhanBangs
                .FirstOrDefaultAsync(x => !x.Deleted && x.MaSoSinhVien.ToLower() == mssv.ToLower() && x.TrangThai == TraoBangConstants.ThamGiaTraoBang)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);

            var subPlan = await _smDbContext.SubPlans
                .FirstOrDefaultAsync(x => x.Id == sinhVien.IdSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var maxOrder = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => x.IdSubPlan == sinhVien.IdSubPlan && !x.Deleted)
                .MaxAsync(x => (int?)x.Order) ?? 0;

            var minOrder = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => x.IdSubPlan == sinhVien.IdSubPlan && !x.Deleted)
                .MinAsync(x => (int?)x.Order) ?? 0;

            var totalSubPlans = await _smDbContext.SubPlans
                .Where(x => x.IdPlan == subPlan.IdPlan && !x.Deleted)
                .CountAsync();

            var result = _mapper.Map<ViewSinhVienNhanBangDto>(sinhVien);
            result.OrderSubPlan = $"{subPlan.Order}/{totalSubPlans}";
            result.Order = $"{sinhVien.Order}/{maxOrder}";
            result.IsShowNext = sinhVien.Order < maxOrder;
            result.IsShowPrev = sinhVien.Order > minOrder;

            return result;
        }

        public async Task<ViewSinhVienNhanBangDto> NextSinhVienNhanBang(string mssv)
        {
            _logger.LogInformation($"{nameof(NextSinhVienNhanBang)}, mssv= {mssv} ");

            var currentSinhVien = await _smDbContext.DanhSachSinhVienNhanBangs
                .FirstOrDefaultAsync(x => !x.Deleted && x.MaSoSinhVien.ToLower() == mssv.ToLower() && x.TrangThai == TraoBangConstants.ThamGiaTraoBang)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);

            var nextSinhVien = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => !x.Deleted && x.IdSubPlan == currentSinhVien.IdSubPlan && x.Order > currentSinhVien.Order && x.TrangThai == TraoBangConstants.ThamGiaTraoBang)
                .OrderBy(x => x.Order)
                .FirstOrDefaultAsync();

            if (nextSinhVien == null)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);
            }

            var subPlan = await _smDbContext.SubPlans
                .FirstOrDefaultAsync(x => x.Id == nextSinhVien.IdSubPlan && !x.Deleted )
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var maxOrder = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => x.IdSubPlan == nextSinhVien.IdSubPlan && !x.Deleted && x.TrangThai == TraoBangConstants.ThamGiaTraoBang)
                .MaxAsync(x => (int?)x.Order) ?? 0;

            var minOrder = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => x.IdSubPlan == nextSinhVien.IdSubPlan && !x.Deleted && x.TrangThai == TraoBangConstants.ThamGiaTraoBang)
                .MinAsync(x => (int?)x.Order) ?? 0;

            var totalSubPlans = await _smDbContext.SubPlans
                .Where(x => x.IdPlan == subPlan.IdPlan && !x.Deleted)
                .CountAsync();

            var result = _mapper.Map<ViewSinhVienNhanBangDto>(nextSinhVien);
            result.OrderSubPlan = $"{subPlan.Order}/{totalSubPlans}";
            result.Order = $"{nextSinhVien.Order}/{maxOrder}";
            result.IsShowNext = nextSinhVien.Order < maxOrder;
            result.IsShowPrev = nextSinhVien.Order > minOrder;

            return result;
        }

        public async Task<ViewSinhVienNhanBangDto> PreviousSinhVienNhanBang(string mssv)
        {
            _logger.LogInformation($"{nameof(PreviousSinhVienNhanBang)}, mssv= {mssv} ");

            var currentSinhVien = await _smDbContext.DanhSachSinhVienNhanBangs
                .FirstOrDefaultAsync(x => !x.Deleted && x.MaSoSinhVien.ToLower() == mssv.ToLower() && x.TrangThai == TraoBangConstants.ThamGiaTraoBang)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);

            var previousSinhVien = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => !x.Deleted && x.IdSubPlan == currentSinhVien.IdSubPlan && x.Order < currentSinhVien.Order && x.TrangThai == TraoBangConstants.ThamGiaTraoBang)
                .OrderByDescending(x => x.Order)
                .FirstOrDefaultAsync();

            if (previousSinhVien == null)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);
            }

            var subPlan = await _smDbContext.SubPlans
                .FirstOrDefaultAsync(x => x.Id == previousSinhVien.IdSubPlan && !x.Deleted )
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var maxOrder = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => x.IdSubPlan == previousSinhVien.IdSubPlan && !x.Deleted && x.TrangThai == TraoBangConstants.ThamGiaTraoBang)
                .MaxAsync(x => (int?)x.Order) ?? 0;

            var minOrder = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => x.IdSubPlan == previousSinhVien.IdSubPlan && !x.Deleted && x.TrangThai == TraoBangConstants.ThamGiaTraoBang)
                .MinAsync(x => (int?)x.Order) ?? 0;

            var totalSubPlans = await _smDbContext.SubPlans
                .Where(x => x.IdPlan == subPlan.IdPlan && !x.Deleted)
                .CountAsync();

            var result = _mapper.Map<ViewSinhVienNhanBangDto>(previousSinhVien);
            result.OrderSubPlan = $"{subPlan.Order}/{totalSubPlans}";
            result.Order = $"{previousSinhVien.Order}/{maxOrder}";
            result.IsShowNext = previousSinhVien.Order < maxOrder;
            result.IsShowPrev = previousSinhVien.Order > minOrder;

            return result;
        }
        public async Task<DiemDanhNhanBangDto> DiemDanhNhanBang(string mssv)
        {
            _logger.LogInformation($"{nameof(DiemDanhNhanBang)}, mssv= {mssv} ");
            var sinhVien = _smDbContext.DanhSachSinhVienNhanBangs
                .FirstOrDefault(x => !x.Deleted && x.MaSoSinhVien.ToLower() == mssv.ToLower())
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);
            var maxOrder = _smDbContext.TienDoTraoBangs
                .Where(x => x.IdSubPlan == sinhVien.IdSubPlan && !x.Deleted)
                .Max(x => (int?)x.Order) ?? 0;
            var mssvexisting = _smDbContext.TienDoTraoBangs.Any(x => !x.Deleted && x.MaSoSinhVien.ToLower() == mssv.ToLower());
            if (mssvexisting)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienDaTonTaiTrongHangDoi);
            }
            var subPlan = _smDbContext.SubPlans
                .FirstOrDefault(x => x.Id == sinhVien.IdSubPlan && !x.Deleted);
            var tienDoTraoBang = new TienDoTraoBang
            {
                IdSubPlan = sinhVien.IdSubPlan,
                IdSinhVienNhanBang = sinhVien.Id,
                HoVaTen = sinhVien.HoVaTen,
                MaSoSinhVien = sinhVien.MaSoSinhVien,
                TrangThai = TraoBangConstants.ChuanBi,
                Order = maxOrder + 1,
                IsShow = true,
                CreatedDate = DateTime.Now,
                Deleted = false
            };
            _smDbContext.TienDoTraoBangs.Add(tienDoTraoBang);
            _smDbContext.SaveChanges();
            await _traoBangService.NotifyCheckIn(mssv);
            return new DiemDanhNhanBangDto
            {
                TenKhoa = subPlan?.Ten ?? String.Empty,
                Id = sinhVien.Id,
                HoVaTen = sinhVien.HoVaTen,
                MaSoSinhVien = sinhVien.MaSoSinhVien,
                TrangThai = TraoBangConstants.ChuanBi,
                Order = maxOrder + 1,
                IsShow = true
            };
        }
        public async Task UpdateTrangThaiSinhVienNhanBang(int idSubPlan, int id)
        {
            _logger.LogInformation($"{nameof(UpdateTrangThaiSinhVienNhanBang)} ");
            var subPlan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == idSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            var sinhVien = _smDbContext.TienDoTraoBangs.FirstOrDefault(x => x.Id == id && x.IdSubPlan == idSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);
            sinhVien.TrangThai = TraoBangConstants.DangTraoBang;
            _smDbContext.TienDoTraoBangs.Update(sinhVien);
            _smDbContext.SaveChanges();
            await _traoBangService.NotifySinhVienDangTrao(idSubPlan, id);
             _logger.LogInformation($"Đã bắn SignalR cho sinh viên Id: {id}, SubPlan: {idSubPlan}");
        }
        public async Task<GetSinhVienDangTraoBangInforDto> GetSinhVienDangTraoBang()
        {
            _logger.LogInformation($"{nameof(GetSinhVienDangTraoBang)} ");

            var subPlan = await _smDbContext.SubPlans
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x => x.TrangThai == TraoBangConstants.DangTraoBang && !x.Deleted)
                   ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var tienDo = await _smDbContext.TienDoTraoBangs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdSubPlan == subPlan.Id && x.TrangThai == TraoBangConstants.DangTraoBang && !x.Deleted);
            if (tienDo == null) {
                return null;
            }
            var sinhVien = await _smDbContext.DanhSachSinhVienNhanBangs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == tienDo.IdSinhVienNhanBang && x.IdSubPlan == subPlan.Id && !x.Deleted);
            if (sinhVien == null)
            {
                return null;
            }

            return new GetSinhVienDangTraoBangInforDto
            {
                TenSubPlan = subPlan.Ten,
                Id = sinhVien.Id,
                HoVaTen = sinhVien.HoVaTen,
                MaSoSinhVien = sinhVien.MaSoSinhVien,
                TenNganhDaoTao = sinhVien.TenNganhDaoTao,
                XepHang = sinhVien.XepHang,
                ThanhTich = sinhVien.ThanhTich,
                CapBang = sinhVien.CapBang,
                Note = sinhVien.Note ?? "",
            };
        }
        public async Task UpdateTrangThaiSubPlan(int id)
        {
            _logger.LogInformation($"{nameof(UpdateTrangThaiSubPlan)}");

            var subPlan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var currentSubPlans = _smDbContext.SubPlans
                .Where(x => x.TrangThai == TraoBangConstants.DangTraoBang && x.Id != id && !x.Deleted)
                .ToList();

            foreach (var current in currentSubPlans)
            {
                current.TrangThai = TraoBangConstants.ChuanBi;
                _smDbContext.SubPlans.Update(current);
            }

            subPlan.TrangThai = TraoBangConstants.DangTraoBang;
            _smDbContext.SubPlans.Update(subPlan);

            _smDbContext.SaveChanges();
            await _traoBangService.NotifyChonKhoa(id);
        }
        public async Task<List<ViewTienDoNhanBangResponseDto>> GetTienDoNhanBang(ViewTienDoNhanBangRequestDto dto)
        {
            _logger.LogInformation($"{nameof(GetTienDoNhanBang)}, dto= {JsonSerializer.Serialize(dto)} ");
            var khoaDangTrao = _smDbContext.SubPlans
                .AsNoTracking()
                .FirstOrDefault(x => x.TrangThai == TraoBangConstants.DangTraoBang && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            var sinhVien = _smDbContext.DanhSachSinhVienNhanBangs
                .AsNoTracking()
                .FirstOrDefault(x => !x.Deleted && x.IdSubPlan == khoaDangTrao.Id)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSinhVienNotFound);

            var results = new List<TienDoTraoBang>();

            var sinhVienDaTrao = await _smDbContext.TienDoTraoBangs
                .AsNoTracking()
                .Where(x => !x.Deleted
                            && x.IdSubPlan == khoaDangTrao.Id
                            && x.TrangThai == TraoBangConstants.DaTraoBang)
                .OrderByDescending(x => x.Order)
                .FirstOrDefaultAsync();

            if (sinhVienDaTrao != null)
            {
                results.Add(sinhVienDaTrao);
            }

            var soLuongConLai = dto.SoLuong - results.Count;
            if (soLuongConLai > 0)
            {
                var sinhVienChuanBi = await _smDbContext.TienDoTraoBangs
                    .AsNoTracking()
                    .Where(x => !x.Deleted
                                && x.IdSubPlan == khoaDangTrao.Id
                                && x.TrangThai != TraoBangConstants.DaTraoBang)
                    .OrderBy(x => x.Order)
                    .Take(soLuongConLai)
                    .ToListAsync();

                results.AddRange(sinhVienChuanBi);
            }

            if (!results.Any())
            {
                return null;
            }

            return results.Select(result => new ViewTienDoNhanBangResponseDto
            {
                Id = result.Id,
                HoVaTen = result.HoVaTen,
                MaSoSinhVien = result.MaSoSinhVien,
                CapBang = sinhVien.CapBang,
                TenNganhDaoTao = sinhVien.TenNganhDaoTao,
                TrangThai = result.TrangThai,
                Note = sinhVien.Note ?? "",
                Order = result.Order,
                IsShow = result.IsShow
            }).ToList();
        }
        public async Task<GetNextSubPlanResponseDto?> NextSubPlan()
        {
            _logger.LogInformation($"{nameof(NextSubPlan)}");

            var currentSubPlan = _smDbContext.SubPlans
                .FirstOrDefault(x => x.TrangThai == TraoBangConstants.DangTraoBang && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            currentSubPlan.TrangThai = TraoBangConstants.DaTraoBang;
            _smDbContext.SubPlans.Update(currentSubPlan);

            var nextSubPlan = _smDbContext.SubPlans
                .Where(x => x.TrangThai != TraoBangConstants.DaTraoBang && x.Id != currentSubPlan.Id && !x.Deleted)
                .OrderBy(x => x.Order)
                .FirstOrDefault();

            if (nextSubPlan != null)
            {
                nextSubPlan.TrangThai = TraoBangConstants.DangTraoBang;
                _smDbContext.SubPlans.Update(nextSubPlan);
            }

            _smDbContext.SaveChanges();
            await _traoBangService.NotifyChuyenKhoa();
            return nextSubPlan != null ? new GetNextSubPlanResponseDto
            {
                Id = nextSubPlan.Id,
                TenSubPlan = nextSubPlan.Ten,
                TruongKhoa = nextSubPlan.TruongKhoa,
                Order = nextSubPlan.Order,
                TrangThai = nextSubPlan.TrangThai
            } : null;
        }

        public async Task<List<GetListSubPlanDto>> GetListSubPlanInfor(int idPlan)
        {
            _logger.LogInformation($"{nameof(GetListSubPlanInfor)}, idPlan= {idPlan}");

            var subPlans = await _smDbContext.SubPlans
                .AsNoTracking()
                .Where(x => x.IdPlan == idPlan && !x.Deleted)
                .OrderByDescending(x => x.TrangThai == TraoBangConstants.DaTraoBang)
                .ThenByDescending(x => x.TrangThai == TraoBangConstants.DangTraoBang)
                .ThenByDescending(x => x.TrangThai == TraoBangConstants.ChuanBi)
                .ThenBy(x => x.Order)
                .ToListAsync();

            var items = new List<GetListSubPlanDto>();

            foreach (var subPlan in subPlans)
            {
                var daTrao = await _smDbContext.TienDoTraoBangs
                    .AsNoTracking()
                    .CountAsync(x => x.IdSubPlan == subPlan.Id && !x.Deleted && x.TrangThai == TraoBangConstants.DaTraoBang);

                var tongSo = await _smDbContext.DanhSachSinhVienNhanBangs
                    .AsNoTracking()
                    .CountAsync(x => x.IdSubPlan == subPlan.Id && !x.Deleted && x.TrangThai == TraoBangConstants.ThamGiaTraoBang);

                var tienDo = tongSo > 0 ? $"{daTrao}/{tongSo}" : "0/0";

                items.Add(new GetListSubPlanDto
                {
                    Id = subPlan.Id,
                    Ten = subPlan.Ten,
                    TrangThai = subPlan.TrangThai,
                    Order = subPlan.Order,
                    TienDo = tienDo
                });
            }

            return items;
        }
        public async Task<GetInforSubPlanDto> GetInforSubPlan(int idSubPlan)
        {
            _logger.LogInformation($"{nameof(GetInforSubPlan)}, idSubPlan= {idSubPlan} ");
            var subPlan = await _smDbContext.SubPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var soLuongThamGia = await _smDbContext.DanhSachSinhVienNhanBangs
                .AsNoTracking()
                .CountAsync(x => x.IdSubPlan == idSubPlan && !x.Deleted && x.TrangThai == TraoBangConstants.ThamGiaTraoBang);

            var soLuongVangMat = await _smDbContext.DanhSachSinhVienNhanBangs
                .AsNoTracking()
                .CountAsync(x => x.IdSubPlan == idSubPlan && !x.Deleted && x.TrangThai == TraoBangConstants.VangMat);

            var soLuongDaTrao = await _smDbContext.TienDoTraoBangs
                .AsNoTracking()
                .CountAsync(x => x.IdSubPlan == idSubPlan && !x.Deleted && x.TrangThai == TraoBangConstants.DaTraoBang);

            var soLuongConLai = await _smDbContext.TienDoTraoBangs
                .AsNoTracking()
                .CountAsync(x => x.IdSubPlan == idSubPlan && !x.Deleted && x.TrangThai == TraoBangConstants.ChuanBi);
           
           
            return new GetInforSubPlanDto
            {
                Ten = subPlan.Ten,
                SoLuongThamGia = soLuongThamGia,
                SoLuongVangMat = soLuongVangMat,
                SoLuongDaTrao = soLuongDaTrao,
                SoLuongConLai = soLuongConLai
            };
           
        }
        

        //Get số sinh viên đã trao/ tổng số sinh viên tham dự
        public async Task<GetTienDoTraoBangResponseDto> GetTienDoTraoBang()
        {
            var sinhVienDaTrao = await _smDbContext.TienDoTraoBangs
                .AsNoTracking()
                .CountAsync(x => x.TrangThai == TraoBangConstants.DaTraoBang && !x.Deleted);
            var tongSinhVienThamGiaTraoBang = await _smDbContext.DanhSachSinhVienNhanBangs
                .AsNoTracking()
                .CountAsync(x => x.TrangThai == TraoBangConstants.ThamGiaTraoBang && !x.Deleted);
            return new GetTienDoTraoBangResponseDto
            {
                TienDo = $"{sinhVienDaTrao}/{tongSinhVienThamGiaTraoBang}"
            };
        }

        //Next sinh viên trao bằng 
        public async Task<GetSinhVienDangTraoBangInforDto> NextSinhVienTraoBang(int idSubPlan)
        {
            _logger.LogInformation($"{nameof(NextSinhVienTraoBang)}");

            var sinhVienDangTrao = await _smDbContext.TienDoTraoBangs
                .FirstOrDefaultAsync(x => x.IdSubPlan == idSubPlan
                            && x.TrangThai == TraoBangConstants.DangTraoBang
                            && !x.Deleted);

            if (sinhVienDangTrao == null)
            {
                var sinhVienDauTien = await _smDbContext.TienDoTraoBangs
                    .Where(x => x.IdSubPlan == idSubPlan
                                && x.TrangThai == TraoBangConstants.ChuanBi
                                && !x.Deleted)
                    .OrderBy(x => x.Order)
                    .FirstOrDefaultAsync();

                if (sinhVienDauTien == null)
                {
                    return null;
                }

                sinhVienDauTien.TrangThai = TraoBangConstants.DangTraoBang;
                _smDbContext.TienDoTraoBangs.Update(sinhVienDauTien);
                await _smDbContext.SaveChangesAsync();

                var sinhVienInfo = await _smDbContext.DanhSachSinhVienNhanBangs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == sinhVienDauTien.IdSinhVienNhanBang && !x.Deleted);

                var subPlan = await _smDbContext.SubPlans
                    .AsNoTracking()

                    .FirstOrDefaultAsync(x => x.Id == idSubPlan && !x.Deleted);

                await _traoBangService.NotifySinhVienDangTrao(idSubPlan, sinhVienDauTien.Id);
              
                return new GetSinhVienDangTraoBangInforDto
                {
                    TenSubPlan = subPlan?.Ten ?? string.Empty,
                    Id = sinhVienDauTien.Id,
                    HoVaTen = sinhVienInfo?.HoVaTen ?? string.Empty,
                    MaSoSinhVien = sinhVienInfo?.MaSoSinhVien ?? string.Empty,
                    TenNganhDaoTao = sinhVienInfo?.TenNganhDaoTao ?? string.Empty,
                    XepHang = sinhVienInfo?.XepHang ?? string.Empty,
                    ThanhTich = sinhVienInfo?.ThanhTich ?? string.Empty,
                    CapBang = sinhVienInfo?.CapBang ?? string.Empty,
                    Note = sinhVienInfo?.Note ?? string.Empty
                };
            }

            sinhVienDangTrao.TrangThai = TraoBangConstants.DaTraoBang;
            _smDbContext.TienDoTraoBangs.Update(sinhVienDangTrao);
            await _smDbContext.SaveChangesAsync();

            var sinhVienTiepTheo = await _smDbContext.TienDoTraoBangs
                .Where(x => x.IdSubPlan == idSubPlan
                            && x.TrangThai == TraoBangConstants.ChuanBi
                            && !x.Deleted)
                .OrderBy(x => x.Order)
                .FirstOrDefaultAsync();

            if (sinhVienTiepTheo == null)
            {
                return null;
            }

            sinhVienTiepTheo.TrangThai = TraoBangConstants.DangTraoBang;
            _smDbContext.TienDoTraoBangs.Update(sinhVienTiepTheo);
            await _smDbContext.SaveChangesAsync();

            var sinhVienInfoTiepTheo = await _smDbContext.DanhSachSinhVienNhanBangs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == sinhVienTiepTheo.IdSinhVienNhanBang && !x.Deleted);

            var subPlanInfo = await _smDbContext.SubPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idSubPlan && !x.Deleted);

            await _traoBangService.NotifySinhVienDangTrao(idSubPlan, sinhVienTiepTheo.Id);
          

            return new GetSinhVienDangTraoBangInforDto
            {
                TenSubPlan = subPlanInfo?.Ten ?? string.Empty,
                Id = sinhVienTiepTheo.Id,
                HoVaTen = sinhVienInfoTiepTheo?.HoVaTen ?? string.Empty,
                MaSoSinhVien = sinhVienInfoTiepTheo?.MaSoSinhVien ?? string.Empty,
                TenNganhDaoTao = sinhVienInfoTiepTheo?.TenNganhDaoTao ?? string.Empty,
                XepHang = sinhVienInfoTiepTheo?.XepHang ?? string.Empty,
                ThanhTich = sinhVienInfoTiepTheo?.ThanhTich ?? string.Empty,
                CapBang = sinhVienInfoTiepTheo?.CapBang ?? string.Empty,
                Note = sinhVienInfoTiepTheo?.Note ?? string.Empty
            };
        }
        //Prev sinh viên trao bằng 
        public async Task<GetSinhVienDangTraoBangInforDto?> PrevSinhVienTraoBang(int idSubPlan)
        {
            _logger.LogInformation($"{nameof(PrevSinhVienTraoBang)}");
            var sinhVienDangTrao = await _smDbContext.TienDoTraoBangs
                .FirstOrDefaultAsync(x => x.IdSubPlan == idSubPlan
                            && x.TrangThai == TraoBangConstants.DangTraoBang
                            && !x.Deleted);
            if (sinhVienDangTrao == null)
            {
                return null;
            }
            sinhVienDangTrao.TrangThai = TraoBangConstants.ChuanBi;
            _smDbContext.TienDoTraoBangs.Update(sinhVienDangTrao);
            await _smDbContext.SaveChangesAsync();
            var sinhVienTruocDo = await _smDbContext.TienDoTraoBangs
                .Where(x => x.IdSubPlan == idSubPlan
                            && x.TrangThai == TraoBangConstants.DaTraoBang
                            && !x.Deleted)
                .OrderByDescending(x => x.Order)
                .FirstOrDefaultAsync();
            if (sinhVienTruocDo == null)
            {
                return null;
            }
            sinhVienTruocDo.TrangThai = TraoBangConstants.DangTraoBang;
            _smDbContext.TienDoTraoBangs.Update(sinhVienTruocDo);
            await _smDbContext.SaveChangesAsync();
            var sinhVienInfoTiepTheo = await _smDbContext.DanhSachSinhVienNhanBangs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == sinhVienTruocDo.IdSinhVienNhanBang && !x.Deleted);
            var subPlanInfo = await _smDbContext.SubPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idSubPlan && !x.Deleted);
            await _traoBangService.NotifySinhVienDangTrao(idSubPlan, sinhVienTruocDo.Id);

            return new GetSinhVienDangTraoBangInforDto
            {
                TenSubPlan = subPlanInfo?.Ten ?? string.Empty,
                Id = sinhVienTruocDo.Id,
                HoVaTen = sinhVienInfoTiepTheo?.HoVaTen ?? string.Empty,
                MaSoSinhVien = sinhVienInfoTiepTheo?.MaSoSinhVien ?? string.Empty,
                TenNganhDaoTao = sinhVienInfoTiepTheo?.TenNganhDaoTao ?? string.Empty,
                XepHang = sinhVienInfoTiepTheo?.XepHang ?? string.Empty,
                ThanhTich = sinhVienInfoTiepTheo?.ThanhTich ?? string.Empty,
                CapBang = sinhVienInfoTiepTheo?.CapBang ?? string.Empty,
                Note = sinhVienInfoTiepTheo?.Note ?? string.Empty
            };
        }
        // GetInFor sinh viên được trao bằng tiếp theo 
        public async Task<GetInforSinhVienChuanBiDuocTraoBangResponseDto?> GetInforSinhVienChuanBiDuocTraoBang(int idSubPlan)
        {
            _logger.LogInformation($"{nameof(GetInforSinhVienChuanBiDuocTraoBang)}, idSubPlan= {idSubPlan} ");
            var subPlan = await _smDbContext.SubPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);

            var sinhVienTiepTheo = await _smDbContext.TienDoTraoBangs
                .Where(x => x.IdSubPlan == idSubPlan
                            && x.TrangThai == TraoBangConstants.ChuanBi
                            && !x.Deleted)
                .OrderBy(x => x.Order)
                .FirstOrDefaultAsync();

            if (sinhVienTiepTheo == null)
            {
                return null;
            }

            var sinhVienInfor = await _smDbContext.DanhSachSinhVienNhanBangs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == sinhVienTiepTheo.IdSinhVienNhanBang && !x.Deleted);

            if (sinhVienInfor == null)
            {
                return null;
            }

            return new GetInforSinhVienChuanBiDuocTraoBangResponseDto
            {
                TenSubPlan = subPlan?.Ten ?? string.Empty,
                Id = sinhVienInfor.Id,
                HoVaTen = sinhVienInfor?.HoVaTen ?? string.Empty,
                MaSoSinhVien = sinhVienInfor?.MaSoSinhVien ?? string.Empty,
                TenNganhDaoTao = sinhVienInfor?.TenNganhDaoTao ?? string.Empty,
                XepHang = sinhVienInfor?.XepHang ?? string.Empty,
                ThanhTich = sinhVienInfor?.ThanhTich ?? string.Empty,
                CapBang = sinhVienInfor?.CapBang ?? string.Empty,
                Note = sinhVienInfor?.Note ?? string.Empty
            };
        }
        public async Task<ImportDanhSachSinhVienNhanBangResponseDto> ImportDanhSachNhanBang(ImportDanhSachSinhVienNhanBangDto dto)
        {
            _logger.LogInformation($"{nameof(ImportDanhSachNhanBang)}, dto= {JsonSerializer.Serialize(dto)} ");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var sheetData = await _getSheetData(dto.Url, dto.SheetName);

            var headers = sheetData[0];
            var dataRows = sheetData.Skip(1).ToList();

            // Map header indices
            var colIndexMap = new Dictionary<string, int>();
            for (int i = 0; i < headers.Count; i++)
            {
                var header = headers[i].Trim();
                colIndexMap[header] = i;
            }

            // Validate required headers
            var requiredHeaders = new[] { "Thứ tự Khoa", "STT", "MSSV", "Họ và tên", "Lớp", "Ngày sinh",
                           "Cấp bằng", "Tên ngành đào tạo", "Xếp hạng", "Thành tích",
                           "email", "Khoa Quản lý", "Số quyết định tốt nghiệp",
                           "Ngày quyết định", "Status", "Link QR minio" };

            foreach (var header in requiredHeaders)
            {
                if (!colIndexMap.ContainsKey(header))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid);
                }
            }

            var existingMSSVs = await _smDbContext.DanhSachSinhVienNhanBangs
                .Where(x => !x.Deleted)
                .Select(x => x.MaSoSinhVien)
                .ToListAsync();

            var existingMSSVSet = new HashSet<string>(existingMSSVs, StringComparer.OrdinalIgnoreCase);
            var processedMSSVs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var danhSachList = new List<domain.TraoBang.DanhSachSinhVienNhanBang>();
            var vietnamNow = GetVietnamTime();
            int totalDataImported = 0;

            foreach (var row in dataRows)
            {
                if (row.Count == 0 || row.All(c => string.IsNullOrWhiteSpace(c)))
                {
                    continue;
                }

                try
                {
                    var mssv = GetCellValue(row, colIndexMap, "MSSV");

                    if (string.IsNullOrWhiteSpace(mssv))
                    {
                        continue;
                    }

                    if (existingMSSVSet.Contains(mssv))
                    {
                        _logger.LogWarning($"MSSV {mssv} đã tồn tại trong DB, bỏ qua");
                        continue;
                    }

                    if (processedMSSVs.Contains(mssv))
                    {
                        _logger.LogWarning($"MSSV {mssv} bị trùng trong file import, bỏ qua");
                        continue;
                    }

                    processedMSSVs.Add(mssv);

                    var idSubPlanStr = GetCellValue(row, colIndexMap, "Thứ tự Khoa");
                    var orderStr = GetCellValue(row, colIndexMap, "STT");

                    var idSubPlan = string.IsNullOrWhiteSpace(idSubPlanStr) ? 0 : int.Parse(idSubPlanStr);
                    var order = string.IsNullOrWhiteSpace(orderStr) ? 0 : int.Parse(orderStr);

                    var hoVaTen = GetCellValue(row, colIndexMap, "Họ và tên");
                    var lop = GetCellValue(row, colIndexMap, "Lớp");
                    var ngaySinhStr = GetCellValue(row, colIndexMap, "Ngày sinh");
                    var capBang = GetCellValue(row, colIndexMap, "Cấp bằng");
                    var tenNganhDaoTao = GetCellValue(row, colIndexMap, "Tên ngành đào tạo");
                    var xepHang = GetCellValue(row, colIndexMap, "Xếp hạng");
                    var thanhTich = GetCellValue(row, colIndexMap, "Thành tích");
                    var email = GetCellValue(row, colIndexMap, "email");
                    var khoaQuanLy = GetCellValue(row, colIndexMap, "Khoa Quản lý");
                    var soQuyetDinh = GetCellValue(row, colIndexMap, "Số quyết định tốt nghiệp");
                    var ngayQuyetDinhStr = GetCellValue(row, colIndexMap, "Ngày quyết định");
                    var statusStr = GetCellValue(row, colIndexMap, "Status");
                    var noteStr = GetCellValue(row, colIndexMap, "Note cho MC");
                    var linkQR = GetCellValue(row, colIndexMap, "Link QR minio");

                    var ngaySinh = ParseDate(ngaySinhStr);
                    var ngayQuyetDinh = ParseDate(ngayQuyetDinhStr);
                    var trangThai = MapStatus(statusStr);
                    var emailSinhVien = $"{mssv}@st.huce.edu.vn";

                    var danhSach = new domain.TraoBang.DanhSachSinhVienNhanBang
                    {
                        IdSubPlan = idSubPlan,
                        Order = order,
                        MaSoSinhVien = mssv,
                        HoVaTen = hoVaTen,
                        Lop = lop,
                        NgaySinh = ngaySinh,
                        CapBang = capBang,
                        TenNganhDaoTao = tenNganhDaoTao,
                        XepHang = xepHang,
                        ThanhTich = thanhTich,
                        Email = email,
                        EmailSinhVien = emailSinhVien,
                        KhoaQuanLy = khoaQuanLy,
                        SoQuyetDinhTotNghiep = soQuyetDinh,
                        NgayQuyetDinh = ngayQuyetDinh,
                        TrangThai = trangThai,
                        Note = noteStr,
                        LinkQR = linkQR,
                        IsShow = true,
                        CreatedDate = vietnamNow,
                        Deleted = false
                    };

                    danhSachList.Add(danhSach);
                    totalDataImported++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Lỗi khi xử lý dòng: {string.Join(", ", row)}. Error: {ex.Message}");
                    continue;
                }
            }

            if (danhSachList.Any())
            {
                await _smDbContext.BulkInsertAsync(danhSachList);
            }

            _logger.LogInformation($"Tổng số dòng đã insert: {totalDataImported}");

            stopwatch.Stop();

            return new ImportDanhSachSinhVienNhanBangResponseDto
            {
                TotalRowsImported = dataRows.Count,
                TotalDataImported = totalDataImported,
                ImportTimeInSeconds = (int)stopwatch.Elapsed.TotalSeconds
            };
        }

        private string GetCellValue(List<string> row, Dictionary<string, int> colIndexMap, string headerName)
        {
            if (!colIndexMap.ContainsKey(headerName))
            {
                return string.Empty;
            }

            var index = colIndexMap[headerName];
            if (index >= row.Count)
            {
                return string.Empty;
            }

            return row[index]?.Trim() ?? string.Empty;
        }

        private DateTime ParseDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
            {
                return DateTime.MinValue;
            }

            if (DateTime.TryParse(dateStr, out var date))
            {
                return date;
            }

            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dateExact))
            {
                return dateExact;
            }

            return DateTime.MinValue;
        }

        private int MapStatus(string statusStr)
        {
            if (string.IsNullOrWhiteSpace(statusStr))
            {
                return TraoBangConstants.ChuanBi;
            }

            var status = statusStr.Trim().ToLower();

            if (status.Contains("vắng") || status.Contains("vang"))
            {
                return TraoBangConstants.VangMat;
            }

            if (status.Contains("tham gia"))
            {
                return TraoBangConstants.ThamGiaTraoBang;
            }

            return TraoBangConstants.ChuanBi;
        }

        private async Task<List<List<string>>> _getSheetData(string sheetUrl, string sheetName)
        {
            var serviceAccountPath = _configuration["Google:ServiceAccountPath"];

            if (string.IsNullOrEmpty(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ServiceAccountErrorNotFound));
            }

            if (!Path.IsPathRooted(serviceAccountPath))
            {
                var basePath = AppContext.BaseDirectory;
                serviceAccountPath = Path.Combine(basePath, serviceAccountPath);
            }

            if (!File.Exists(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ServiceAccountErrorNotFound));
            }

            var credential = GoogleCredential.FromFile(serviceAccountPath)
                .CreateScoped("https://www.googleapis.com/auth/spreadsheets.readonly");

            var service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API"
            });

            var spreadsheetId = _extractSpreadsheetId(sheetUrl);
            var range = sheetName;

            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            var values = response.Values;

            var responseData = new List<List<string>>();

            if (values != null && values.Any())
            {
                foreach (var row in values)
                {
                    var stringRow = row.Select(c => c?.ToString() ?? string.Empty).ToList();
                    responseData.Add(stringRow);
                }
            }

            return responseData;
        }
        private string _extractSpreadsheetId(string sheetUrl)
        {
            var match = System.Text.RegularExpressions.Regex.Match(sheetUrl, @"/spreadsheets/d/([a-zA-Z0-9-_]+)");
            if (!match.Success || string.IsNullOrEmpty(match.Groups[1].Value))
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid, ErrorMessages.GetMessage(ErrorCodes.GoogleSheetUrlErrorInvalid));
            }
            return match.Groups[1].Value;
        }
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
