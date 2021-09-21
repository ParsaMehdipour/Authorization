using System;
using System.Linq;
using Endpoint.Site.Models;
using Endpoint.Site.Models.Context;
using Endpoint.Site.ViewModels.Role;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Endpoint.Site.Controllers
{
    public class SiteSettingController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IMemoryCache _memoryCache;

        public SiteSettingController(ApplicationContext context,IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            var model = _context.SiteSettings.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult RoleValidationGuid()
        {
            var roleValidationGuidSiteSetting =
                _context.SiteSettings.FirstOrDefault(t => t.Key == "RoleValidationGuid");

            var model = new RoleValidationGuidViewModel()
            {
                Value = roleValidationGuidSiteSetting?.Value,
                LastTimeChanged = roleValidationGuidSiteSetting?.LatsTimeChanged
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult RoleValidationGuid(RoleValidationGuidViewModel model)
        {
            var roleValidationGuidSiteSetting =
                _context.SiteSettings.FirstOrDefault(t => t.Key == "RoleValidationGuid");

            if (roleValidationGuidSiteSetting == null)
            {
                _context.SiteSettings.Add(new SiteSetting()
                {
                    Key = "RoleValidationGuid",
                    Value = Guid.NewGuid().ToString(),
                    LatsTimeChanged = DateTime.Now
                });
            }
            else
            {
                roleValidationGuidSiteSetting.Value = Guid.NewGuid().ToString();
                roleValidationGuidSiteSetting.LatsTimeChanged = DateTime.Now;
                _context.Update(roleValidationGuidSiteSetting);
            }
            _context.SaveChanges();
            _memoryCache.Remove("RoleValidationGuid");

            return RedirectToAction("Index");
        }
    }
}
