using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.Config.Interfaces;
using thongbao.be.application.GuiTinNhan.Implements;
using thongbao.be.infrastructure.data;

namespace thongbao.be.application.Config.Implements
{
    public class UserCreditsJobService: BaseService,IUserCreditsJobService
    {
        private IBackgroundJobClient _backgroundJobClient;
        private IUserCreditsService _userCreditsService;

        public UserCreditsJobService(
            SmDbContext smDbContext,
            ILogger<GuiTinNhanJobService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IBackgroundJobClient backgroundJobClient,
            IUserCreditsService userCreditsService
            ):base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _backgroundJobClient = backgroundJobClient;
            _userCreditsService = userCreditsService;
        }

        public void CronJobCreateUserCreditsMoiThang()
        {
            var vietNamNow = GetVietnamTime();
            RecurringJob.AddOrUpdate(
                "create-user-credits-moi-thang",
                () => _userCreditsService.AddCreditToUserCredits(),
                Cron.Daily(0, 5),
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
                }
            );
            _logger.LogInformation("Đã đăng ký cronjob tạo mới danh sách credits cho các user tháng {Thang}/{Nam}", vietNamNow.Month, vietNamNow.Year);
        }
    }
}
