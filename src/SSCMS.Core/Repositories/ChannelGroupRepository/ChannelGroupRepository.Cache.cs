using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS;
using SSCMS.Core.Utils;

namespace SSCMS.Core.Repositories.ChannelGroupRepository
{
    public partial class ChannelGroupRepository 
    {
        private string GetCacheKey(int siteId)
        {
            return Caching.GetListKey(TableName, siteId);
        }

        public async Task<List<string>> GetGroupNamesAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(ChannelGroup.GroupName))
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .OrderBy(nameof(ChannelGroup.Taxis))
                .OrderBy(nameof(ChannelGroup.GroupName))
                .CachingGet(GetCacheKey(siteId))
            );
        }

        public async Task<bool> IsExistsAsync(int siteId, string groupName)
        {
            var groupNames = await GetGroupNamesAsync(siteId);
            return groupNames.Contains(groupName);
        }
    }
}