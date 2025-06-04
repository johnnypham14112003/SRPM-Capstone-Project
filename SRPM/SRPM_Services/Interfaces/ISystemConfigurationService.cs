using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;
public interface ISystemConfigurationService
{
    Task<(bool scResult, bool notiResult)> AddNewConfig(RQ_SystemConfiguration inputData);
    Task<List<RQ_SystemConfiguration>> ListConfig(string typeData, string? keyData);
    Task<RS_SystemConfiguration> ViewDetailConfig(Guid id);
    Task<bool> ChangeConfig(RQ_SystemConfiguration newConfig);
    Task<bool> RemoveConfig(Guid id);
}
