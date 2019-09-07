using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab_1_Serie_1_1251518_1229918.Models;
namespace Lab_1_Serie_1_1251518_1229918.Controllers
{
    public class CompresorHuffmanController : Controller
    {
        static string texto = "";
        //lectura del archivo
        [HttpPost]
        //el siguiente ActionResult permite guardar el texto del archivo en un string 
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            string ArchivoLeido = string.Empty;
            //el siguiente if permite seleccionar un archivo en específico
            if (postedFile != null)
            {
                string ruta = Server.MapPath("~/Archivos/");
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }
                //se toma la ruta y nombre del archivo
                ArchivoLeido = ruta + Path.GetFileName(postedFile.FileName);
                // se añade la extensión del archivo
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(ArchivoLeido);
                //se lee el archivo con texto que se desa comprimir
                string textoArchivo = System.IO.File.ReadAllText(ArchivoLeido);
                texto = textoArchivo;
            }
            return RedirectToAction("SeparaciónDelTexto");
        }
        public ActionResult Index()
        {
            return View();
        }
        //Diccionario donde se guardarán los caracteres por llaves y sus cantidades por sus valores
        static Dictionary<char, object> TablaCaracteres = new Dictionary<char, object>();
        //Al momento de haber recibido el string del texto, habrá que separar caracter por caracter
        static int CantidadRepetidos = 0;
        public ActionResult SeparaciónDelTexto()
        {
            //se separará la cantidad de characteres de todo el texto
            int caracterestotales = 0;
            foreach (char letra in texto)
            {
                if (TablaCaracteres.Count == 0)
                {
                    TablaCaracteres.Add(letra,1);
                    CantidadRepetidos++;
                }
                else
                {
                    if (TablaCaracteres.ContainsKey(letra))
                    {
                        int numero=GetAnyValue<int>(letra);
                        TablaCaracteres.Remove(letra);
                        numero++;
                        TablaCaracteres.Add(letra,numero);
                    }
                    else
                    {
                        TablaCaracteres.Add(letra, 1);
                        CantidadRepetidos++;
                    }
                }
                caracterestotales++;
            }

            //se ordenará por orden ascendente la lista
            var sorted = from entrada in TablaCaracteres orderby entrada.Value ascending select entrada;
            TablaCaracteres = new Dictionary<char, object>();
            //se introducirán los porcentajes de los caracteres en la tabla
            foreach (var caracter in sorted)
            {
                double valor = (Convert.ToDouble(caracter.Value) / caracterestotales)*100;
                //valor = Math.Truncate(valor)/1;
                TablaCaracteres.Add(caracter.Key,  valor);
                Elementos_De_La_Lista elemento = new Elementos_De_La_Lista();
                elemento.frecuencia = Convert.ToInt32(caracter.Value);
                lista.Add(elemento);
            }
            return RedirectToAction("Arbol");
        }
        //retorna los valores que contiene la lista
        private static T GetAnyValue<T>(char strKey)
        {
            object obj;
            T retType;

            TablaCaracteres.TryGetValue(strKey, out obj);
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

        //lista para introducir las llaves
        static List<Elementos_De_La_Lista> lista = new List<Elementos_De_La_Lista>();
        static List<char> ListAux = new List<char>();
        public ActionResult Arbol()
        {
            //introducir todos los caracteres con sus respectivas probabilidades a la lista y sus cantidades
            foreach(Elementos_De_La_Lista elem in lista)
            { 
                elem.caracter = Convert.ToChar(TablaCaracteres.Keys.First());
                elem.probabilidad = Convert.ToDouble(TablaCaracteres.Values.First());
                ListAux.Add(elem.caracter);
                TablaCaracteres.Remove(elem.caracter);
            }
            
            for(int i = 0; i < CantidadRepetidos+1; i++)
            {
                if (lista.Count < 2)
                {
                    break;
                }
                else
                {
                    Arbol arbol = new Arbol();
                    NodoArbol Aux = new NodoArbol();
                    NodoArbol izquierdo = new NodoArbol();
                    NodoArbol derecho = new NodoArbol();
                    string nombre = "n" + (i + 1);
                    if (lista[0].Aux == null&&lista[1].Aux==null)
                    {
                        //hijo izquierdo
                        izquierdo.caracter = Convert.ToString(lista[0].caracter);
                        izquierdo.frecuencia = lista[0].frecuencia;
                        izquierdo.probabilidad = lista[0].probabilidad;
                        //hijo derecho
                        derecho.caracter = Convert.ToString(lista[1].caracter);
                        derecho.frecuencia = lista[1].frecuencia;
                        derecho.probabilidad = lista[1].probabilidad;
                    }
                    else
                    {
                        if (lista[0].Aux != null && lista[1].Aux == null)
                        {
                            //hijo izquierdo
                            izquierdo =lista[0].Aux;
                            //hijo derecho
                            derecho.caracter = Convert.ToString(lista[1].caracter);
                            derecho.frecuencia = lista[1].frecuencia;
                            derecho.probabilidad = lista[1].probabilidad;
                        }
                        else
                        {
                            if(lista[0].Aux == null && lista[1].Aux != null)
                            {
                                //hijo izquierdo
                                izquierdo.caracter = Convert.ToString(lista[0].caracter);
                                izquierdo.frecuencia = lista[0].frecuencia;
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
                    Aux = arbol.ingresar(izquierdo, derecho, nombre);
                    Elementos_De_La_Lista elemento = new Elementos_De_La_Lista();
                    elemento.Aux = Aux;
                    elemento.probabilidad = Aux.probabilidad;
                    lista.Add(elemento);
                    lista.Sort();
                }
            }
            NodoArbol árbol = new NodoArbol();
            árbol = lista[0].Aux;
            Arbol X = new Arbol();
            Dictionary<char, string> códigoPrefíjos = new Dictionary<char, string>();
            string prefíjo = "";
            códigoPrefíjos=X.códigosPrefíjo(árbol, ListAux, códigoPrefíjos, prefíjo);

            return View();
        }
    }
}