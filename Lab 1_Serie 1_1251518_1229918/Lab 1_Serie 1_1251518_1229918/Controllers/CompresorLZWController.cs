using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lab_1_Serie_1_1251518_1229918.Controllers
{
    public class CompresorLZWController : Controller
    {
        
        static Dictionary<string, int> diccionario = new Dictionary<string, int>();
        static int ContadorElementosDiccionario = 0;
        static int  CantidadCaracteresArchivo = 0;
        const int bufferLengt = 1000;
        static string RutaArchivos = string.Empty;
        public ActionResult Index()
        {
            return View();
        }
        
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
                RutaArchivos = rutaDirectorioUsuario;
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
                                llave += (char)bit;
                                if (!diccionario.ContainsKey(llave))
                                {
                                    diccionario.Add(llave, ContadorElementosDiccionario);
                                    ContadorElementosDiccionario++;
                                }
                                CantidadCaracteresArchivo++;
                            }
                            //se escribe el diccionario original en el archivo
                            EscribirDiccionarioArchivo();
                        }
                    }
                }
            }
            return RedirectToAction("MétodoLZW", new RouteValueDictionary(new { Controller = "CompresorLZW", Action = "MétodoLZW", ArchivoLeido = ArchivoLeido }));
        }
        //metodo para escribir el diccionario original en el archivo 
        public void EscribirDiccionarioArchivo()
        {
            using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\archivoComprimido.lzw", FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(writeStream))
                {
                    //se escribe la cantidad de caracteres que tiene el archivo que se esta comprimiendo
                    writer.Write(CantidadCaracteresArchivo + "\r\n");
                    foreach (var elemento in diccionario)
                    {
                       
                        writer.Write(elemento.Key + "|" + elemento.Value + "\r\n");

                    }
                   
                }
            }

                

        }
        public ActionResult MétodoLZW(string ArchivoLeido)
        {
            string llave = string.Empty;
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
                                diccionario.Add(llave, ContadorElementosDiccionario);
                                llave = string.Empty;
                                llave += (char)bit;
                                ContadorElementosDiccionario++;
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



