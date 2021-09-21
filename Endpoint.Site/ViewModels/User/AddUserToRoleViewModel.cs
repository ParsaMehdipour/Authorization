using System.Collections.Generic;

namespace Endpoint.Site.ViewModels.User
{
    public class AddUserToRoleViewModel
    {
        public AddUserToRoleViewModel()
        {
            UserRoleViewModels = new List<UserRoleViewModel>();
        }

        public string UserId { get; set; }
        public List<UserRoleViewModel> UserRoleViewModels { get; set; }
    }

    public class UserRoleViewModel
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
