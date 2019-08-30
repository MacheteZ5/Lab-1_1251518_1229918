using System.Web;
using System.Web.Mvc;

namespace Lab_1_Serie_1_1251518_1229918
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
