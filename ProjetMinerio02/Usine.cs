using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProjetMinerio02
{
    class Usine
    {
        private string nom;
        private Recette recette;
        private Dictionary<Ressource, int> nbRessource;
        private string imageAfficher;
        public Usine(string nom, string imageAfficher)
        {
            this.nom = nom;
            this.recette = null;
            this.nbRessource = new Dictionary<Ressource, int>();
            this.imageAfficher = imageAfficher;
        }
        public Usine(string nom, Recette recette, string imageAfficher)
        {
            this.nom = nom;
            this.recette = recette;
            this.nbRessource = new Dictionary<Ressource, int>();
            this.imageAfficher = imageAfficher;
        }
        public string Nom { get { return nom; } }
        public Recette Recette { get { return recette; } set { recette = value; } }
        public Dictionary<Ressource, int> NbRessource { get { return nbRessource; } }
        public string ImageAfficher { get { return imageAfficher; } }
        public static List<Usine> RechercheUsineCSV(List<Recette> recettesDispo)
        {
            return RecursiviteRechercheUsineCSV(new StreamReader("Fichier_csv/Usines_Info.csv"), new List<Usine>(), recettesDispo);
        }
        public static List<Usine> RecursiviteRechercheUsineCSV(StreamReader fichierLecture, List<Usine> usineRetour, List<Recette> recettesDispo)
        {
            string recu = fichierLecture.ReadLine();
            if (recu == null)
            {
                fichierLecture.Close();
                return usineRetour;
            }
            string[] recuTab = recu.Split(";");
            if (recuTab[0].Substring(0, 6) == "Mineur")
            {
                usineRetour.Add(new Usine(recuTab[0], new Recette(recuTab[0], new Ressource("Minerai" + recuTab[0].Substring(6), 0, "Gisement" + recuTab[0].Substring(6) + ".png")), recuTab[0] + ".png"));
            }

            foreach (Recette a in recettesDispo)
                if (a.Nom == recuTab[0])
                    usineRetour.Add(new Usine(recuTab[0], a, recuTab[1]));
            return RecursiviteRechercheUsineCSV(fichierLecture, usineRetour, recettesDispo);
        }
    }

    class Recette
    {
        private string nom;
        private Dictionary<Ressource, int> produitBrut;
        private Ressource produitTransformee;
        public Recette(string nom, Ressource produitTransformee)
        {
            this.nom = nom;
            this.produitBrut = null;
            this.produitTransformee = produitTransformee;
        }
        public Recette(string nom, Dictionary<Ressource, int> produitBrut, Ressource produitTransformee)
        {
            this.nom = nom;
            this.produitBrut = produitBrut;
            this.produitTransformee = produitTransformee;
        }
        public string Nom { get { return nom; } }
        public Dictionary<Ressource, int> ProduitBrut { get { return produitBrut; } }
        public Ressource ProduitTransformee { get { return produitTransformee; } }
        public static List<Recette> RechercherRecette(List<Ressource> ressourceDispo)
        {
            return RecursiviteRechercherRecette(new StreamReader("Fichier_csv/Recette.csv"), new List<Recette>(), ressourceDispo);
        }
        public static List<Recette> RecursiviteRechercherRecette(StreamReader fichierLecture, List<Recette> recetteRetour, List<Ressource> ressourceDispo)
        {
            string recu = fichierLecture.ReadLine();
            if (recu == null)
            {
                fichierLecture.Close();
                return recetteRetour;
            }
            string[] recuTab = recu.Split(";");
            Dictionary<Ressource, int> produitBrutRecette = new Dictionary<Ressource, int>();
            Ressource produitTransformeeRecette = new Ressource();
            for (int i = 2; i < recu.Length; i += 2)
            {
                if (recuTab[i] == "Sortie")
                {
                    foreach (Ressource a in ressourceDispo)
                        if (a.Nom == recuTab[i])
                            produitTransformeeRecette = a;
                    break;
                }
                foreach (Ressource a in ressourceDispo)
                    if (a.Nom == recuTab[i])
                        produitBrutRecette.Add(a, Convert.ToInt32(recuTab[i + 1]));
            }
            recetteRetour.Add(new Recette(recuTab[0], produitBrutRecette, produitTransformeeRecette));
            return RecursiviteRechercherRecette(fichierLecture, recetteRetour, ressourceDispo);
        }
    }

    class TapisRoulant
    {
        private char directionSortie;
        private int[] timerTap;
        private Queue<Ressource> ressources;
        public TapisRoulant()
        {
            directionSortie = 'S';
            timerTap = new int[4];
            ressources = new Queue<Ressource>();
        }
        public TapisRoulant(char dirSortie)
        {
            if (dirSortie == 'N' || dirSortie == 'E' || dirSortie == 'S' || dirSortie == 'O')
                directionSortie = dirSortie;
            else directionSortie = 'S';
            timerTap = new int[4];
            ressources = new Queue<Ressource>();
        }

        public char DirectionSortie { get { return directionSortie; } }
        public int[] TimerTap { get { return timerTap; } set { timerTap = value; } }
        public Queue<Ressource> Ress { get { return ressources; } set { ressources = value; } }

        public void Rotation90(int angle)
        {
            while (angle > 0)
            {
                switch (directionSortie)
                {
                    case 'N': directionSortie = 'O'; break;
                    case 'O': directionSortie = 'S'; break;
                    case 'S': directionSortie = 'E'; break;
                    case 'E': directionSortie = 'N'; break;
                }
                angle--;
            }
        }
        public char Oppose()
        {
            char res = 'A';
            switch (directionSortie)
            {
                case 'A': res = 'A'; break;
                case 'N': res = 'S'; break;
                case 'O': res = 'E'; break;
                case 'S': res = 'N'; break;
                case 'E': res = 'E'; break;
            }
            return res;
        }
        public void AjoutRess(Ressource ress)
        {
            if (ress != null && timerTap[0] == 0)
            {
                timerTap[0] = 4;
                if (ress != null)
                    if (ressources.Count < 3)
                        ressources.Enqueue(ress);
            }
        }
        public Ressource SupprRess()
        {
            return ressources.Dequeue();
        }
        public Ressource DerniereRess()
        {
            Ressource res = null;
			for (int i = 0; i < ressources.Count; i++)
            {
                res = ressources.Dequeue();
				ressources.Enqueue(res);
			}
            return res;
		}
        public void TempsQuiPasse()
        {
            if (timerTap[3] == 0)
            {
                int stockage = 0;
                for (int i = 0; i < 3; i++)
                {
                    stockage = timerTap[i];
                    if (stockage > 0) timerTap[i + 1] = stockage--;
                    timerTap[i] = 0;
                }
            }
        }
    }

    class Ressource
    {
        private string nom;
        private int valeurMarch; //ValeurMarchande
        private string imageAfficher;
        public Ressource()
        {
            this.nom = "";
            this.valeurMarch = 0;
            this.imageAfficher = "";
        }
        public Ressource(string nom, int valeur)
        {
            this.nom = nom;
            this.valeurMarch = valeur;
            this.imageAfficher = "";
        }
        public Ressource(string nom, int valeur, string imageAfficher)
        {
            this.nom = nom;
            this.valeurMarch = valeur;
            this.imageAfficher = imageAfficher;
        }
        public string Nom { get { return nom; } set { nom = value; } }
        public int ValeurMarch { get { return valeurMarch; } }
        public string ImageAfficher { get { return imageAfficher; } }

        public static List<Ressource> RechercherRessource()
        {
            string[] recu;
            List<Ressource> ressourceRetour = new List<Ressource>();
            StreamReader fichierLecture = new StreamReader("Fichier_csv/Ressources_Info.csv");
            while (fichierLecture.Peek() > 0)
            {
                recu = fichierLecture.ReadLine().Split(";");
                ressourceRetour.Add(new Ressource(recu[0], Convert.ToInt32(recu[1]), recu[2]));
            }
            fichierLecture.Close();
            return ressourceRetour;
        }
    }
}