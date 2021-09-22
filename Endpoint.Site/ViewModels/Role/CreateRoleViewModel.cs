using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Endpoint.Site.ViewModels.Role
{
    public class CreateRoleViewModel
    {
        public CreateRoleViewModel()
        {
            ActionAndControllerNames = new List<ActionAndControllerName>();
        }

        [Required()]
        [Display(Name = "نام مقام")]
        public string RoleName { get; set; }
        public IList<ActionAndControllerName> ActionAndControllerNames { get; set; }
    }
}
