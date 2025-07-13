using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using SRPM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using SRPM_Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace SRPM_Services.Implements
{
    public class FieldService : IFieldService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FieldService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RS_Field?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetFieldRepository().GetByIdAsync<Guid>(id);
            return entity?.Adapt<RS_Field>();
        }

        public async Task<PagingResult<RS_Field>> GetListAsync(string? name, int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.GetFieldRepository().GetListAsync(
                f =>
                    (string.IsNullOrWhiteSpace(name) || f.Name.Contains(name)),
                hasTrackings: false
            );

            var total = list.Count;
            var paged = list
                .OrderBy(f => f.Name) 
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagingResult<RS_Field>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = total,
                DataList = paged.Adapt<List<RS_Field>>()
            };
        }

        public async Task<List<RS_Field>> GetAllAsync()
        {
            var fields = await _unitOfWork.GetFieldRepository().GetListAsync(
                f => true, 
                hasTrackings: false
            );

            return fields.Adapt<List<RS_Field>>();
        }



        public async Task<RS_Field> CreateAsync(RQ_Field request)
        {
            var entity = request.Adapt<Field>();
            entity.Id = Guid.NewGuid();

            await _unitOfWork.GetFieldRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Field>();
        }

        public async Task<RS_Field?> UpdateAsync(Guid id, RQ_Field request)
        {
            var repo = _unitOfWork.GetFieldRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;

            request.Adapt(entity);
            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Field>();
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetFieldRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return false;

            await repo.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }

}
