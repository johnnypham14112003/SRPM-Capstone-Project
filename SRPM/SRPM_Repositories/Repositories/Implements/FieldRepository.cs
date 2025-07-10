using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Repositories.Repositories.Implements
{
    public class FieldRepository : GenericRepository<Field>, IFieldRepository
    {
        private readonly SRPMDbContext _context;
        public FieldRepository(SRPMDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
