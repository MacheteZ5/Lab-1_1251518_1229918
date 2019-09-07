using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lab_1_Serie_1_1251518_1229918.Models
{
    public class CantidadChar : IComparable
    {
        public int cantidad { get; set; }
        public string codPref { get; set; }
        public int CompareTo(object obj)
        {
            CantidadChar compareToObj = (CantidadChar)obj;
            return this.cantidad.CompareTo(compareToObj.cantidad);
        }
    }
}