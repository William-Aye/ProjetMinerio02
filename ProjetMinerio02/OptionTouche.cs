using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjetMinerio02
{
	class OptionTouche
	{
		private string nom;
		private Key toucheAssocier;

		public OptionTouche(string nom, string toucheAssocier)
		{
			this.nom = nom;
			KeyConverter k = new KeyConverter();
			try { this.toucheAssocier = (Key)k.ConvertFromInvariantString(toucheAssocier); }
			catch { }
		}
		public string Nom { get { return nom; } }
		public Key ToucheAssocier { get { return toucheAssocier; } }

		public void TransformeEnTouche(string toucheAppuyer)
		{
			KeyConverter k = new KeyConverter();
			try { this.toucheAssocier = (Key)k.ConvertFromInvariantString(toucheAppuyer); }
			catch { }
		}
	}
}
