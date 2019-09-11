﻿using System;
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
            Arbol.raíz= lista[0].Aux;
            string prefíjo = "";
            diccionario = Arbol.códigosPrefíjo(Arbol.raíz, diccionario, prefíjo);
            //separación de los caracteres para convertirlos a decimal y luego a ASCII
            List<char> cadena = new List<char>();
            string x = "";
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
                    if (cadena.Count() > 8)
                    {
                        x = "";
                        for(int i = 0; i < 8;i++)
                        {
                            x = x + cadena[0];
                            cadena.Remove(cadena[0]);
                        }
                        Byte DECABYTE;
                        var pref = x;
                        decimal Y = Convert.ToInt32(pref, 2);
                        DECABYTE = Convert.ToByte(Y);
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
                                if (byteBuffer[i + 1] == 13)
                                {
                                    if (byteBuffer[i - 1] == 10)
                                    {
                                        separación = true;
                                    }
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
                                    if ((byteBuffer[i] != 10) && (byteBuffer[i] != 13))
                                    {
                                        prefijo.codPref = prefijo.codPref + (char)byteBuffer[i];
                                    }
                                    else
                                    {
                                        diccionario.Add(caracter, prefijo);
                                        verdad = false;
                                        prefijo = new CantidadChar();
                                        caracter = ' ';
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
            for (int i = 0; i < 3; i++)
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
                binario = (bit % 2) + binario;
                bit = Convert.ToByte(Convert.ToInt32(bit / 2));
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