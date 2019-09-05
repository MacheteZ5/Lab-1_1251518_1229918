using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Lab_1_Serie_1_1251518_1229918.Models
{
    public class Arbol
    {
        public NodoArbol raíz;

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
        public void generarArchivoDiccionario(char caracter, string prefijo)
        {
            string tablaCaracterPrefijo = caracter + "|" + prefijo;
            //se escribe en el archivo linea por linea el diccionario
            StreamWriter WriteReportFile = File.AppendText(AppDomain.CurrentDomain.BaseDirectory+"Archivos\\Actual.huff");
            WriteReportFile.WriteLine(tablaCaracterPrefijo);
            WriteReportFile.Close();
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
                        //se envian los valores y llaves al diccionario para generar la tabla de prefíjos
                        dic.Remove(Convert.ToChar(raíz.caracter));
                        dic.Add(Convert.ToChar(raíz.caracter), cantidad);
                    }
                }
                dic = códigosPrefíjo(raíz.hijoDerecho, dic, códigoprefíjo+1);
            }
            return dic;
        }
        public void generarArchivoASCII(string prefíjo)
        {
            Byte DECABYTE;
            char DECAASCII;
            var pref = prefíjo;
            decimal x = Convert.ToInt32(pref,2);
            DECABYTE = Convert.ToByte(x);
            DECAASCII = Convert.ToChar(DECABYTE);

            
            
        }
    }
}