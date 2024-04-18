using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjetMinerio02
{
	public partial class Option : Page
	{
		private ListeOption option = new ListeOption("Fichier_csv\\Option_Touche.csv");
		public Option()
		{
			InitializeComponent();
			CreationPage();
		}
		private Button boutonSauvegarde = new Button();
		private void CreationPage()
		{
			List<TextBox> TextBoxPourSauvegardee = new List<TextBox>();
			for (int i = 0; i < option.ListeTouche.Count(); i++)
			{
				TextBox nomTextBox = new TextBox() { Name = "nom" + i, Text = $"{option.ListeTouche[i].Nom}", Background = Brushes.LightGray, IsHitTestVisible = false };
				stackPanelOption.Children.Add(nomTextBox);

				TextBox lectureUtilisateur = new TextBox() { Name = "lecture" + i, Text = $"{option.ListeTouche[i].ToucheAssocier}" };
				lectureUtilisateur.TextChanged += ToucheModifier;
				stackPanelOption.Children.Add(lectureUtilisateur);
			}
			boutonSauvegarde.Content = "Sauvegarder les changements";
			boutonSauvegarde.Click += SauvegarderClicker;
			stackPanelOption.Children.Add(boutonSauvegarde);

			Button boutonRetour = new Button();
			boutonRetour.Content = "Retour";
			boutonRetour.Click += RetourClicker;
			stackPanelOption.Children.Add(boutonRetour);
			//On créée les boutons programatiquement pour qu'ils soit en bas du stackpanel
		}
		private void ToucheModifier(object sender, TextChangedEventArgs e)
		{
			TextBox textBoxActuelle = sender as TextBox;
			option.ListeTouche[Convert.ToInt32(textBoxActuelle.Name.Substring(7))].TransformeEnTouche(textBoxActuelle.Text);
			boutonSauvegarde.Background = Brushes.LightGray;
			//A chaque modification on rechange la couleur du bouton de sauvegarde pour bien voir qu'on a modifier quelque chose
		}
		private void RetourClicker(object sender, RoutedEventArgs e)
		{
			this.NavigationService.GoBack();
		}
		private void SauvegarderClicker(object sender, RoutedEventArgs e)
		{
			Button bouttonSauvegardee = sender as Button;
			option.Sauvegarder("Fichier_csv\\Option_Touche.csv");
			for (int i = 0; i < stackPanelOption.Children.Count; i++)
			{
				if (stackPanelOption.Children[i].GetType() == typeof(TextBox))
				{
					TextBox textBoxActuelle = stackPanelOption.Children[i] as TextBox;
					if (textBoxActuelle.Name.Substring(0, 3) != "nom")
						//ce if ne permet de prendre que les textBox ou l'ont peut rentrer des données.
						textBoxActuelle.Text = Convert.ToString(option.ListeTouche[Convert.ToInt32(textBoxActuelle.Name.Substring(7))].ToucheAssocier);
				}

			}
			//Quand on sauvegarde on montre a l'utilisateur quel sont les nouvelle touche et celle qui ont donc bien été modifier ou pas
			bouttonSauvegardee.Background = Brushes.Green;
			//Met le bouton a vert quand les modification on bien été sauvegarder
		}
	}
}
