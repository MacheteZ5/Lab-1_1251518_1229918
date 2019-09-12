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
        const int tamañoBuffer = 10000;
        int espaciosUtilizados = 0;
        byte[] buffer = new byte[tamañoBuffer];
        int cantCaracteres = 0;
        public void generarArchivoDiccionario(string caracter, string prefijo)
        {
            cantCaracteres++;
            string linea = $"{caracter}|{prefijo}";
            while (buffer.Length >= espaciosUtilizados && espaciosUtilizados != tamañoBuffer)
            {
                if (tamañoBuffer - espaciosUtilizados > linea.Length)
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

            if (cantCaracteres == cantidadNodos || cantCaracteres == tamañoBuffer || tamañoBuffer - espaciosUtilizados < linea.Length)
            {
                using (var writeStream = new FileStream("C:\\Users\\usuario\\Desktop\\nuevaprueba.huff", FileMode.Append))


                    }
                    break;
                }
                break;
            }

            if (cantCaracteres == cantidadNodos || cantCaracteres == tamañoBuffer || tamañoBuffer - espaciosUtilizados < linea.Length)
            {
                using (var writeStream = new FileStream("C:\\Users\\mache\\Desktop\\nuevaprueba.huff", FileMode.Append))

                {
                    //public virtual long Seek(int offset, System.IO.SeekOrigin origin);
                    using (var writer = new BinaryWriter(writeStream))
                    {
                        writer.Write(buffer);
                    }
                }
            }
        }

        public Dictionary<char, CantidadChar> códigosPrefíjo(NodoArbol raíz, Dictionary<char, CantidadChar> dic, string códigoprefíjo)

        public Dictionary<char, CantidadChar> códigosPrefíjo(NodoArbol raíz, Dictionary<char, CantidadChar>dic, string códigoprefíjo)

        {
            if (raíz == null)
            {
                return dic;
            }
            else
            {

                dic = códigosPrefíjo(raíz.hijoIzquierdo, dic, códigoprefíjo + 0);
=======
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
                        cantidadNodos++;
                        generarArchivoDiccionario(raíz.caracter, códigoprefíjo);
                    }
                }

                dic = códigosPrefíjo(raíz.hijoDerecho, dic, códigoprefíjo + 1);
            }
            return dic;

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