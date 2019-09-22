using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Lab_1_Serie_1_1251518_1229918.Models;
namespace Lab_1_Serie_1_1251518_1229918
{
    public interface HuffmanInterface
    {
        Dictionary<char, CantidadChar> LecturaArchivoCompresion(Dictionary<char, CantidadChar> diccionario, string ArchivoLeido, int bufferLengt, ref List<byte> ListaByte);
        List<TreeElement> OrdenamientoDelDiccionario(Dictionary<char, CantidadChar> diccionario, List<TreeElement> lista, List<byte> ListaByte);
        NodoArbol TreeCreation(List<TreeElement> lista);
    }
}
