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
        const int bufferLengt = 1000;
        //lectura del archivo
        [HttpPost]
        //el siguiente ActionResult permite guardar el texto del archivo en un string 
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            using (var stream = new FileStream("C:\\Users\\mache\\Documents\\BIBLIA COMPLETA.txt"/*KH 3.2.jpg*/, FileMode.Open))
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
            byte[] buffer = new byte[bufferLengt];
            Arbol.raíz= lista[0].Aux;
            string prefíjo = "";
            diccionario = Arbol.códigosPrefíjo(Arbol.raíz, diccionario, prefíjo);
            return View();
        }


        //Método de descompresión
        //Lectura del archivo e introducir los códigos prefijos con sus respectivos caracteres al diccionario 
        [HttpPost]
        public ActionResult LecturaDescompresión(HttpPostedFileBase postedFile)
        {
            diccionario = new Dictionary<char, CantidadChar>();
            using (var stream = new FileStream("C:\\Users\\mache\\Documents\\Segundo año\\Lab-1_1251518_1229918\\Lab 1_Serie 1_1251518_1229918\\Lab 1_Serie 1_1251518_1229918\\Archivos\\Actual.huff", FileMode.Open))
            {
                //te va a devolver un numero cualquiera
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLengt];
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLengt);
                        char caracter = ' ';
                        CantidadChar prefijo = new CantidadChar();
                        bool verdad = false;
                        for (int i = 0; i < byteBuffer.Count()+1; i++)
                        {
                            if (verdad == false)
                            {
                                if (byteBuffer[i + 1] == 124)
                                {
                                    caracter = (char)byteBuffer[i];
                                    verdad = true;
                                    i++;
                                }
                            }
                            else
                            {
                                if (i == byteBuffer.Count())
                                {
                                    diccionario.Add(caracter, prefijo);
                                    verdad = false;
                                    break;
                                }
                                if (byteBuffer[i] != 13)
                                {
                                    prefijo.codPref = prefijo.codPref + (char)byteBuffer[i];
                                }
                                else
                                {
                                    diccionario.Add(caracter, prefijo);
                                    verdad = false;
                                    prefijo = new CantidadChar();
                                }
                            }
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
    }
}