using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lab_1_Serie_1_1251518_1229918.Models;
namespace Lab_1_Serie_1_1251518_1229918.Controllers
{
    public class CompresorHuffmanController : Controller
    {
        //diccionario donde se guardarán las variables como llaves y sus cantidades de aparición como los valores
        static Dictionary<char, CantidadChar> diccionario = new Dictionary<char, CantidadChar>();
        static string RutaArchivos = string.Empty;
        static List<byte> ListaByte = new List<byte>();
        static List<TreeElement> lista = new List<TreeElement>();
        //largo del buffer al momento de la lectura
        const int bufferLengt = 1000000;
        [HttpPost]
        public ActionResult LecturaCompresión(HttpPostedFileBase postedFile)
        {
            bool Exists;
            string Paths = Server.MapPath("~/Files/");
            Exists = Directory.Exists(Paths);
            if (!Exists)
            {
                Directory.CreateDirectory(Paths);
            }
            if (postedFile != null)
            {
                string rutaDirectorioUsuario = Server.MapPath("");
                //se toma la ruta y nombre del archivo
                string ArchivoLeido = rutaDirectorioUsuario + Path.GetFileName(postedFile.FileName);
                // se añade la extensión del archivo
                RutaArchivos = rutaDirectorioUsuario;
                Arbol send = new Arbol();
                send.recibirRutaArchivo(RutaArchivos);
                postedFile.SaveAs(ArchivoLeido);
                //se aplicó la interfaz y el modelo Huffman
                //La Listabyte se utilizó una referencia debido a que la función retornará el diccionario
                Huffman HuffmanProcess = new Huffman();
                diccionario = HuffmanProcess.LecturaArchivoCompresion(diccionario, ArchivoLeido, bufferLengt, ref ListaByte);
                lista = HuffmanProcess.OrdenamientoDelDiccionario(diccionario,lista,ListaByte);
            }
            return RedirectToAction("Arbol");
        }
        public ActionResult LecturaCompresión()
        {
            return View();
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

        private static CantidadChar GetAnyValue<T>(byte Key)
        {
            CantidadChar obj;
            CantidadChar retType;
            diccionario.TryGetValue((char)Key, out obj);
            try
            {
                retType = (CantidadChar)obj;
            }
            catch
            {
                retType = default(CantidadChar);
            }
            return retType;
        }
        public ActionResult Index()
        {
            return View();
        }
        
        //Al momento de haber recibido el string del texto, habrá que separar caracter por caracter
        public ActionResult Arbol()
        {
            //creación del árbol
            Huffman HuffmanProcess = new Huffman();
            Arbol Arbol = new Arbol();
            Arbol.raíz = HuffmanProcess.TreeCreation(lista);
            string prefíjo = string.Empty;
            diccionario = Arbol.códigosPrefíjo(Arbol.raíz, diccionario, prefíjo);
            //Escritura del compresor códigos prefíjos convertidos a bytes
            using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\archivoComprimido.huff", FileMode.Open))
            {
                using (var writer = new BinaryWriter(writeStream))
                {
                    byte[] bytebuffer = new byte[500];
                    List<char> cadena = new List<char>();
                    int cantidadbuffer = 0;
                    foreach (byte bit in ListaByte)
                    {
                        CantidadChar separación = new CantidadChar();
                        separación = GetAnyValue<int>(bit);
                        foreach (char caracter in separación.codPref)
                        {
                            cadena.Add(caracter);
                        }
                    }
                    string binario = "";
                    foreach (char car in cadena)
                    {
                        if (binario.Count() == 8)
                        {
                            byte DECABYTE = new byte();
                            var pref = binario;
                            decimal x = Convert.ToInt32(pref, 2);
                            DECABYTE = Convert.ToByte(x);
                            bytebuffer[cantidadbuffer] = DECABYTE;
                            cantidadbuffer++;
                            binario = string.Empty;
                            binario = binario + car;
                        }
                        else
                        {
                            binario = binario + car;
                        }
                        if (cantidadbuffer == 500)
                        {
                            writer.Seek(0, SeekOrigin.End);
                            writer.Write(bytebuffer);
                            cantidadbuffer = 0;
                            bytebuffer = new byte[500];
                        }
                    }
                    if (binario != string.Empty)
                    {
                        while (binario.Count() != 8)
                        {
                            binario = binario + "0";
                        }
                        byte DECABYTE = new byte();
                        var pref = binario;
                        decimal x = Convert.ToInt32(pref, 2);
                        DECABYTE = Convert.ToByte(x);
                        bytebuffer[cantidadbuffer] = DECABYTE;
                        int contador0 = 0;
                        List<byte> ListAux = new List<byte>();
                        foreach (byte bit in bytebuffer)
                        {
                            if (bit == 0)
                            {
                                contador0++;
                            }
                            else
                            {
                                contador0 = 0;
                            }
                            if (contador0 != 10)
                            {
                                ListAux.Add(bit);
                            }
                            else
                            {
                                for (int i = 0; i < 9; i++)
                                {
                                    ListAux.Remove(ListAux.Last());
                                }
                                break;
                            }
                        }
                        bytebuffer = new byte[ListAux.Count()];
                        int j = 0;
                        foreach (byte bit in ListAux)
                        {
                            bytebuffer[j] = bit;
                            j++;
                        }
                        writer.Seek(0, SeekOrigin.End);
                        writer.Write(bytebuffer);
                    }

                }
            }
            return RedirectToAction("Download");
        }
        //Método de descompresión
        //Lectura del archivo e introducir los códigos prefijos con sus respectivos caracteres al diccionario 
        static List<byte> ASCII = new List<byte>();
        [HttpPost]

        public ActionResult LecturaDescompresión(HttpPostedFileBase postedFile)
        {
            diccionario = new Dictionary<char, CantidadChar>();
            using (var stream = new FileStream(RutaArchivos + "\\..\\Files\\archivoComprimido.huff", FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    string prefijos = string.Empty;
                    char caracter = ' ';
                    byte[] byteBuffer = new byte[bufferLengt];
                    bool encontrado = false;
                    bool separador = false;
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLengt);
                        for (int i = 0; i < byteBuffer.Count(); i++)
                        {
                            if (separador != true)
                            {
                                if ((byteBuffer[i] == 45))
                                {
                                    if ((byteBuffer[i + 1] == 45))
                                    {
                                        separador = true;
                                        i = i + 2;
                                    }
                                }
                                if (encontrado == false)
                                {
                                    if (byteBuffer[i] == 124)
                                    {
                                        caracter = (char)byteBuffer[i-1];
                                        encontrado = true;
                                    }
                                }
                                else
                                {
                                    if ((byteBuffer[i+1] != 124)&&(byteBuffer[i]!=2))
                                    {
                                        prefijos += (char)byteBuffer[i];
                                    }
                                    else
                                    {
                                        CantidadChar prefijo = new CantidadChar();
                                        prefijo.codPref = prefijos;
                                        if (prefijo.codPref[0] == '|')
                                        {
                                            string prueba = string.Empty;
                                            for (int j = 1; j < prefijo.codPref.Count(); j++)
                                            {
                                                prueba = prueba + prefijo.codPref[j];
                                            }
                                            prefijo.codPref = prueba;
                                        }
                                        diccionario.Add(caracter, prefijo);
                                        encontrado = false;
                                        prefijos = "";
                                    }
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
            return RedirectToAction("GeneraciónDelArchivoOriginal");
        }
        public ActionResult LecturaDescompresión()
        {
            return View();
        }
        public string Convertir(byte bit, string binario)
        {
            bit = Convert.ToByte(int.Parse(Convert.ToString(bit)));
            while (true)
            {
                if ((bit % 2) != 0)
                {
                    binario = "1" + binario;

                }
                else
                {
                    binario = "0" + binario;
                }
                bit /= 2;
                if (bit <= 0)
                {
                    break;
                }
            }
            if (binario.Count() <= 8)
            {
                while (binario.Count() != 8)
                {
                    binario = "0" + binario;
                }
            }
            return binario;
        }
        public ActionResult RazonFactor()
        {
            long tamañoOriginal = 500;
            long tamañoComprimido = new System.IO.FileInfo(RutaArchivos + "\\..\\Files\\archivoComprimido.huff").Length;
            long tamañoDescomprimido = new System.IO.FileInfo(RutaArchivos + "\\..\\Files\\archivoDescomprimido.huff").Length;


            double razon = Convert.ToInt32(tamañoOriginal) / Convert.ToInt32(tamañoComprimido);
            double factor = Convert.ToInt32(tamañoComprimido) / Convert.ToInt32(tamañoOriginal);
            double porcentaje = Convert.ToInt32(tamañoDescomprimido) / Convert.ToInt32(tamañoComprimido);

            using (StreamWriter datos = new StreamWriter(RutaArchivos + "\\..\\Files\\DatosCompresión.txt"))
            {
                datos.WriteLine("La razón de compresión es: " + Convert.ToString(razon));
                datos.WriteLine("El factor de compresión es: " + Convert.ToString(factor));
                datos.WriteLine("El porcentaje de compresión es: " + Convert.ToString(porcentaje));
            }

            return RedirectToAction("Download");
        }
        public ActionResult GeneraciónDelArchivoOriginal()
        {
            string binario = string.Empty;
            string texto = string.Empty;
            CantidadChar valor = new CantidadChar();
            foreach (byte bit in ASCII)
            {
                binario = string.Empty;
                binario = binario + Convertir(bit, binario);
                foreach (char car in binario)
                {
                    valor.codPref =valor.codPref+ car;
                    foreach (char Key in diccionario.Keys)
                    {
                        CantidadChar valor2 = GetAnyValue<CantidadChar>(Convert.ToByte(Key));
                        if (valor.codPref == valor2.codPref)
                        {
                            texto = texto + Key;
                            valor.codPref = string.Empty;
                        }
                    }
                }
            }
            using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\archivoDescomprimido.huff", FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(writeStream))
                {
                    int cantidadvecesbuffer = 0;
                    byte[] byteBufferfinal = new byte[100];
                    int cantidad = 0;
                    foreach (char carfinal in texto)
                    {
                        byteBufferfinal[cantidad] = Convert.ToByte(carfinal);
                        cantidad++;
                        if (cantidad == 100)
                        {
                            if (cantidadvecesbuffer == 0)
                            {
                                writer.Write(byteBufferfinal);
                                byteBufferfinal = new byte[100];
                                cantidadvecesbuffer++;
                                cantidad = 0;
                            }
                            else
                            {
                                writer.Seek(0, SeekOrigin.End);
                                writer.Write(byteBufferfinal);
                                byteBufferfinal = new byte[100];
                                cantidad = 0;
                            }
                        }
                    }
                    if ((byteBufferfinal[0] == 0)&&(byteBufferfinal[1] == 0))
                    {
                        // nada porque se aseguró que el buffer esté vacío
                    }
                    else
                    {
                        int contador0 = 0;
                        List<byte> ListAux = new List<byte>();
                        foreach (byte bit in byteBufferfinal)
                        {
                            if (bit == 0)
                            {
                                contador0++;
                            }
                            else
                            {
                                contador0 = 0;
                            }
                            if (contador0 != 3)
                            {
                                ListAux.Add(bit);
                            }
                            else
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    ListAux.Remove(ListAux.Last());
                                }
                                break;
                            }
                        }
                        byteBufferfinal = new byte[ListAux.Count()];
                        int j = 0;
                        foreach (byte bite in ListAux)
                        {
                            byteBufferfinal[j] = bite;
                            j++;
                        }
                        writer.Seek(0, SeekOrigin.End);
                        writer.Write(byteBufferfinal);
                    }
                }
            }
            return RedirectToAction("RazonFactor");
        }
    }
}