using System.Web;
using System.Web.Mvc;

namespace Bits_And_Bytes_Vincenzo_Russo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
