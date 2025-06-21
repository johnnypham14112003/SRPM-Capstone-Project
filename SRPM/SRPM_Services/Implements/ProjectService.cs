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
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RS_Project?> GetByIdAsync(Guid id)
        {
            var project = await _unitOfWork.GetProjectRepository().GetByIdAsync<Guid>(id);
            return project?.Adapt<RS_Project>();
        }

        public async Task<List<RS_Project>> GetAllAsync()
        {
            var projects = await _unitOfWork.GetProjectRepository().GetListAsync(_ => true, hasTrackings: false);
            return projects.Adapt<List<RS_Project>>();
        }

        public async Task<RS_Project> CreateAsync(RQ_Project request)
        {
            var project = request.Adapt<Project>();
            project.Id = Guid.NewGuid();
            project.CreatedAt = DateTime.UtcNow;
            project.Status = "created";

            await _unitOfWork.GetProjectRepository().AddAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return project.Adapt<RS_Project>();
        }

        public async Task<RS_Project?> UpdateAsync(Guid id, RQ_Project request)
        {
            var repo = _unitOfWork.GetProjectRepository();
            var project = await repo.GetByIdAsync<Guid>(id);
            if (project == null) return null;

            request.Adapt(project); 
            project.UpdatedAt = DateTime.UtcNow;

            await repo.UpdateAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return project.Adapt<RS_Project>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetProjectRepository();
            var project = await repo.GetByIdAsync<Guid>(id);
            if (project == null) return false;

            await repo.DeleteAsync(project);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }

}
