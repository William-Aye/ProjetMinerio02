using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetMinerio02
{
	class ListeOption
	{
		private List<OptionTouche> listeTouche = new List<OptionTouche>();
		public ListeOption(string fichier)
		{
			StreamReader fichierLecture = new StreamReader(fichier);
			while (fichierLecture.Peek() > 0)
			{
				string[] tableauLecture = fichierLecture.ReadLine().Split(';');
				listeTouche.Add(new OptionTouche(tableauLecture[0], tableauLecture[1]));
			}
			fichierLecture.Close();
		}
		public List<OptionTouche> ListeTouche { get { return listeTouche; } }
		public void Sauvegarder(string fichier)
		{
			StreamWriter fichierEcriture = new StreamWriter(fichier);
			foreach (OptionTouche a in listeTouche)
			{
				fichierEcriture.WriteLine(a.Nom + ";" + a.ToucheAssocier);
			}
			fichierEcriture.Close();
		}
	}
}
