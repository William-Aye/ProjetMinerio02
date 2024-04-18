using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetMinerio02
{
    class Case
    {
        private bool gisement;
        private Ressource ress;
        private TapisRoulant roulot;
        private Usine usineCase;

        public Case()
        {
            gisement = false;
            ress = null;
            roulot = null;
            usineCase = null;
        }

        public Case(string nom)
        {
            gisement = true;
            ress = new Ressource(nom, 0);
            roulot = null;
            usineCase = null;
        }

        public bool Gisement { get { return gisement; } set { gisement = value; } }
        public Ressource Ress { get { return ress; } set { ress = value; } }
        public TapisRoulant Roulot
        {
            get { return roulot; }
            set { roulot = value; }
        }
        public Usine UsineCase
        {
            get { return usineCase; }
            set { usineCase = value; }
        }
    }
}
