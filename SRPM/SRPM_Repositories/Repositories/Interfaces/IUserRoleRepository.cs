using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;
public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task<List<UserRole>> GetListByFilterAsync(
        Guid? accountId,
        Guid? roleId,
        Guid? projectId,
        Guid? appraisalCouncilId,
        string? status,
        bool? isOfficial
    );
}
