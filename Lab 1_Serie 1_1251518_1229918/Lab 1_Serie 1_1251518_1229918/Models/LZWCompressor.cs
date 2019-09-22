using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Lab_1_Serie_1_1251518_1229918.Models
{
    public class LZWCompressor : LZWCompresor
    {
        public Dictionary<string, int> LecturaArchivo(string ArchivoLeido, int bufferLengt, Dictionary<string, int> diccionario, int ContadorElementosDiccionario, string RutaArchivos)
        {
            using (var stream = new FileStream(ArchivoLeido, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLengt];
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLengt);
                        string llave = string.Empty;
                        foreach (byte bit in byteBuffer)
                        {
                            //añadir al diccionario
                            llave = string.Empty;
                            llave += (char)bit;
                            if (!diccionario.ContainsKey(llave))
                            {
                                ContadorElementosDiccionario++;
                                diccionario.Add(llave, ContadorElementosDiccionario);
                            }
                        }
                        //se escribe el diccionario original en el archivo
                        EscribirDiccionarioArchivo(diccionario, RutaArchivos);
                    }
                }
            }
            return diccionario;
        }
        //metodo para escribir el diccionario original en el archivo 
        void EscribirDiccionarioArchivo(Dictionary<string, int> diccionario, string RutaArchivos)
        {
            //se utilizó una lista para que se escriba el numero de bytes correctos en el archivo y que no escriba más bytes debido por los espacios sobrantes del diccionario
            var ListaElementosDiccionario = new List<byte>();
            using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\archivoComprimido.lzw", FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(writeStream))
                {
                    foreach (var elemento in diccionario)
                    {
                        string linea = $"{elemento.Key}|{Convert.ToByte(elemento.Value)}";
                        for (int i = 0; i < linea.Length; i++)
                        {
                            ListaElementosDiccionario.Add(Convert.ToByte(linea[i]));
                        }
                    }
                    byte[] byteBuffer = new byte[ListaElementosDiccionario.Count()];
                    for(int i = 0; i < ListaElementosDiccionario.Count(); i++)
                    {
                        byteBuffer[i] = ListaElementosDiccionario[i];
                    }
                    writer.Write(byteBuffer);
                    writer.Write("\r\n");
                    writer.Write('-');
                    writer.Write('-');
                    writer.Write("\r\n");
                }
            }
        }
    }
}