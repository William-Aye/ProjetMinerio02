using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class ToolBarre : Page
    {
        int ligneMatrice, colonneMatrice;

        public ToolBarre()
        {
            InitializeComponent();
            GenererAfficherBarre();
        }
        private void GenererAfficherBarre()
        {
            //Roulant, Usine(4),
            int compteur = 0;
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 6; j++)
                {
                    string choix = "Sol02";
                    switch (compteur)
                    {
                        case 0: choix = "TapisRoulant"; break;
                        case 1: choix = "Boiler"; break;
                        case 2: choix = "MineurFer"; break;
                        case 3: choix = "FourPlaqueFer"; break;
                        case 4: choix = "MineurCharbon"; break;
                        case 6: choix = "Sauvegarder"; break;
                    }

                    Image bmpImage = new Image();
                    bmpImage.Source = new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Images/{choix}.png", UriKind.Absolute));
                    Grid.SetColumn(bmpImage, i);
                    Grid.SetRow(bmpImage, j);
                    grid.Children.Add(bmpImage);

                    Button MyControl = new Button();
                    MyControl.Name = $"Bouton{i}_{j}";
                    MyControl.FontSize = 1000;
                    MyControl.Content = choix;
                    MyControl.MouseEnter += SourisRentrer;
                    MyControl.Background = Brushes.Transparent;
                    MyControl.BorderBrush = Brushes.Transparent;
                    MyControl.Click += BoutonCliquer;
                    Grid.SetColumn(MyControl, i);
                    Grid.SetRow(MyControl, j);
                    grid.Children.Add(MyControl);
                    compteur++;
                }
        }
        private void BoutonCliquer(object sender, RoutedEventArgs e)
        {
            Button inter = sender as Button;
            string choix = (string)inter.Content;

			if (choix != "Sauvegarder")
            {
                MINERIOBackOffice.TypeChoisi = choix;
            }
            else
            {
                MINERIOBackOffice.SauvegardeMap();
			}
        }
        private void SourisRentrer(object sender, System.EventArgs e)
        {
            var test = (Button)sender;
            string[] position = test.Name.Split("_");
            colonneMatrice = Convert.ToInt32(position[1]);
            ligneMatrice = Convert.ToInt32(position[0].Substring(6));
        }
    }
}
