using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Repositories;
public class SystemConfigurationService : ISystemConfigurationService
{
    private readonly ISystemConfigurationRepository _systemConfigurationRepository;
    public SystemConfigurationService(ISystemConfigurationRepository systemConfigurationRepository)
    {
        _systemConfigurationRepository = systemConfigurationRepository;
    }
    //=============================================================================

    public async Task<bool> AddNewConfig(RQ_SystemConfiguration inputData)
    {
        var existConfig = await _systemConfigurationRepository.GetOneAsync(sys =>
        sys.ConfigType.Equals(inputData.ConfigType) &&
        sys.ConfigKey.Equals(inputData.ConfigKey) &&
        sys.ConfigValue.Equals(inputData.ConfigValue)
        , false);

        if (existConfig is not null) throw new ConflictException("This Config is existed!");

        //Check Null Data
        bool hasInvalidFields = new[] { inputData.ConfigKey, inputData.ConfigValue, inputData.ConfigType }
        .Any(string.IsNullOrWhiteSpace);

        await _systemConfigurationRepository.AddAsync(inputData.Adapt<SystemConfiguration>());
        return await _systemConfigurationRepository.SaveChangeAsync();
    }

    public async Task<List<RQ_SystemConfiguration>> ListConfig(string typeData, string? keyData)
    {
        var listConfig = string.IsNullOrWhiteSpace(keyData) ?
            await _systemConfigurationRepository.GetListAsync(sys => sys.ConfigType.Equals(typeData), false) :
            await _systemConfigurationRepository.GetListAsync(sys => sys.ConfigType.Equals(typeData) && sys.ConfigKey.Equals(keyData), false);

        if (listConfig is null) throw new NotFoundException("Not found any config!");
        return listConfig.Adapt<List<RQ_SystemConfiguration>>();
    }

    public async Task<RS_SystemConfiguration> ViewDetailConfig(Guid id)
    {
        var existConfig = await _systemConfigurationRepository.GetOneAsync(sys => sys.Id == id)
            ?? throw new NotFoundException("Not found any config!");

        return existConfig.Adapt<RS_SystemConfiguration>();
    }

    public async Task<bool> ChangeConfig(RQ_SystemConfiguration newConfig)
    {
        var existConfig = await _systemConfigurationRepository.GetOneAsync(sys => sys.Id == newConfig.Id)
            ?? throw new NotFoundException("Not found any config!");

        //Check Null Data
        bool hasInvalidFields = new[] { newConfig.ConfigKey, newConfig.ConfigValue, newConfig.ConfigType }
        .Any(string.IsNullOrWhiteSpace);

        //Transfer new Data to old Data
        newConfig.Adapt(existConfig);
        return await _systemConfigurationRepository.SaveChangeAsync();
    }

    public async Task<bool> RemoveConfig(Guid id)
    {
        var existConfig = await _systemConfigurationRepository.GetOneAsync(sys => sys.Id == id)
            ?? throw new NotFoundException("Not found any config!");

        await _systemConfigurationRepository.DeleteAsync(existConfig);
        return await _systemConfigurationRepository.SaveChangeAsync();
    }
}