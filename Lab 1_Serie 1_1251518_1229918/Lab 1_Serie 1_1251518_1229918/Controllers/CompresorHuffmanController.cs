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

        //lectura del archivo
        [HttpPost]
        //el siguiente ActionResult permite guardar el texto del archivo en un string 
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            using (var stream = new FileStream(postedFile.FileName, FileMode.Open))
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
            return RedirectToAction("SeparaciónDelTexto");
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
        static List<Elementos_De_La_Lista> lista = new List<Elementos_De_La_Lista>();

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
                elemento.probabilidad = Convert.ToDouble((aux / caracterestotales));
                lista.Add(elemento);
            }
            lista.Sort();
            return RedirectToAction("Arbol");
        }

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
            Arbol.raíz = lista[0].Aux;
            string prefíjo = "";
            diccionario = Arbol.códigosPrefíjo(Arbol.raíz, diccionario, prefíjo);



            //Escritura del compresor códigos prefíjos convertidos a bytes
            using (var writeStream = new FileStream("C:\\Users\\mache\\Desktop\\nuevaprueba.huff", FileMode.Open))
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
                            binario = "";
                            binario = binario + car;
                        }
                        else
                        {
                            binario = binario + car;
                        }
                        if (cantidadbuffer== 500)
                        {
                            writer.Seek(0, SeekOrigin.End);
                            writer.Write(bytebuffer);
                            cantidadbuffer = 0;
                            bytebuffer = new byte[500];
                        }
                    }
                    if (binario != "")
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
                        writer.Seek(0, SeekOrigin.End);
                        writer.Write(bytebuffer);
                    }

                }
            }
            return View();
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
                using (var reader = new BinaryReader(stream))
                {
                    int conteo0 = 0;
                    string prefijos = "";
                    char caracter = ' ';
                    bool encontrado = false;
                    byte[] byteBuffer = new byte[10000];
                    bool separador = false;
                    bool demasiado = false;
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(10000);
                        if (demasiado == false)
                        {
                            for (int i = 0; i < byteBuffer.Count(); i++)
                            {
                                if (separador != true)
                                {
                                    if (byteBuffer[i] == 45)
                                    {
                                        if (byteBuffer[i + 1] == 45)
                                        {
                                            separador = true;
                                            i = i + 2;
                                        }
                                    }
                                    if (encontrado == false)
                                    {
                                        if (byteBuffer[i + 1] == 124)
                                        {
                                            caracter = (char)byteBuffer[i];
                                            encontrado = true;
                                            i++;
                                        }
                                    }
                                    else
                                    {
                                        if ((byteBuffer[i] != 13)&&(byteBuffer[i]!=2))
                                        {
                                            prefijos = prefijos + (char)byteBuffer[i];
                                        }
                                        else
                                        {
                                            CantidadChar prefijo = new CantidadChar();
                                            prefijo.codPref = prefijos;
                                            i++;
                                            diccionario.Add(caracter, prefijo);
                                            encontrado = false;
                                            prefijos = "";
                                        }
                                    }
                                }
                                else
                                {
                                    if (byteBuffer[i] == 0)
                                    {
                                        conteo0++;
                                    }
                                    ASCII.Add(byteBuffer[i]);
                                    if (conteo0 == 100)
                                    {
                                        demasiado = true;
                                    }
                                }
                                if (demasiado == true)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                }
            }
            for(int i = 0; i < 2; i++)
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
                bit /=2;
                if (bit <= 0)
                {
                    break;
                }
            }
            if (binario.Count() <= 8)
            {
                while (binario.Count()!=8)
                {
                    binario = "0" + binario;
                }
            }
            return binario;
        }
        public ActionResult GeneraciónDelArchivoOriginal()
        {
            string binario = "";
            string texto = "";
            //string clave = "";
            CantidadChar valor = new CantidadChar();
            foreach (byte bit in ASCII)
            {
                binario = "";
                binario = binario + Convertir(bit, binario);
                foreach (char car in binario)
                {
                    valor.codPref = valor.codPref + car;
                    foreach (char Key in diccionario.Keys)
                    {
                        CantidadChar valor2 = GetAnyValue<CantidadChar>(Convert.ToByte(Key));
                        if (valor.codPref == valor2.codPref)
                        {
                            texto = texto + Key;
                            //clave = "";
                            valor = new CantidadChar();
                        }
                    }
                }
            }
            using (var writeStream = new FileStream("C:\\Users\\mache\\Desktop\\nuevaprueba2.huff", FileMode.OpenOrCreate))
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
                }
            }
            return View();
        }
    }
}