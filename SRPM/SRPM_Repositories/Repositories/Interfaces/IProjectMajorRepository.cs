using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces
{
    public interface IProjectMajorRepository : IGenericRepository<ProjectMajor>
    {
       Task<List<ProjectMajor>> GetListWithIncludesAsync(Guid? projectId, Guid? majorId);
    }
}
