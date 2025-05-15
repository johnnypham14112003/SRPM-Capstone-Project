using SRPM_Repositories.DTOs;
using SRPM_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> LoginWithGoogleAsync(GoogleLoginRQ request);
        // ... other account methods
    }
}
