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
            var data = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(data))
            {
                data = _httpContextAccessor.HttpContext?.User.FindFirstValue(Claims.Subject);
            }
            //_logger.LogInformation($"getCurrentUserId: {data}");
            return data!;
        }
        protected string getCurrentName()
        {
            var data = _httpContextAccessor.HttpContext?.User.FindFirstValue(Claims.Name);
            return data!;
        }
        protected bool IsSuperAdmin()
        {
            var roles = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).ToList();
            var isSuperAdmin = roles?.Any(r => r.Value == RoleConstants.ROLE_SUPER_ADMIN) ?? false;
            return isSuperAdmin;
        }
    }
}