using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lab_1_Serie_1_1251518_1229918.Models
{
    //nodo y variables que contendrá el arbol
    public class NodoArbol
    {
        public string caracter;
        //public int frecuencia;
        public double probabilidad;
        public NodoArbol hijoDerecho;
        public NodoArbol hijoIzquierdo;
        public NodoArbol()
        {
            hijoDerecho = null;
            hijoIzquierdo = null;
        }
    }
}