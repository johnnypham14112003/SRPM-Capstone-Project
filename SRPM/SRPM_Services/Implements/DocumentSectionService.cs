using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class DocumentSectionService : IDocumentSectionService
{
    private readonly IUnitOfWork _unitOfWork;
    public DocumentSectionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //=============================================================================
    public async Task<(bool result, Guid DocumentId)> AddSection(RQ_DocumentSection docSec)
    {
        //Check Null Data
        if (string.IsNullOrWhiteSpace(docSec.Title)) throw new BadRequestException("Document Name or Type cannot be empty!");

        var existDocument = await _unitOfWork.GetDocumentRepository().GetFullDetailDocument(docSec.DocumentId) ??
            throw new NotFoundException("Not found Document to insert Section!");

        //Handle invalid insert index
        int sectionIndex = docSec.SectionOrder <= 0 ? 1 : docSec.SectionOrder;

        //Shift index of all section after this insert index
        foreach (var section in existDocument.DocumentSections.Where(ds => ds.SectionOrder >= sectionIndex))
        {
            section.SectionOrder++;
        }

        DocumentSection documentSectionDTO = docSec.Adapt<DocumentSection>();

        await _unitOfWork.GetDocumentSectionRepository().AddAsync(documentSectionDTO);
        var resultSave = await _unitOfWork.GetDocumentRepository().SaveChangeAsync();
        return (resultSave, documentSectionDTO.Id);
    }

    public async Task<bool> UpdateDocumentSection(RQ_DocumentSection newDocumentSection)
    {
        var existDocumentSection = await _unitOfWork.GetDocumentSectionRepository().GetOneAsync(ds => ds.Id == newDocumentSection.Id)
            ?? throw new NotFoundException("Not found any DocumentSection match this Id!");

        //Transfer new Data to old Data
        newDocumentSection.Adapt(existDocumentSection);
        return await _unitOfWork.GetDocumentSectionRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteDocumentSection(Guid id)
    {
        var existDocumentSection = await _unitOfWork.GetDocumentSectionRepository().GetOneAsync(ds => ds.Id == id)
            ?? throw new NotFoundException("Not found any DocumentSection match this Id!");

        //Remove reference Key
        if (existDocumentSection.SectionContents is not null)
            await _unitOfWork.GetSectionContentRepository().DeleteRangeAsync(existDocumentSection.SectionContents);

        if (existDocumentSection.TableStructures is not null)
            await _unitOfWork.GetTableStructureRepository().DeleteRangeAsync(existDocumentSection.TableStructures);

        await _unitOfWork.GetDocumentSectionRepository().DeleteAsync(existDocumentSection);
        return await _unitOfWork.SaveChangesAsync();
    }
}