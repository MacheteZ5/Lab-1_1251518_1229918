using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Lab_1_Serie_1_1251518_1229918.Models
{
    public class Huffman : HuffmanInterface
    {
        public Dictionary<char, CantidadChar> LecturaArchivoCompresion(Dictionary<char, CantidadChar> diccionario, string ArchivoLeido, int bufferLengt, ref List<byte> ListaByte)
        {
            //el siguiente if permite seleccionar un archivo en específico
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

                            if (!diccionario.ContainsKey((char)bit))
                            {
                                cantidad.cantidad = 1;
                                diccionario.Add((char)bit, cantidad);
                            }
                            else
                            {
                                diccionario[(char)bit].cantidad++;
                            }
                            ListaByte.Add(bit);
                        }
                    }
                }
            }
            return diccionario;
        }

        public List<TreeElement> OrdenamientoDelDiccionario(Dictionary<char, CantidadChar> diccionario, List<TreeElement> ListaProbabilidades, List<byte> ListaByte)
        {
            //se ordenará por orden ascendente la lista
            var sorted = from entrada in diccionario orderby entrada.Value ascending select entrada;
            //se introducirán los porcentajes de los caracteres en la tabla
            foreach (var caracter in sorted)
            {
                TreeElement elemento = new TreeElement();
                double aux = (Convert.ToDouble(caracter.Value.cantidad));
                elemento.caracter = caracter.Key;
                elemento.probabilidad = Convert.ToDouble((aux / ListaByte.Count()));
                ListaProbabilidades.Add(elemento);
            }
            ListaProbabilidades.Sort();
            return ListaProbabilidades;
        }

        public NodoArbol TreeCreation(List<TreeElement> lista)
        {
            Arbol Auxiliar = new Arbol();
            NodoArbol Aux = new NodoArbol();
            NodoArbol izquierdo = new NodoArbol();
            NodoArbol derecho = new NodoArbol();
            int Repeticiones = lista.Count();
            for (int i = 0; i < Repeticiones; i++)
            {
                if (lista.Count < 2)
                {
                    break;
                }
                else
                {
                    Auxiliar = new Arbol();
                    Aux = new NodoArbol();
                    izquierdo = new NodoArbol();
                    derecho = new NodoArbol();
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
                    TreeElement elemento = new TreeElement();
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
            return lista[0].Aux;
        }
    }
}