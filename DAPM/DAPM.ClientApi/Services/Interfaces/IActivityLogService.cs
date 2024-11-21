using System;
using System.Threading.Tasks;

namespace DAPM.ClientApi.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task LogUserActivity(string userName, string action, string result, DateTime timestamp);
    }
}
