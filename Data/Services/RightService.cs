using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Cirice.Data.Services
{
    public class RightService
    {
        private readonly DbCompositionService _dbCompositionService;
        private UserManager<User> _userManager;

        public RightService(DbCompositionService dbCompositionService,UserManager<User> userManager)
        {
            _dbCompositionService = dbCompositionService;
            _userManager = userManager;
        }
        public async Task<bool> CheckRights(long compositionId,User user)
        {
            if (user != null)
            {
                var composition = _dbCompositionService.FindById(compositionId);
                if (composition != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    return composition.UserId == user.Id
                           || roles.Contains("admin");
                }
            }
            return false;
        }
    }
}
