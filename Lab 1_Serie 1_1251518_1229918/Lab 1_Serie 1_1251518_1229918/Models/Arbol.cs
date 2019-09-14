using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Lab_1_Serie_1_1251518_1229918.Models;
namespace Lab_1_Serie_1_1251518_1229918.Models
{
    public class Arbol
    {
        static string RutaArchivos = "";
        public void recibirRutaArchivo(string ruta)
        {
            RutaArchivos = ruta;
        }
        public NodoArbol raíz;
        int cantidadNodos = 0;
        public Arbol()
        {
            raíz = null;
        }
        public NodoArbol ingresar(NodoArbol izquierdo, NodoArbol derecho, string nombre)
        {
            raíz = new NodoArbol();
            raíz.hijoDerecho = derecho;
            raíz.hijoIzquierdo = izquierdo;
            raíz.caracter = nombre;
            raíz.probabilidad = derecho.probabilidad + izquierdo.probabilidad;
            return raíz;
        }
        //Método para escribir el caracter  su codigo prefijo en un archivo .huff
        const int tamañoBuffer = 100000;
        int espaciosUtilizados = 0;
        byte[] buffer = new byte[tamañoBuffer];
        int cantCaracteres = 0;
        public void generarArchivoDiccionario(string caracter, string prefijo, Dictionary<char, CantidadChar> dic)
        {
            cantCaracteres++;
            string linea = $"{caracter}|{prefijo}";
            for (int i = 0; i < linea.Length; i++)
            {
                buffer[espaciosUtilizados] = Convert.ToByte(linea[i]);
                espaciosUtilizados++;
            }
            if (cantCaracteres ==dic.Count())
            {
                buffer[espaciosUtilizados] = Convert.ToByte('-');
                buffer[espaciosUtilizados+1] = Convert.ToByte('-');
                int conteo = 0;
                using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\archivoComprimido.huff", FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(writeStream))
                    {
                        for (int j = 0; j < buffer.Count(); j++)
                        {
                            if(j == espaciosUtilizados)
                            {
                                writer.Write("\r\n");
                                writer.Write(buffer[j]);
                                writer.Write(buffer[j+1]);
                                writer.Write("\r\n");
                                break;
                            }
                            if (buffer[j + 1] == 124)
                            {
                                if (conteo != 0)
                                {
                                    writer.Write("\r\n");
                                    writer.Write(buffer[j]);
                                }
                                else
                                {
                                    writer.Write(buffer[j]);
                                    conteo++;
                                }
                            }
                            else
                            {
                                writer.Write(buffer[j]);
                            }
                        }
                    }
                }
            }
        }
        public Dictionary<char, CantidadChar> códigosPrefíjo(NodoArbol raíz, Dictionary<char, CantidadChar>dic, string códigoprefíjo)
        {
            if (raíz == null)
            {
                return dic;
            }
            else
            {
                dic = códigosPrefíjo(raíz.hijoIzquierdo, dic, códigoprefíjo+0);
                if (raíz.hijoDerecho == null && raíz.hijoIzquierdo == null)
                {
                    if (dic.ContainsKey(Convert.ToChar(raíz.caracter)))
                    {
                        CantidadChar cantidad = new CantidadChar();
                        cantidad.codPref = códigoprefíjo;
                        dic.Remove(Convert.ToChar(raíz.caracter));
                        dic.Add(Convert.ToChar(raíz.caracter), cantidad);
                        cantidadNodos++;
                        generarArchivoDiccionario(raíz.caracter, códigoprefíjo, dic);
                    }
                }
                dic = códigosPrefíjo(raíz.hijoDerecho, dic, códigoprefíjo+1);
            }
            return dic;
        }
        
    }
}