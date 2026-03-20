using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.DashBoard.Dto;
using thongbao.be.application.DashBoard.Interfaces;
using thongbao.be.application.GuiTinNhan.Implements;
using thongbao.be.domain.Auth;
using thongbao.be.infrastructure.data;

namespace thongbao.be.application.DashBoard.Implements
{
    public class DashBoardService : BaseService, IDashBoardService
    {
        private readonly UserManager<AppUser> _userManager;

        public DashBoardService(
            SmDbContext smDbContext,
            ILogger<ChienDichService> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _userManager = userManager;
        }

        public GetStatisticsUserCreditsTheoNam GetStatisticsUserCreditsTheoNam(DateTime tuNgay, DateTime denNgay)
        {
            _logger.LogInformation($"{nameof(GetStatisticsUserCreditsTheoNam)}, tuNgay={tuNgay:yyyy-MM-dd}, denNgay={denNgay:yyyy-MM-dd}");

            var rawData = (from uc in _smDbContext.UserCredits
                           join u in _userManager.Users on uc.UserId equals u.Id
                           where !uc.Deleted
                                 && uc.ThoiGianBatDauApDungHanMuc >= tuNgay
                                 && uc.ThoiGianBatDauApDungHanMuc <= denNgay
                           select new
                           {
                               uc.UserId,
                               u.FullName,
                               uc.CreditDaSuDung
                           }).ToList(); 

            var grouped = rawData
                .GroupBy(x => new { x.UserId, x.FullName })
                .Select(g => new GetStatisticsUserCreditsTheoNamByUser
                {
                    User = new User
                    {
                        UserId = g.Key.UserId,
                        FullName = g.Key.FullName
                    },
                    CreditDaSuDung = g.Sum(x =>
                        string.IsNullOrEmpty(x.CreditDaSuDung)
                            ? 0
                            : double.Parse(x.CreditDaSuDung)
                    ).ToString(),
                    DonVi = "VND"
                }).ToList();

            return new GetStatisticsUserCreditsTheoNam
            {
                ListUserCredits = grouped
            };
        }

        public GetStatisticsTongSoTinNhanDaGuiTheoNam GetStatisticsTongSoTinNhanDaGuiTheoNam(DateTime tuNgay, DateTime denNgay)
        {
            _logger.LogInformation($"{nameof(GetStatisticsTongSoTinNhanDaGuiTheoNam)}, tuNgay={tuNgay:yyyy-MM-dd}, denNgay={denNgay:yyyy-MM-dd}");

            var rawData = (from log in _smDbContext.ChienDichLogTrangThaiGuis
                           join u in _userManager.Users on log.CreatedBy equals u.Id
                           where !log.Deleted
                                 && log.CreatedDate >= tuNgay
                                 && log.CreatedDate <= denNgay
                           select new
                           {
                               log.CreatedBy,
                               u.FullName,
                               log.SmsSendSuccess,
                               log.SmsSendFailed,
                               log.TongSoSms
                           }).ToList();

            if (!rawData.Any())
            {
                return new GetStatisticsTongSoTinNhanDaGuiTheoNam
                {
                    ListThongKeTongSoTinNhanDaGui = new List<GetStatisticsTongSoTinNhanDaGuiTheoNamTheoUser>()
                };
            }

            var grouped = rawData
                .GroupBy(x => new { x.CreatedBy, x.FullName })
                .Select(g => new GetStatisticsTongSoTinNhanDaGuiTheoNamTheoUser
                {
                    User = new User
                    {
                        UserId = g.Key.CreatedBy ?? string.Empty,
                        FullName = g.Key.FullName
                    },
                    TongSoTinNhanDaGuiThanhCong = g.Sum(x => x.SmsSendSuccess),
                    TongSoTinNhanDaGuiThatBai = g.Sum(x => x.SmsSendFailed),
                    TongSoTinNhanDaGui = g.Sum(x => x.TongSoSms)
                }).ToList();

            return new GetStatisticsTongSoTinNhanDaGuiTheoNam
            {
                ListThongKeTongSoTinNhanDaGui = grouped
            };
        }

