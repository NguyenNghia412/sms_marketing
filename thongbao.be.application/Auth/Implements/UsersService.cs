using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Auth.Dtos.User;
using thongbao.be.application.Auth.Interfaces;
using thongbao.be.application.Base;
using thongbao.be.domain.Auth;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.Auth.Implements
{
    public class UsersService : BaseService, IUsersService
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersService(
            SmDbContext smDbContext, ILogger<BaseService> logger, IHttpContextAccessor httpContextAccessor, IMapper mapper,
            UserManager<AppUser> userManager
        ) : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _userManager = userManager;
        }

        public async Task Create(CreateUserDto dto)
        {
            _logger.LogInformation($"{nameof(Create)} dto={JsonSerializer.Serialize(dto)}");

            using var transaction = await _smDbContext.Database.BeginTransactionAsync();
            var newUser = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName ?? "",
                PhoneNumber = dto.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                throw new UserFriendlyException(ErrorCodes.AuthErrorCreateUser, string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRolesAsync(newUser, dto.RoleNames);

            await _smDbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task<ViewUserDto> FindById(string id)
        {
            _logger.LogInformation($"{nameof(FindById)} id={id}");

            var user = await _userManager.FindByIdAsync(id);

            return _mapper.Map<ViewUserDto>(user);
        }

        public async Task<ViewUserDto> FindByMsAccount(string msAccount)
        {
            _logger.LogInformation($"{nameof(FindByMsAccount)} id={msAccount}");

            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.MsAccount == msAccount)
                ?? throw new UserFriendlyException(ErrorCodes.AuthErrorUserNotFound);

            return _mapper.Map<ViewUserDto>(user);
        }

        public async Task<BaseResponsePagingDto<ViewUserDto>> FindPaging(FindPagingUserDto dto)
        {
            _logger.LogInformation($"{nameof(FindPaging)} dto={JsonSerializer.Serialize(dto)}");

            var query = _userManager.Users.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();

            var users = await query
                    .OrderBy(x => x.UserName)
                    .Paging(dto)
                    .ToListAsync();
            var items = _mapper.Map<List<ViewUserDto>>(users);

            return new BaseResponsePagingDto<ViewUserDto>
            {
                Items = items,
                TotalItems = totalCount,
            };
        }

        public async Task SetRoleForUser(SetRoleForUserDto dto)
        {
            _logger.LogInformation($"{nameof(SetRoleForUser)} dto={JsonSerializer.Serialize(dto)}");

            var transaction = await _smDbContext.Database.BeginTransactionAsync();
            var user = await _userManager.FindByIdAsync(dto.Id)
                ?? throw new UserFriendlyException(ErrorCodes.AuthErrorUserNotFound);

            await _userManager.RemoveFromRolesAsync(user, (await _userManager.GetRolesAsync(user)).ToArray());
            await _userManager.AddToRolesAsync(user, dto.RoleNames);
            await _smDbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(UpdateUserDto dto)
        {
            _logger.LogInformation($"{nameof(Update)} dto={JsonSerializer.Serialize(dto)}");

            var user = await _userManager.FindByIdAsync(dto.Id)
                ?? throw new UserFriendlyException(ErrorCodes.AuthErrorUserNotFound);

            var transaction = await _smDbContext.Database.BeginTransactionAsync();
            user.FullName = dto.FullName ?? user.FullName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.Email = dto.Email ?? user.Email;

            await _userManager.UpdateAsync(user);
            await _userManager.RemoveFromRolesAsync(user, (await _userManager.GetRolesAsync(user)).ToArray());
            await _userManager.AddToRolesAsync(user, dto.RoleNames);

            await _smDbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
