using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Lab_1_1251518_1229918.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        //el siguiente ActionResult permite guardar el texto del archivo en un string 
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            string ArchivoLeido = string.Empty;
            //el siguiente if permite seleccionar un archivo en específico
            if (postedFile != null)
            {
                string ruta = Server.MapPath("~/Archivos/");
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }
                //se toma la ruta y nombre del archivo
                ArchivoLeido = ruta + Path.GetFileName(postedFile.FileName);
                // se añade la extensión del archivo
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(ArchivoLeido);
                //se lee el archivo con texto que se desa comprimir
                        string textoArchivo = System.IO.File.ReadAllText(ArchivoLeido);
            }
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

    }
}