        public GetStatisticsDashBoard GetStatisticsDashBoard()
        {
            _logger.LogInformation($"{nameof(GetStatisticsDashBoard)}");

            var tongSoLuongTinNhanGuiThanhCong = _smDbContext.ChienDichLogTrangThaiGuis
                .Where(x => !x.Deleted)
                .Sum(x => (int?)x.SmsSendSuccess) ?? 0;

            var tongSoUserSuDungDichVu = _smDbContext.UserNhaCungCapDichVus
                .Where(x => !x.Deleted)
                .Select(x => x.UserId)
                .Distinct()
                .Count();

            var tongSoChienDichGuiTinNhan = _smDbContext.ChienDiches
                .Where(x => !x.Deleted)
                .Count();

            var tongSoNhaCungCapDichVu = _smDbContext.NhaCungCapDichVus
                .Where(x => !x.Deleted)
                .Count();

            return new GetStatisticsDashBoard
            {
                TongSoLuongTinNhanGuiThanhCong = tongSoLuongTinNhanGuiThanhCong,
                TongSoUserSuDungDichVu = tongSoUserSuDungDichVu,
                TongSoChienDichGuiTinNhan = tongSoChienDichGuiTinNhan,
                TongSoNhaCungCapDichVu = tongSoNhaCungCapDichVu
            };
        }

        public GetStatisticsUserCreditsTheoThangByUser GetStatisticsUserCreditsTheoThangByUser(string userId, DateTime tuThang, DateTime denThang)
        {
            _logger.LogInformation($"{nameof(GetStatisticsUserCreditsTheoThangByUser)} userId = {userId}; tuThang = {tuThang:MM/yyyy}; denThang = {denThang:MM/yyyy}");

            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

            var thangBatDau = new DateTime(tuThang.Year, tuThang.Month, 1);
            var thangKetThuc = new DateTime(denThang.Year, denThang.Month, 1);

            var listTheoThang = new List<GetStatisticsUserCreditsByUser>();

            for (var thang = thangBatDau; thang <= thangKetThuc; thang = thang.AddMonths(1))
            {
                var tuNgay = thang;
                var denNgay = new DateTime(thang.Year, thang.Month, DateTime.DaysInMonth(thang.Year, thang.Month));

                var credits = _smDbContext.UserCredits
                    .Where(x => x.UserId == userId
                             && !x.Deleted
                             && x.ThoiGianBatDauApDungHanMuc >= tuNgay
                             && x.ThoiGianBatDauApDungHanMuc <= denNgay)
                    .Select(x => new
                    {
                        x.CreditDaSuDung,
                        x.HanMucCredit,
                    })
                    .ToList();

                var tongCreditDaSuDung = credits
                    .Where(x => !string.IsNullOrEmpty(x.CreditDaSuDung))
                    .Sum(x => double.Parse(x.CreditDaSuDung));

                var tongHanMucCredit = credits
                    .Where(x => !string.IsNullOrEmpty(x.HanMucCredit))
                    .Sum(x => double.Parse(x.HanMucCredit));

                listTheoThang.Add(new GetStatisticsUserCreditsByUser
                {
                    TuNgay = tuNgay,
                    DenNgay = denNgay,
                    User = new UserTheoThang
                    {
                        UserId = userId,
                        FullName = user != null ? user.FullName : string.Empty
                    },
                    CreditDaSuDung = tongCreditDaSuDung.ToString(),
                    HanMucCredit = tongHanMucCredit.ToString(),
                    DonVi = "VND"
                });
            }

            return new GetStatisticsUserCreditsTheoThangByUser
            {
                UserCreditsTheoThangByUsers = listTheoThang
            };
        }



    }


 }
