using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class SystemConfigurationService : ISystemConfigurationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    public SystemConfigurationService(IUnitOfWork unitOfWork,
         INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }
    //=============================================================================

    public async Task<(bool scResult, bool notiResult)> AddNewConfig(RQ_SystemConfiguration inputData)
    {
        var existConfig = await _unitOfWork.GetSystemConfigurationRepository().GetOneAsync(sys =>
        sys.ConfigType.Equals(inputData.ConfigType) &&
        sys.ConfigKey.Equals(inputData.ConfigKey) &&
        sys.ConfigValue.Equals(inputData.ConfigValue)
        , null, false);

        if (existConfig is not null) throw new ConflictException("This Config is existed!");

        //Check Null Data
        bool hasInvalidFields = new[] { inputData.ConfigKey, inputData.ConfigValue, inputData.ConfigType }
        .Any(string.IsNullOrWhiteSpace);

        if (hasInvalidFields) throw new BadRequestException("ConfigKey or ConfigValue or ConfigType cannot be empty!");

        SystemConfiguration systemConfigurationDTO = inputData.Adapt<SystemConfiguration>();
        await _unitOfWork.GetSystemConfigurationRepository().AddAsync(systemConfigurationDTO);
        var resultSys = await _unitOfWork.GetSystemConfigurationRepository().SaveChangeAsync();

        //Create Notification After Add SystemConfig
        var (resultNoti, notiId) = await _notificationService.CreateNew(new RQ_Notification
        {
            Title = "New System Configuration just added!",
            IsGlobalSend = true,
            Type = "SystemConfiguration",
            ObjecNotificationId = systemConfigurationDTO.Id
        });
        return (resultSys, resultNoti);
    }

    public async Task<List<RQ_SystemConfiguration>> ListConfig(string typeData, string? keyData)
    {
        var listConfig = string.IsNullOrWhiteSpace(keyData) ?
            await _unitOfWork.GetSystemConfigurationRepository().GetListAsync(sys => sys.ConfigType.Equals(typeData), hasTrackings: false) :
            await _unitOfWork.GetSystemConfigurationRepository().GetListAsync(sys => sys.ConfigType.Equals(typeData) && sys.ConfigKey.Equals(keyData), hasTrackings : false);

        if (listConfig is null) throw new NotFoundException("Not found any config!");
        return listConfig.Adapt<List<RQ_SystemConfiguration>>();
    }

    public async Task<RS_SystemConfiguration> ViewDetailConfig(Guid id)
    {
        var existConfig = await _unitOfWork.GetSystemConfigurationRepository().GetOneAsync(sys => sys.Id == id)
            ?? throw new NotFoundException("Not found any config!");

        return existConfig.Adapt<RS_SystemConfiguration>();
    }

    public async Task<bool> ChangeConfig(RQ_SystemConfiguration newConfig)
    {
        var existConfig = await _unitOfWork.GetSystemConfigurationRepository().GetOneAsync(sys => sys.Id == newConfig.Id)
            ?? throw new NotFoundException("Not found any config!");

        //Null data is handled for Patch API in Mapster config

        //Transfer new Data to old Data
        newConfig.Adapt(existConfig);
        return await _unitOfWork.GetSystemConfigurationRepository().SaveChangeAsync();
    }

    public async Task<bool> RemoveConfig(Guid id)
    {
        var existConfig = await _unitOfWork.GetSystemConfigurationRepository().GetOneAsync(sys => sys.Id == id)
            ?? throw new NotFoundException("Not found any config!");

        var notifications = await _unitOfWork.GetNotificationRepository().GetListAsync(n => n.SystemConfigurationId == id);

        // Delete reference
        if (notifications is not null) await _unitOfWork.GetNotificationRepository().DeleteRangeAsync(notifications);

        await _unitOfWork.GetSystemConfigurationRepository().DeleteAsync(existConfig);
        return await _unitOfWork.GetSystemConfigurationRepository().SaveChangeAsync();
    }
}