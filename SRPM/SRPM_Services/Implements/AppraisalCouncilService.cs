using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class AppraisalCouncilService : IAppraisalCouncilService
{
    private readonly IUnitOfWork _unitOfWork;
    public AppraisalCouncilService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //=============================================================================
    public async Task<(bool result, Guid CoucilId)> NewAppraisal(RQ_AppraisalCouncil inputData)
    {
        //Check Null Data
        bool hasInvalidFields = new[] { inputData.Code, inputData.Name }
        .Any(string.IsNullOrWhiteSpace);
        if (hasInvalidFields) throw new BadRequestException("Council code,name cannot be empty!");

        var existCouncil = await _unitOfWork.GetAppraisalCouncilRepository().GetOneAsync
            (apc => apc.Code.Equals(inputData.Code), null, false);

        if (existCouncil is not null)
            throw new ConflictException("This Council Code is existed!");

        AppraisalCouncil appraisalCouncilDTO = inputData.Adapt<AppraisalCouncil>();
        await _unitOfWork.GetAppraisalCouncilRepository().AddAsync(appraisalCouncilDTO);
        var resultSave = await _unitOfWork.GetAppraisalCouncilRepository().SaveChangeAsync();
        return (resultSave, appraisalCouncilDTO.Id);
    }

    public async Task<PagingResult<RS_AppraisalCouncil>?> ListCouncil(Q_AppraisalCouncil queryInput)
    {
        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageSize < 1 ? 1 : queryInput.PageSize;

        var dataResult = await _unitOfWork.GetAppraisalCouncilRepository().ListPaging
            (queryInput.KeyWord, queryInput.FromDate, queryInput.ToDate,
            queryInput.SortBy, queryInput.Status, queryInput.PageIndex, queryInput.PageSize);

        // Checking Result
        if (dataResult.listCouncil is null || dataResult.listCouncil.Count == 0)
            throw new NotFoundException("Not Found Any Council!");

        return new PagingResult<RS_AppraisalCouncil>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = dataResult.totalCount,
            DataList = dataResult.listCouncil.Adapt<List<RS_AppraisalCouncil>>()
        };
    }

    public async Task<RS_AppraisalCouncil> ViewDetailCouncil(Guid? id, byte includeNum = 0)
    {
        if (id is null || id == Guid.Empty) throw new BadRequestException("Cannot view a null council Id!");
        var existCouncil = await _unitOfWork.GetAppraisalCouncilRepository().GetDetailWithInclude((Guid)id, includeNum)
            ?? throw new NotFoundException("Not found this Council Id!");

        return existCouncil.Adapt<RS_AppraisalCouncil>();
    }

    public async Task<bool> UpdateCouncilInfo(RQ_AppraisalCouncil newCouncil)
    {
        var existCouncil = await _unitOfWork.GetAppraisalCouncilRepository().GetOneAsync(ac => ac.Id == newCouncil.Id)
            ?? throw new NotFoundException("Not found any Council match this Id!");

        //Null data is handled for Patch API in Mapster config

        //Transfer new Data to old Data
        newCouncil.Adapt(existCouncil);
        return await _unitOfWork.GetAppraisalCouncilRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteCouncil(Guid id)
    {
        var existCouncil = await _unitOfWork.GetAppraisalCouncilRepository().GetOneAsync(ac => ac.Id == id)
            ?? throw new NotFoundException("Not found any Council match this Id!");

        //Remove reference Key
        //...
        //await _unitOfWork.GetAppraisalCouncilRepository().DeleteAsync(existCouncil);

        existCouncil.Status = "deleted";
        return await _unitOfWork.GetAppraisalCouncilRepository().SaveChangeAsync();
    }
}