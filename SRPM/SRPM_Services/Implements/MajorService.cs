using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Implements
{
    public class MajorService : IMajorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MajorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RS_Major?> GetByIdAsync(Guid id)
        {
            var major = await _unitOfWork.GetMajorRepository().GetByIdAsync<Guid>(id);
            return major?.Adapt<RS_Major>();
        }

        public async Task<List<RS_Major>> GetAllAsync()
        {
            var majors = await _unitOfWork.GetMajorRepository().GetListAsync(_ => true, hasTrackings: false);
            return majors.Adapt<List<RS_Major>>();
        }

        public async Task<RS_Major> CreateAsync(RQ_Major request)
        {
            var entity = request.Adapt<Major>();
            entity.Id = Guid.NewGuid();

            await _unitOfWork.GetMajorRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Major>();
        }

        public async Task<RS_Major?> UpdateAsync(Guid id, RQ_Major request)
        {
            var repo = _unitOfWork.GetMajorRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;

            request.Adapt(entity);

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Major>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetMajorRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return false;

            await repo.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }

}
