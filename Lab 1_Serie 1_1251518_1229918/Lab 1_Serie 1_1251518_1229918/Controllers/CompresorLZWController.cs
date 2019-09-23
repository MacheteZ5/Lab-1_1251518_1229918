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
        static int ContadorElementosDiccionario = 0;
        const int bufferLengt = 10000;
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
                string rutaDirectorioUsuario = Server.MapPath("");
                //se toma la ruta y nombre del archivo
                ArchivoLeido = rutaDirectorioUsuario + Path.GetFileName(postedFile.FileName);
                // se añade la extensión del archivo
                //string extension = Path.GetExtension(postedFile.FileName);
                RutaArchivos = rutaDirectorioUsuario;
                postedFile.SaveAs(ArchivoLeido);
                LZWCompressor LZWLectura = new LZWCompressor();
                diccionario = LZWLectura.LecturaArchivo(ArchivoLeido, bufferLengt, diccionario, ContadorElementosDiccionario, RutaArchivos);
                ContadorElementosDiccionario = diccionario.Count();
            }
            return RedirectToAction("MétodoLZW", new RouteValueDictionary(new { Controller = "CompresorLZW", Action = "MétodoLZW", ArchivoLeido }));
        }
        
        public ActionResult MétodoLZW(string ArchivoLeido)
        {
            LZWCompressor LZW = new LZWCompressor();
            var ListaValores = new List<int>();
            var ListaBytesComprimidos = new List<byte>();
            var previo = string.Empty;
            var actual = string.Empty;
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
                            actual += (char)bit;
                            if (!diccionario.ContainsKey(actual))
                            {
                                ContadorElementosDiccionario++;
                                diccionario.Add(actual, ContadorElementosDiccionario);
                                actual = string.Empty;
                                actual += (char)bit;
                                ListaValores.Add(/*LZW.ConvertToBinary*/(diccionario[previo]));
                                previo = string.Empty;
                                previo += (char)bit;
                            }
                            else
                            {
                                previo += (char)bit;
                            }
                        }
                        if (previo != string.Empty)
                        {
                            ListaValores.Add(/*LZW.ConvertToBinary*/(diccionario[previo]));
                        }
                    }
                    //convertirlos a bytes
                    var binario = string.Empty;
                    var valorCadena = LZW.CuantosBitsSeNecesitan(diccionario.Count());

                    foreach (int aux in ListaValores)
                    {
                        byte DECABYTE = new byte();
                        binario = LZW.ConvertToBinary(aux);
                        binario = binario.PadLeft(valorCadena, '0');
                        if (aux < 256)
                        {
                            decimal x = Convert.ToInt32(binario, 2);
                            DECABYTE = Convert.ToByte(x);
                            ListaBytesComprimidos.Add(DECABYTE);
                            binario = string.Empty;
                        }
                        else
                        {
                            string pruebas = string.Empty;
                            foreach(char caracter in binario)
                            {
                                pruebas+= caracter;
                                if (pruebas.Count() == 8)
                                {
                                    decimal x = Convert.ToInt32(pruebas, 2);
                                    DECABYTE = Convert.ToByte(x);
                                    ListaBytesComprimidos.Add(DECABYTE);
                                    pruebas = string.Empty;
                                }
                            }
                            if (pruebas != string.Empty)
                            {
                                pruebas = pruebas.PadRight(8, '0');
                                decimal x = Convert.ToInt32(pruebas, 2);
                                DECABYTE = Convert.ToByte(x);
                                ListaBytesComprimidos.Add(DECABYTE);
                            }
                            binario = string.Empty;
                        }
                    }
                    byte[] bytebuffer = new byte[ListaBytesComprimidos.Count()];
                    for (int i = 0; i < ListaBytesComprimidos.Count(); i++)
                    {
                        bytebuffer[i] = ListaBytesComprimidos[i];
                    }
                    using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\archivoComprimido.lzw", FileMode.Open))
                    {
                        using (var writer = new BinaryWriter(writeStream))
                        {
                            writer.Seek(0,SeekOrigin.Begin);
                            writer.Write(Convert.ToByte(valorCadena));
                            writer.Seek(0, SeekOrigin.End);
                            writer.Write(bytebuffer);
                        }
                    }
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
            diccionario = new Dictionary<string, int>();
            using (var stream = new FileStream(RutaArchivos + "\\..\\Files\\archivoComprimido.lzw", FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var caracter = string.Empty;
                    var ValorDiccionario = 0;
                    byte[] byteBuffer = new byte[10000];
                    var encontrado = false;
                    var separador = false;
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(10000);
                        CantidadBitsRequeridos = byteBuffer[0];
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
                                        ValorDiccionario = byteBuffer[i+1];
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
            return RedirectToAction("MétodoLZWDescompresion");
        }

        public ActionResult MétodoLZWDescompresion()
        {
            LZWCompressor LZW = new LZWCompressor();
            LZW.Descompress(diccionario, ASCII, CantidadBitsRequeridos);
            return View();
        }
    }
}