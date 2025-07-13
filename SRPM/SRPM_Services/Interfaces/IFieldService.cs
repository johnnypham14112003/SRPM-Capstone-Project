using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface IFieldService
    {
        Task<RS_Field?> GetByIdAsync(Guid id);
        Task<PagingResult<RS_Field>> GetListAsync(string? name, int pageIndex, int pageSize);
        Task<List<RS_Field>> GetAllAsync();
        Task<RS_Field> CreateAsync(RQ_Field request);
        Task<RS_Field?> UpdateAsync(Guid id, RQ_Field request);
        Task<bool> DeleteAsync(Guid id);
    }

}
