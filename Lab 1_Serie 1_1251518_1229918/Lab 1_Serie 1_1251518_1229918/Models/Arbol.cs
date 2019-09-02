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
        public void generarArchivoDiccionario(string caracter, string prefijo)
        {
            string tablaCaracterPrefijo = caracter + "|" + prefijo;
            //se escribe en el archivo linea por linea el diccionario
            StreamWriter WriteReportFile = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "Archivos\\Actual.huff");
            WriteReportFile.WriteLine(tablaCaracterPrefijo);
            WriteReportFile.Close();

        }
        public void códigosPrefíjo(NodoArbol raíz, List<char> dic, string códigoprefíjo)
        {
            if (raíz == null)
            {
                return;
            }
            else
            {
                códigosPrefíjo(raíz.hijoIzquierdo, dic, códigoprefíjo+0);
                if (raíz.hijoDerecho == null && raíz.hijoIzquierdo == null)
                {
                    foreach (char c in dic)
                    {
                        if (c == (Convert.ToChar(raíz.caracter)))
                        {
                            //se envian los valores y llaves del diccionario a un metodo que permite la escritura en los archvios generados
                            generarArchivoDiccionario(raíz.caracter, códigoprefíjo);
                        }
                    }
                }
                códigosPrefíjo(raíz.hijoDerecho, dic, códigoprefíjo+1);
            }
            return;
        }
    }
}