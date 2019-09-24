using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lab1_FernandoOliva_1251518.Controllers
{
    public class HomeController : Controller
    {
        //este controlador se utiliza solo para mostrar el menu que permite seleccionar el metodo de compresion
        public ActionResult Index()
        {
            return View();
        }
    }
}