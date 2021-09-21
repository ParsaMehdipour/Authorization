using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Endpoint.Site.Repositories;
using Endpoint.Site.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Site.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUser : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageUser(UserManager<IdentityUser> userManager
            ,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var model = _userManager.Users.Select(x => new IndexViewModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, string userName)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(userName)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            user.UserName = userName;
                
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty,error.Description);
            }

            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> AddUserToRole(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            var roles = _roleManager.Roles.ToList();

            var model = new AddUserToRoleViewModel() {UserId = id};

            foreach (var role in roles)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name) is false)
                {
                    model.UserRoleViewModels.Add(new UserRoleViewModel()
                    {
                        RoleName = role.Name
                    });
                }
            }

            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToRole(AddUserToRoleViewModel model)
        {
            if (model == null) return NotFound();

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null) return NotFound();

            var requestedRoles = model.UserRoleViewModels.Where(x => x.IsSelected)
                .Select(x => x.RoleName)
                .ToList();

            var result = await _userManager.AddToRolesAsync(user, requestedRoles);

            if (result.Succeeded)
                return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveUserFromRole(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = _roleManager.Roles.ToList();
            var model = new AddUserToRoleViewModel() { UserId = id };

            foreach (var role in roles)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name) is true)
                {
                    model.UserRoleViewModels.Add(new UserRoleViewModel()
                    {
                        RoleName = role.Name
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(AddUserToRoleViewModel model)
        {
            if (model == null) return NotFound();
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            var requestRoles = model.UserRoleViewModels.Where(r => r.IsSelected)
                .Select(u => u.RoleName)
                .ToList();
            var result = await _userManager.RemoveFromRolesAsync(user, requestRoles);

            if (result.Succeeded) return RedirectToAction("index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddUserToClaim(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var allClaim = ClaimStore.AllClaims;
            var userClaims = await _userManager.GetClaimsAsync(user);
            var validClaims =
                allClaim.Where(c => userClaims.All(x => x.Type != c.Type))
                    .Select(c => new ClaimsViewModel(c.Type)).ToList();

            var model = new AddOrRemoveClaimViewModel(id, validClaims);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToClaim(AddOrRemoveClaimViewModel model)
        {
            if (model == null) return NotFound();

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null) return NotFound();

            var requestedClaims = model.UserClaims.Where(u => u.IsSelected)
                .Select(u => new Claim(u.ClaimType, true.ToString())).ToList();

            var result = await _userManager.AddClaimsAsync(user, requestedClaims);

            if (result.Succeeded)
                return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

    }
}
