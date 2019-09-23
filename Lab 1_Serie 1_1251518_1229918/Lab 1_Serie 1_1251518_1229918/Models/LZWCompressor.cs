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
                        ListaElementosDiccionario.Add(Convert.ToByte(elemento.Key[0]));
                        ListaElementosDiccionario.Add(Convert.ToByte('|'));
                        ListaElementosDiccionario.Add(Convert.ToByte(elemento.Value));
                    }
                    byte[] byteBuffer = new byte[ListaElementosDiccionario.Count()];
                    for(int i = 0; i < ListaElementosDiccionario.Count(); i++)
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
        public string ConvertToBinary(int numero)
        {
            string binario = string.Empty;
            while (numero!=0)
            {
                if ((numero % 2) != 0)
                {
                    binario = "1"+binario;

                }
                else
                {
                    binario = "0" + binario;
                }
                numero /= 2;
            }
            return binario;
        }
        public bool VerificarConversion(int numerooriginal, string binario)
        {
            int Conversor = 0;
            for(int x =binario.Length-1, y = 0; x >= 0; x--, y++)
            {
                if ((binario[x] == '0') || (binario[x] == '1'))
                {
                    Conversor += (int)(int.Parse(binario[x].ToString()) * Math.Pow(2, y));
                }
            }
            if (Conversor == numerooriginal)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            string texto = string.Empty;
            string binario = string.Empty;
            var previo = string.Empty;
            var actual = string.Empty;
            LZWCompressor LZW = new LZWCompressor();
            for (int i=0;i<ASCII.Count();i++)
            { 
                string Auxiliar = ConvertToBinary(ASCII[i]);
                if (Auxiliar.Count() < 8)
                {
                    Auxiliar = Auxiliar.PadLeft(8, '0');
                }
                binario += Auxiliar;
                int valor = LZW.ConvertToDecimal(binario);
                if (diccionario.Count() < 256)
                {
                    actual += diccionario.FirstOrDefault(x => x.Value == valor).Key;
                    if (!diccionario.ContainsKey(actual))
                    {
                        diccionario.Add(actual, diccionario.Count() + 1);
                        actual = string.Empty;
                        actual += diccionario.FirstOrDefault(x => x.Value == valor).Key;
                        texto += previo;
                        previo = string.Empty;
                        previo = actual;
                    }
                    else
                    {
                        previo += diccionario.FirstOrDefault(x => x.Value == valor).Key; 
                    }
                    binario = string.Empty;
                }
                else
                {
                    break;
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