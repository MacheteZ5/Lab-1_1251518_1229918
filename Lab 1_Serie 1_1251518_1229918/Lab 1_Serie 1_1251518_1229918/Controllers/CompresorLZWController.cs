using Lab_1_Serie_1_1251518_1229918.Models;
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
        //static int ContadorElementosDiccionario = 0;
        const int bufferLengt = 1000000;
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
            //le permite corroborar si la carpeta Files ya existe en la solución
            bool Exists;
            string Paths = Server.MapPath("~/Files/");
            Exists = Directory.Exists(Paths);
            if (!Exists)
            {
                Directory.CreateDirectory(Paths);
            }
            //el siguiente if permite seleccionar un archivo en específico
            if (postedFile != null)
            {
                string rutaDirectorioUsuario = Server.MapPath(string.Empty);
                //se toma la ruta y nombre del archivo
                ArchivoLeido = rutaDirectorioUsuario + Path.GetFileName(postedFile.FileName);
                // se añade la extensión del archivo
                RutaArchivos = rutaDirectorioUsuario;
                postedFile.SaveAs(ArchivoLeido);
                LZWCompressor LZWLectura = new LZWCompressor();
                diccionario = LZWLectura.LecturaArchivo(ArchivoLeido, bufferLengt, diccionario, RutaArchivos);
            }
            return RedirectToAction("MétodoLZW", new RouteValueDictionary(new { Controller = "CompresorLZW", Action = "MétodoLZW", ArchivoLeido }));
        }

        public ActionResult MétodoLZW(string ArchivoLeido)
        {
            LZWCompressor LZW = new LZWCompressor();
            var ListaValores = new List<string>();
            var ListaBytesComprimidos = new List<byte>();
            diccionario = LZW.CompararCaracteres(diccionario, ref ListaValores, bufferLengt, ArchivoLeido);
            //convertirlos a bytes
            var valorCadena = LZW.CuantosBitsSeNecesitan(diccionario.Count());
            byte[] bytebuffer = LZW.CreaciónBufferEscritura(diccionario, ListaValores, ListaBytesComprimidos, valorCadena);
            using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\archivoComprimido.lzw", FileMode.Open))
            {
                using (var writer = new BinaryWriter(writeStream))
                {
                    writer.Seek(0, SeekOrigin.Begin);
                    writer.Write(Convert.ToByte(valorCadena));
                    writer.Seek(0, SeekOrigin.End);
                    writer.Write(bytebuffer);
                }
            }
            return View();
        }
        public ActionResult LecturaDescompresión()
        {
            return View();
        }
        static List<byte> ASCII = new List<byte>();
        static int CantidadBitsRequeridos = 0;
        [HttpPost]
        public ActionResult LecturaDesCompresión(HttpPostedFileBase postedFile)
        {
            string ArchivoLeido = string.Empty;
            //le permite corroborar si la carpeta Files ya existe en la solución
            bool Exists;
            string Paths = Server.MapPath("~/Files/");
            Exists = Directory.Exists(Paths);
            if (!Exists)
            {
                Directory.CreateDirectory(Paths);
            }
            if (postedFile != null)
            {
                string rutaDirectorioUsuario = Server.MapPath(string.Empty);
                //se toma la ruta y nombre del archivo
                ArchivoLeido = rutaDirectorioUsuario + Path.GetFileName(postedFile.FileName);
                // se añade la extensión del archivo
                RutaArchivos = rutaDirectorioUsuario;
                postedFile.SaveAs(ArchivoLeido);
                diccionario = new Dictionary<string, int>();
                using (var stream = new FileStream(ArchivoLeido, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var caracter = string.Empty;
                        var ValorDiccionario = 0;
                        byte[] byteBuffer = new byte[bufferLengt];
                        var encontrado = false;
                        var separador = false;
                        int conteo = 0;
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBuffer = reader.ReadBytes(10000000);
                            if (conteo == 0)
                            {
                                CantidadBitsRequeridos = byteBuffer[0];
                                conteo++;
                            }
                            for (int i = 0; i < byteBuffer.Count(); i++)
                            {
                                if (!separador)
                                {
                                    if ((byteBuffer[i] == 45))
                                    {
                                        if ((byteBuffer[i + 1] == 45))
                                        {
                                            separador = true;
                                            i = i + 2;
                                        }
                                    }
                                    if (!encontrado)
                                    {
                                        if (byteBuffer[i] == 124)
                                        {
                                            caracter += (char)byteBuffer[i - 1];
                                            ValorDiccionario = byteBuffer[i + 1];
                                            encontrado = true;
                                        }
                                    }
                                    else
                                    {
                                        diccionario.Add(caracter, ValorDiccionario);
                                        caracter = string.Empty;
                                        encontrado = false;
                                    }
                                }
                                else
                                {
                                    ASCII.Add(byteBuffer[i]);
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    ASCII.Remove(ASCII[0]);
                }
            }
            
            return RedirectToAction("MétodoLZWDescompresion");
        }

        public ActionResult MétodoLZWDescompresion()
        {
            LZWCompressor LZW = new LZWCompressor();
            string texto = LZW.Descompress(diccionario, ASCII, CantidadBitsRequeridos);
            return View("Download");
        }


        public ActionResult Download()
        {
            string path = Server.MapPath("~/Files/");
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles(".");
            List<string> lista = new List<string>(files.Length);
            foreach (var item in files)
            {
                lista.Add(item.Name);
            }
            return View(lista);
        }
        public ActionResult DownloadFile(string filename)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Files/"), filename);
            return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }
    }
}