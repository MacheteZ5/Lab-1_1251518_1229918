using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1_Serie_1_1251518_1229918
{
    public interface LZWCompresor
    {
        Dictionary<string, int> LecturaArchivo(string ArchivoLeido, int bufferLengt, Dictionary<string, int> diccionario, int ContadorElementosDiccionario, string RutaArchivos);
        string ConvertToBinary(int numero);
        bool VerificarConversion(int numero, string binario);

        int CuantosBitsSeNecesitan(int numero);
    }
}
