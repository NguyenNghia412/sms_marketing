using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Auth.Dtos.Permission;
using thongbao.be.application.Auth.Interfaces;
using thongbao.be.application.Base;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.Constants.Auth;

namespace thongbao.be.application.Auth.Implements
{
    public class PermissionsService : BaseService, IPermissionsService
    {
        public PermissionsService(
            SmDbContext smDbContext, ILogger<BaseService> logger, IHttpContextAccessor httpContextAccessor, IMapper mapper
        ) : base(smDbContext, logger, httpContextAccessor, mapper)
        {
        }

        public List<ViewPermissionDto> GetAllPermissions()
        {
            _logger.LogInformation($"{nameof(GetAllPermissions)}");

            var query = PermissionKeys.All.OrderBy(p => p).Select(x => new ViewPermissionDto
            {
                Key = x.Key,
                Name = x.Name,
                Category = x.Category
            }).ToList();

            return query;
        }
    }
}
