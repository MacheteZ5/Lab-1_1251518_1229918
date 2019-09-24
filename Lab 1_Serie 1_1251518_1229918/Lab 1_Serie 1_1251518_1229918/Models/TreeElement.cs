using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Lab_1_Serie_1_1251518_1229918.Models
{
    public class TreeElement:IComparable
    {
        public char caracter { get; set; }
        public double probabilidad { get; set; }
        public NodoArbol Aux { get; set; }
        public int CompareTo(object obj)
        {
            TreeElement compareToObj = (TreeElement)obj;
            return this.probabilidad.CompareTo(compareToObj.probabilidad);
        }
    }
}