using System;
using System.Collections.Generic;

using System.IO;


using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Lab_1_Serie_1_1251518_1229918.Controllers
{
    public class CompresorLZWController : Controller
    {
        const int bufferLengt = 1000;


namespace Lab1_FernandoOliva_1251518.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LecturaCompresión(HttpPostedFileBase postedFile)
        {
            //el siguiente if permite seleccionar un archivo en específico
            if (postedFile != null)
            {
                string rutaDirectorioUsuario = Server.MapPath("");
                string ArchivoLeido = string.Empty;

                //se toma la ruta y nombre del archivo
                ArchivoLeido = rutaDirectorioUsuario + Path.GetFileName(postedFile.FileName);
                // se añade la extensión del archivo
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(ArchivoLeido);


                using (var stream = new FileStream(ArchivoLeido, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var byteBuffer = new byte[bufferLengt];
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBuffer = reader.ReadBytes(bufferLengt);
                            foreach (byte bit in byteBuffer)
                            {
                               //añadir al diccionario
                            }
                        }
                    }
                }
            }
            return View();

        }


    }
}


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

