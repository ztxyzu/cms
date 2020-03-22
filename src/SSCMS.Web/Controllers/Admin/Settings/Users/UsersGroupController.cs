﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Request;
using SSCMS.Core.Extensions;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [Route("admin/settings/usersGroup")]
    public partial class UsersGroupController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UsersGroupController(IAuthManager authManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, IUserGroupRepository userGroupRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _userGroupRepository = userGroupRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                Groups = await _userGroupRepository.GetUserGroupListAsync(),
                AdminNames = await _administratorRepository.GetUserNameListAsync()
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody]IdRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            await _userGroupRepository.DeleteAsync(request.Id);

            return new GetResult
            {
                Groups = await _userGroupRepository.GetUserGroupListAsync()
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Submit([FromBody] UserGroup request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            if (request.Id == -1)
            {
                if (await _userGroupRepository.IsExistsAsync(request.GroupName))
                {
                    return this.Error("保存失败，已存在相同名称的用户组！");
                }

                var groupInfo = new UserGroup
                {
                    GroupName = request.GroupName,
                    AdminName = request.AdminName
                };

                await _userGroupRepository.InsertAsync(groupInfo);

                await _authManager.AddAdminLogAsync("新增用户组", $"用户组:{groupInfo.GroupName}");
            }
            else if (request.Id == 0)
            {
                var config = await _configRepository.GetAsync();

                config.UserDefaultGroupAdminName = request.AdminName;

                await _configRepository.UpdateAsync(config);

                await _authManager.AddAdminLogAsync("修改用户组", "用户组:默认用户组");
            }
            else if (request.Id > 0)
            {
                var groupInfo = await _userGroupRepository.GetUserGroupAsync(request.Id);

                if (groupInfo.GroupName != request.GroupName && await _userGroupRepository.IsExistsAsync(request.GroupName))
                {
                    return this.Error("保存失败，已存在相同名称的用户组！");
                }

                groupInfo.GroupName = request.GroupName;
                groupInfo.AdminName = request.AdminName;

                await _userGroupRepository.UpdateAsync(groupInfo);

                await _authManager.AddAdminLogAsync("修改用户组", $"用户组:{groupInfo.GroupName}");
            }

            return new GetResult
            {
                Groups = await _userGroupRepository.GetUserGroupListAsync()
            };
        }
    }
}