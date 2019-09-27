﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Lab_1_Serie_1_1251518_1229918.Models
{
    public class LZWCompressor : LZWCompresor
    {
        public Dictionary<string, int> LecturaArchivo(string ArchivoLeido, int bufferLengt, Dictionary<string, int> diccionario, string RutaArchivos)
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
                            llave += (char)bit;
                            if (!diccionario.ContainsKey(llave))
                            {
                                diccionario.Add(llave, diccionario.Count() + 1);
                            }
                            llave = string.Empty;
                        }
                        //se escribe el diccionario original en el archivo
                    }
                    EscribirDiccionarioArchivo(diccionario, RutaArchivos);
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
                        ListaElementosDiccionario.Add(Convert.ToByte(elemento.Key[0]));
                        ListaElementosDiccionario.Add(Convert.ToByte('|'));
                        ListaElementosDiccionario.Add(Convert.ToByte(elemento.Value));
                    }
                    byte[] byteBuffer = new byte[ListaElementosDiccionario.Count()];
                    for (int i = 0; i < ListaElementosDiccionario.Count(); i++)
                    {
                        byteBuffer[i] = ListaElementosDiccionario[i];
                    }
                    writer.Write("\r\n");
                    writer.Write(byteBuffer);
                    writer.Write("\r\n");
                    writer.Write('-');
                    writer.Write('-');
                    writer.Write("\r\n");
                }
            }
        }

        public Dictionary<string, int> CompararCaracteres(Dictionary<string, int> diccionario, ref List<string> ListaValores, int bufferLengt, string ArchivoLeido)
        {
            var previo = string.Empty;
            var actual = string.Empty;
            using (var stream = new FileStream(ArchivoLeido, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLengt];
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLengt);
                        for (int i = 0; i < byteBuffer.Count(); i++)
                        {
                            actual += (char)byteBuffer[i];
                            if (!diccionario.ContainsKey(actual))
                            {
                                diccionario.Add(actual, diccionario.Count() + 1);
                                ListaValores.Add(Convert.ToString(diccionario[previo], 2));
                                actual = string.Empty;
                                actual += (char)(byteBuffer[i]);
                                previo = actual;
                            }
                            else
                            {
                                previo = actual;
                            }
                        }
                    }
                    if (previo != string.Empty)
                    {
                        ListaValores.Add(Convert.ToString(diccionario[previo], 2));
                    }
                }
            }
            return diccionario;
        }
        public byte[] CreaciónBufferEscritura(Dictionary<string, int> diccionario, List<string> ListaValores, List<byte> ListaBytesComprimidos, int valorCadena)
        {
            var auxiliar = string.Empty;
            foreach (var valor in ListaValores)
            {
                var binario = valor;
                binario = binario.PadLeft(valorCadena, '0');
                foreach (char caracter in binario)
                {
                    auxiliar += caracter;
                    if (auxiliar.Count() == 8)
                    {
                        var x = Convert.ToInt32(auxiliar.Substring(0, 8), 2);
                        byte DECABYTE = Convert.ToByte(x);
                        ListaBytesComprimidos.Add(DECABYTE);
                        auxiliar = string.Empty;
                    }
                }
            }
            if (auxiliar != string.Empty)
            {
                auxiliar = auxiliar.PadRight(8, '0');
                var x = Convert.ToInt32(auxiliar.Substring(0, 8), 2);
                byte DECABYTE = Convert.ToByte(x);
                ListaBytesComprimidos.Add(DECABYTE);
            }
            byte[] bytebuffer = new byte[ListaBytesComprimidos.Count()];
            for (int i = 0; i < ListaBytesComprimidos.Count(); i++)
            {
                bytebuffer[i] = ListaBytesComprimidos[i];
            }
            return bytebuffer;
        }
    
        public int CuantosBitsSeNecesitan(int numero)
        {
            int i = 0;
            while (Math.Pow(2,i) < numero)
            {
                i++;
            }
            return i;
        }
        public string Descompress(Dictionary<string, int> diccionario, List<byte> ASCII, int CantidadBitsRequeridos)
        {
            var texto = string.Empty;
            var previo = string.Empty;
            var actual = string.Empty;
            var AuxiliarBitsRequeridos = string.Empty;
            var binario = string.Empty;
            LZWCompressor LZW = new LZWCompressor();
            var ASCIIABYTE = new List<int>();
            foreach(byte bit in ASCII)
            {
                binario = Convert.ToString(bit, 2);
                binario = binario.PadLeft(8, '0');
                foreach (char caracter in binario)
                {
                    AuxiliarBitsRequeridos += caracter;
                    if (AuxiliarBitsRequeridos.Length == CantidadBitsRequeridos)
                    {
                        int valor = Convert.ToInt32(AuxiliarBitsRequeridos.Substring(0, CantidadBitsRequeridos), 2);
                        var receptor = diccionario.LastOrDefault(x => x.Value == valor).Key;
                        if (diccionario.Count() < valor)
                        {
                            valor = diccionario.Count();
                            receptor = diccionario.LastOrDefault(x => x.Value == valor).Key;
                            receptor = receptor.Substring(0, 1);
                        }
                        foreach (char j in receptor)
                        {
                            

                            actual += j;
                            if (!diccionario.ContainsKey(actual))
                            {
                                texto += previo;
                                diccionario.Add(actual, diccionario.Count() + 1);
                                actual = string.Empty;
                                actual += j;
                            }
                            previo = actual;
                        }
                        AuxiliarBitsRequeridos = string.Empty;
                    }
                }
            }
            return texto;
        }
        public int ConvertToDecimal(string binario)
        {
            int numero = 0;
            for (int x = binario.Length - 1, y = 0; x >= 0; x--, y++)
            {
                if ((binario[x] == '0') || (binario[x] == '1'))
                {
                    numero += (int)(int.Parse(binario[x].ToString()) * Math.Pow(2, y));
                }
            }
            return numero;
        }

        
    }
}