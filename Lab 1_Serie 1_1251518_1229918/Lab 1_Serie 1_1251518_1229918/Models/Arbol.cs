using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace Lab_1_Serie_1_1251518_1229918.Models
{
    public class Arbol
    {
        public NodoArbol raíz;

        public Arbol()
        {
            raíz = null;
        }
        int cantidadNodos = 0;
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
        const int tamañoBuffer = 256;
        int espaciosUtilizados = 0;
        byte[] buffer = new byte[tamañoBuffer];
        int cantCaracteres = 0;
        public void generarArchivoDiccionario(string caracter, string prefijo)
        {
            cantCaracteres++;
            string linea = $"{caracter}|{prefijo}";
            while (buffer.Length >= espaciosUtilizados && espaciosUtilizados !=tamañoBuffer)
            {
                if(tamañoBuffer - espaciosUtilizados > linea.Length)
                {
                    for (int i = 0; i < linea.Length; i++)
                    {
                        buffer[espaciosUtilizados] = Convert.ToByte(linea[i]);
                        espaciosUtilizados++;

                    }
                    break;
                }
                break;
            }

            if(cantCaracteres == cantidadNodos || cantCaracteres == tamañoBuffer || tamañoBuffer - espaciosUtilizados < linea.Length)
            {
                
                using (var writeStream = new FileStream("C:\\Users\\Usuario\\Desktop\\nuevaprueba.huff", FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(writeStream))
                    {                       
                            writer.Write(buffer);
                        
                    }
                }

            }
            //using (var writeStream = new FileStream("C:\\Users\\Usuario\\Desktop\\machete.huff", FileMode.Open))
            //{
            //    using (var writer = new StreamWriter(writeStream))
            //    {
            //        writer.Write($"{caracter}|{prefijo}");
            //    }
            //}


        }
        
        public Dictionary<char,string> códigosPrefíjo(NodoArbol raíz, List<char> dic, Dictionary<char, string> diccionario, string códigoprefíjo)
        {
            if (raíz == null)
            {
                return diccionario;
            }
            else
            {
                diccionario = códigosPrefíjo(raíz.hijoIzquierdo, dic, diccionario, códigoprefíjo+0);
                if (raíz.hijoDerecho == null && raíz.hijoIzquierdo == null)
                {
                    foreach (char c in dic)
                    {
                        if (c == (Convert.ToChar(raíz.caracter)))
                        {
                            diccionario.Add(Convert.ToChar(raíz.caracter), códigoprefíjo);
                            //se envian los valores y llaves del diccionario a un metodo que permite la escritura en los archvios generados
                            cantidadNodos++;
                            generarArchivoDiccionario(raíz.caracter, códigoprefíjo);
                        }
                    }
                }
                diccionario = códigosPrefíjo(raíz.hijoDerecho, dic, diccionario, códigoprefíjo+1);
            }
            return diccionario;
        }
    }
}