using System.Collections.Generic;
using System.Security.Claims;

namespace Endpoint.Site.Repositories
{
    public class ClaimStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim(ClaimTypesStore.CarList,true.ToString()),
            new Claim(ClaimTypesStore.CarDetails,true.ToString()),
            new Claim(ClaimTypesStore.CarEdit,true.ToString()),
            new Claim(ClaimTypesStore.AddCar,true.ToString())
        };
    }

    public static class ClaimTypesStore
    {
        public const string CarList = "carList";
        public const string CarDetails = "carDetails";
        public const string CarEdit = "carEdit";
        public const string AddCar = "Addcar";
    }
}
