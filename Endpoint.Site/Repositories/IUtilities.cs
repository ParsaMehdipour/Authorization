using System.Collections.Generic;
using Endpoint.Site.ViewModels.Role;

namespace Endpoint.Site.Repositories
{
    public interface IUtilities
    {
        public IList<ActionAndControllerName> AreaAndActionAndControllerNamesList();
        public IList<string> GetAllAreasNames();
        public string DataBaseRoleValidationGuid();
    }
}
