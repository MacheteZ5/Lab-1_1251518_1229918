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
        static Dictionary<char, object> diccionario = new Dictionary<char, object>();
        const int bufferLengt = 500;
        //lectura del archivo
        [HttpPost]
        //el siguiente ActionResult permite guardar el texto del archivo en un string 
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            using (var stream = new FileStream("C:\\Users\\mache\\Documents\\Segundo año\\Lab-1_1251518_1229918\\Archivos\\Actual.huff", FileMode.Open))
            {
                //te va a devolver un numero cualquiera
                using (var reader = new BinaryReader(stream))
                {
                    using (var writeStrime = new FileStream("C:\\Users\\mache\\Documents\\Segundo año\\Lab-1_1251518_1229918\\Archivos\\Actual2.huff", FileMode.Append))
                    {
                        using (var writer = new BinaryWriter(writeStrime))
                        {
                            var byteBuffer = new byte[bufferLengt];
                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLengt);
                                foreach(byte bit in byteBuffer)
                                {
                                    if (diccionario.Count == 0)
                                    {
                                        diccionario.Add((char)bit, 1);
                                    }
                                    else
                                    {
                                        if (diccionario.ContainsKey((char)bit))
                                        {
                                            int numero = GetAnyValue<int>(bit, diccionario);
                                            diccionario.Remove((char)bit);
                                            numero++;
                                            diccionario.Add((char)bit, numero);
                                        }
                                        else
                                        {
                                            diccionario.Add((char)bit, 1);
                                        }
                                    }
                                    caracterestotales++;
                                }
                            }
                        }
                    }
                }
            }
           return RedirectToAction("SeparaciónDelTexto");
        }
        private static T GetAnyValue<T>(byte strKey, Dictionary<char, object> diccionario)
        {
            object obj;
            T retType;

            diccionario.TryGetValue((char)strKey, out obj);
            try
            {
                retType = (T)obj;
            }
            catch
            {
                retType = default(T);
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
                double aux = (Convert.ToDouble(caracter.Value));
                elemento.caracter = caracter.Key;
                elemento.probabilidad =Convert.ToDouble((aux/caracterestotales));
                lista.Add(elemento);
            }
            lista.Sort();
            return RedirectToAction("Arbol");
        }
        //lista para introducir las llaves
        static List<char> ListAux = new List<char>();
        public ActionResult Arbol()
        {
            //creación del árbol
            Arbol Arbol = new Arbol();
            //NodoArbol árbol = new NodoArbol();
            int Repeticiones = lista.Count();
            for (int i = 0; i < Repeticiones + 1; i++)
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
                    for (int j = 0; j < 2; j++)
                    {
                        lista.Remove(lista[0]);
                    }
                    Aux = Auxiliar.ingresar(izquierdo, derecho, nombre);
                    Elementos_De_La_Lista elemento = new Elementos_De_La_Lista();
                    elemento.Aux = Aux;
                    elemento.probabilidad = Aux.probabilidad;
                    lista.Add(elemento);
                    lista.Sort();
                }
            }
            Arbol.raíz= lista[0].Aux;
            string prefíjo = "";
            Arbol.códigosPrefíjo(Arbol.raíz, ListAux, prefíjo);
            return View();
        }
    }
}