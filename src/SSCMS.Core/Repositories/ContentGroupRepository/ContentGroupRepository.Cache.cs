using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS;
using SSCMS.Core.Utils;

namespace SSCMS.Core.Repositories.ContentGroupRepository
{
    public partial class ContentGroupRepository 
    {
        private string GetCacheKey(int siteId)
        {
            return Caching.GetListKey(TableName, siteId);
        }

        public async Task<List<string>> GetGroupNamesAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(ContentGroup.GroupName))
                .Where(nameof(ContentGroup.SiteId), siteId)
                .OrderBy(nameof(ContentGroup.Taxis))
                .OrderBy(nameof(ContentGroup.GroupName))
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