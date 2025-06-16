using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;
public class TaskRepository : GenericRepository<Models.Task>, ITaskRepository
{
    private readonly SRPMDbContext _context;
    public TaskRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}