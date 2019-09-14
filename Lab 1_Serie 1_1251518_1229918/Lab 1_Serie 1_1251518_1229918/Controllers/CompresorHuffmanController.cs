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
        static List<byte> ListaByte = new List<byte>();
        const int bufferLengt = 1000;
        const int bufferLengt2 = 500;

        static string RutaArchivos = "";
        
        //lectura del archivo
        [HttpPost]
        //el siguiente ActionResult permite guardar el texto del archivo en un string 
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
                RutaArchivos = rutaDirectorioUsuario;
                Arbol send = new Arbol();
                send.recibirRutaArchivo(RutaArchivos);
                postedFile.SaveAs(ArchivoLeido);


                using (var stream = new FileStream(ArchivoLeido, FileMode.Open))
                {
                    //te va a devolver un numero cualquiera
                    using (var reader = new BinaryReader(stream))
                    {
                        var byteBuffer = new byte[bufferLengt];
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBuffer = reader.ReadBytes(bufferLengt);
                            foreach (byte bit in byteBuffer)
                            {
                                CantidadChar cantidad = new CantidadChar();
                                if (diccionario.Count == 0)
                                {
                                    cantidad.cantidad = 1;
                                    diccionario.Add((char)bit, cantidad);
                                }
                                else
                                {
                                    if (diccionario.ContainsKey((char)bit))
                                    {
                                        CantidadChar numero = GetAnyValue<int>(bit);
                                        diccionario.Remove((char)bit);
                                        cantidad.cantidad = numero.cantidad + 1;
                                        diccionario.Add((char)bit, cantidad);
                                    }
                                    else
                                    {
                                        cantidad.cantidad = 1;
                                        diccionario.Add((char)bit, cantidad);
                                    }
                                }
                                ListaByte.Add(bit);
                                caracterestotales++;
                            }
                        }
                    }
                }
            }
                return RedirectToAction("SeparaciónDelTexto");
            
        }

        public ActionResult Download()
        {
            string path = Server.MapPath("~/Files/");
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles("*.*");
            List<string> lista = new List<string>(files.Length);
            foreach (var item in files)
            {
                lista.Add(item.Name);
            }
            return View (lista);
        }
        public ActionResult DownloadFile(string filename)
        {
           
                string fullPath = Path.Combine(Server.MapPath("~/Files/"), filename);
           
            //return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);

            return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        public ActionResult LecturaCompresión()
        {
            return View();
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
        static List<Elementos_De_La_Lista> lista= new List<Elementos_De_La_Lista>();

        //Al momento de haber recibido el string del texto, habrá que separar caracter por caracter
        static int caracterestotales = 0;
        //retorna los valores que contiene la lista
        public ActionResult SeparaciónDelTexto()
        {
            //se ordenará por orden ascendente la lista
            var sorted = from entrada in diccionario orderby entrada.Value ascending select entrada;
            //se introducirán los porcentajes de los caracteres en la tabla
            foreach (var caracter in sorted)
            {
                Elementos_De_La_Lista elemento = new Elementos_De_La_Lista();
                double aux = (Convert.ToDouble(caracter.Value.cantidad));
                elemento.caracter = caracter.Key;
                elemento.probabilidad =Convert.ToDouble((aux/caracterestotales));
                lista.Add(elemento);
            }
            lista.Sort();
            return RedirectToAction("Arbol");
        }

        byte[] bytebuffer = new byte[100000];
        public ActionResult Arbol()
        {
            //creación del árbol
            Arbol Arbol = new Arbol();
            int Repeticiones = lista.Count();
            for (int i = 0; i < Repeticiones; i++)
            {
                if (lista.Count < 2)
                {
                    break;
                }
                else
                {
                    Arbol Auxiliar = new Arbol();
                    NodoArbol Aux = new NodoArbol();
                    NodoArbol izquierdo = new NodoArbol();
                    NodoArbol derecho = new NodoArbol();
                    string nombre = "n" + (i + 1);
                    if (lista[0].Aux == null && lista[1].Aux == null)
                    {
                        //hijo izquierdo
                        izquierdo.caracter = Convert.ToString(lista[0].caracter);
                        izquierdo.probabilidad = lista[0].probabilidad;
                        //hijo derecho
                        derecho.caracter = Convert.ToString(lista[1].caracter);
                        derecho.probabilidad = lista[1].probabilidad;
                    }
                    else
                    {
                        if (lista[0].Aux != null && lista[1].Aux == null)
                        {
                            //hijo izquierdo
                            izquierdo = lista[0].Aux;
                            //hijo derecho
                            derecho.caracter = Convert.ToString(lista[1].caracter);
                            derecho.probabilidad = lista[1].probabilidad;
                        }
                        else
                        {
                            if (lista[0].Aux == null && lista[1].Aux != null)
                            {
                                //hijo izquierdo
                                izquierdo.caracter = Convert.ToString(lista[0].caracter);
                                izquierdo.probabilidad = lista[0].probabilidad;
                                //hijo derecho
                                derecho = lista[1].Aux;
                            }
                            else
                            {
                                //hijo izquierdo
                                izquierdo = lista[0].Aux;
                                //hijo derecho
                                derecho = lista[1].Aux;
                            }
                        }
                    }
                    lista.Remove(lista[0]);
                    lista[0] = null;
                    Aux = Auxiliar.ingresar(izquierdo, derecho, nombre);
                    Elementos_De_La_Lista elemento = new Elementos_De_La_Lista();
                    elemento.Aux = Aux;
                    elemento.probabilidad = Aux.probabilidad;
                    if (lista.Count() > 1)
                    {
                        for (int j = 1; j < lista.Count(); j++)
                        {
                            if (lista[j].probabilidad > elemento.probabilidad)
                            {
                                lista[j - 1] = elemento;
                                break;
                            }
                            else
                            {
                                lista[j - 1] = lista[j];
                                lista[j] = null;
                                if (lista[lista.Count() - 1] == null)
                                {
                                    lista[lista.Count() - 1] = elemento;
                                }
                            }
                        }
                    }
                    else
                    {
                        lista[0] = elemento;
                    }
                }
            }
            Arbol.raíz= lista[0].Aux;
            string prefíjo = "";
            diccionario = Arbol.códigosPrefíjo(Arbol.raíz, diccionario, prefíjo);
            //separación de los caracteres para convertirlos a decimal y luego a ASCII
            List<char> cadena = new List<char>();
            string x = "";
            int cantidadbuffer = 0;
            foreach (byte bit in ListaByte)
            {
                if (diccionario.ContainsKey((char)bit))
                {
                    CantidadChar separación = new CantidadChar();
                    separación = GetAnyValue<int>(bit);
                    foreach (char caracter in separación.codPref)
                    {
                        cadena.Add(caracter);
                    }
                    if (cadena.Count() > 8|| cadena.Count() == 8)
                    {
                        x = "";
                        for (int i = 0; i < 8; i++)
                        {
                            x = x + cadena[0];
                            cadena.Remove(cadena[0]);
                        }
                        byte DECABYTE;
                        DECABYTE =ConvertirAByte(x);
                        
                        bytebuffer[cantidadbuffer] = DECABYTE;
                        cantidadbuffer++;
                    }
                }
                if (bytebuffer.Count() - 1 == cantidadbuffer)
                {
                    using (var writeStream = new FileStream(RutaArchivos+ "\\..\\Files\\archivo.huff", FileMode.Open))
                    {
                        using (var writer = new BinaryWriter(writeStream))
                        {
                            writer.Seek(0, SeekOrigin.End);
                            writer.Write(bytebuffer);
                            bytebuffer = new byte[10000];
                            cantidadbuffer = 0;
                        }
                    }
                }
            }
            if (x != "")
            {
                while (x.Count() != 8)
                {
                    x = x + "0";
                }
                using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\archivo.huff", FileMode.Open))
                {
                    using (var writer = new BinaryWriter(writeStream))
                    {
                        writer.Seek(0, SeekOrigin.End);
                        writer.Write(bytebuffer);
                    }
                }
            }
            return RedirectToAction("Download");
        }
        public byte ConvertirAByte(string cadena)
        {
            double numero = 0;
            int x = 0;
            int binario = 0;
            foreach (char car in cadena)
            {
                binario = Convert.ToInt32(car);
                if ( binario== 49)
                {
                    numero = numero + (Math.Pow(x,2));
                }
                x++;
            }
            return Convert.ToByte(numero);
        }














        //Método de descompresión
        //Lectura del archivo e introducir los códigos prefijos con sus respectivos caracteres al diccionario 
        static List<byte> ASCII = new List<byte>();
        [HttpPost]
        public ActionResult LecturaDescompresión(HttpPostedFileBase postedFile)
        {
            diccionario = new Dictionary<char, CantidadChar>();
            
            using (var stream = new FileStream(Convert.ToString(postedFile.FileName), FileMode.Open))
            {
                //te va a devolver un numero cualquiera
                using (var reader = new BinaryReader(stream))
                {
                    CantidadChar prefijo = new CantidadChar();
                    char caracter = ' ';
                    var byteBuffer = new byte[bufferLengt];
                    bool verdad = false;
                    bool separación = false;
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLengt2);
                        
                        for (int i = 0; i < byteBuffer.Count(); i++)
                        {
                            if (byteBuffer[i] == 45)
                            {
                                if (byteBuffer[i + 1] == 45)
                                {
                                   separación = true;
                                    i = i + 2;
                                }
                            }
                            if (separación==false)
                            {
                                //Lectura de la tabla
                                if (i < byteBuffer.Count())
                                {
                                    if (byteBuffer[i] == 49 || byteBuffer[i] == 48)
                                    {
                                        verdad = true;
                                    }
                                }
                                if (verdad == false)
                                {
                                    if (i < byteBuffer.Count() - 1)
                                    {
                                        if (byteBuffer[i + 1] == 124)
                                        {
                                            caracter = (char)byteBuffer[i];
                                            verdad = true;
                                            i++;
                                        }
                                    }
                                    if (i < byteBuffer.Count())
                                    {
                                        if (i > 0)
                                        {
                                            if (byteBuffer[i - 1] == 10)
                                            {
                                                caracter = (char)byteBuffer[i];
                                                i++;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (i == byteBuffer.Count() && prefijo.codPref != null)
                                    {
                                        diccionario.Add(caracter, prefijo);
                                        verdad = false;
                                        break;
                                    }
                                    if ((byteBuffer[i] != 10) && (byteBuffer[i] != 13)&&(byteBuffer[i]!=2))
                                    {
                                        prefijo.codPref = prefijo.codPref + (char)byteBuffer[i];
                                    }
                                    else
                                    {
                                        diccionario.Add(caracter, prefijo);
                                        verdad = false;
                                        prefijo = new CantidadChar();
                                        caracter = (char)byteBuffer[i];
                                    }
                                }
                            }
                            else
                            {
                                if (byteBuffer[i] != 0 && byteBuffer[i+1] != 0 && byteBuffer[i+2] != 0 && byteBuffer[i+3] != 0 && byteBuffer[i+4] != 0 && byteBuffer[i+5] != 0 && byteBuffer[i+6] != 0 && byteBuffer[i+7] != 0)
                                {
                                    ASCII.Add(byteBuffer[i]);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            for(int i = 0; i < 3; i++)
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
            while (bit > 0)
            {
                binario =(bit % 2)+binario;
                bit = Convert.ToByte(Convert.ToInt32(bit / 2));
            }
            if (binario.Count() < 8)
            {
                binario = "0" + binario;
            }
            return binario;
        }
        public ActionResult GeneraciónDelArchivoOriginal()
        {
            string texto = "";
            string clave = "";
            foreach (byte bit in ASCII)
            {
                string binario = "";
                binario = Convertir(bit, binario);
                CantidadChar valor = new CantidadChar();
                foreach(char c in binario)
                {
                    clave = clave + c;
                    valor.codPref = clave;
                    foreach(char Key in diccionario.Keys)
                    {
                        CantidadChar valor2 = GetAnyValue<CantidadChar>(Convert.ToByte(Key));
                        if (valor.codPref == valor2.codPref)
                        {
                            texto = texto + Key;
                            clave = "";
                        }
                    }
                }
            }
            return View();
        }
    }
}