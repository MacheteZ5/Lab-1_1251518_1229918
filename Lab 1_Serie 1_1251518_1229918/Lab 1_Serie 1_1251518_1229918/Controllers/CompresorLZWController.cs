using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lab_1_Serie_1_1251518_1229918.Controllers
{
    public class CompresorLZWController : Controller
    {
        const int bufferLengt = 1000;
        public ActionResult Index()
        {
            return View();
        }
        static Dictionary<string, int> diccionario = new Dictionary<string, int>();
        static int i = 0;
        public ActionResult LecturaCompresión()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LecturaCompresión(HttpPostedFileBase postedFile)
        {
            string ArchivoLeido = string.Empty;
            //el siguiente if permite seleccionar un archivo en específico
            if (postedFile != null)
            {
                string rutaDirectorioUsuario = Server.MapPath("");
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
                            string llave = string.Empty;
                            foreach (byte bit in byteBuffer)
                            {
                                //añadir al diccionario
                                llave = string.Empty;
                                llave = llave + (char)bit;
                                if (!diccionario.ContainsKey(llave))
                                {
                                    diccionario.Add(llave, i);
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("MétodoLZW", new RouteValueDictionary(new { Controller = "CompresorLZW", Action = "MétodoLZW", ArchivoLeido = ArchivoLeido }));
        }
        public ActionResult MétodoLZW(string ArchivoLeido)
        {
            string llave = "";
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
                            llave = llave + (char)bit;
                            if (!diccionario.ContainsKey(llave))
                            {
                                diccionario.Add(llave, i);
                                llave = string.Empty;
                                llave = llave + (char)bit;
                                i++;
                            }
                        }
                    }
                }
            }
            return View();
        }
        public ActionResult LecturaDescompresión(HttpPostedFileBase postedFile)
        {
            return View();
        }
    }
}



