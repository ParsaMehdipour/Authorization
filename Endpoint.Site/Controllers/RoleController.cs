using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Endpoint.Site.Repositories;
using Endpoint.Site.ViewModels.Role;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Endpoint.Site.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _memoryCache;
        private readonly IUtilities _utilities;

        public RoleController(RoleManager<IdentityRole> roleManager
        , IMemoryCache memoryCache
        , IUtilities utilities)
        {
            _roleManager = roleManager;
            _memoryCache = memoryCache;
            _utilities = utilities;
        }

        public IActionResult Index()
        {
            var model = _roleManager.Roles.Select(x => new IndexViewModel
            {
                RoleId = x.Id,
                RoleName = x.Name
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            var allMvcNames =
                _memoryCache.GetOrCreate("AreaAndActionAndControllerNamesList", p =>
                    {
                         p.AbsoluteExpiration = DateTimeOffset.MaxValue;
                        return _utilities.AreaAndActionAndControllerNamesList();
                    });

            var model = new CreateRoleViewModel()
            {
                ActionAndControllerNames = allMvcNames
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(model.RoleName);

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    var requestedClaims = model.ActionAndControllerNames
                            .Where(x => x.IsSelected).ToList();

                    foreach (var requestClaim in requestedClaims)
                    {
                        var areaName = (string.IsNullOrEmpty(requestClaim.AreaName)) ?
                            "NoArea" : requestClaim.AreaName;

                        await _roleManager.AddClaimAsync(role,
                            new Claim($"{areaName}|{requestClaim.ControllerName}|{requestClaim.ActionName}",
                                true.ToString()));
                    }

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }

            return View(model);
        }
    }
}
