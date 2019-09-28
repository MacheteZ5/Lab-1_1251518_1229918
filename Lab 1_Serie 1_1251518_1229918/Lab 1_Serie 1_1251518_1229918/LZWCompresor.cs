using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1_Serie_1_1251518_1229918
{
    public interface LZWCompresor
    {
        Dictionary<string, int> LecturaArchivo(string ArchivoLeido, int bufferLengt, Dictionary<string, int> diccionario, string RutaArchivos);
        Dictionary<string, int> CompararCaracteres(Dictionary<string, int> diccionario, ref List<string> ListaValores, int bufferLengt, string ArchivoLeido);

        byte[] CreaciónBufferEscritura(Dictionary<string, int> diccionario, List<string> ListaValores, List<byte> ListaBytesComprimidos, int valorCadena);
        int CuantosBitsSeNecesitan(int numero);
        string Descompress(Dictionary<string, int> diccionario, List<byte> ASCII, int CantidadBitsRequeridos, string RutaArchivos);
        int ConvertToDecimal(string binario);
    }
}
