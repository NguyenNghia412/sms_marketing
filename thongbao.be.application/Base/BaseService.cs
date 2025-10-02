using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.Constants.Auth;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace thongbao.be.application.Base
{
    public class BaseService
    {
        public readonly SmDbContext _smDbContext;
        public readonly ILogger<BaseService> _logger;
        public readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMapper _mapper;

        public BaseService(
            SmDbContext smDbContext,
            ILogger<BaseService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
        {
            _smDbContext = smDbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        protected string getCurrentUserId()
        {
            var data = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return data!;
        }

        protected string getCurrentName()
        {
            var data = _httpContextAccessor.HttpContext.User.FindFirstValue(Claims.Name);
            return data!;
        }
    }
}
