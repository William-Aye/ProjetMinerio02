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
	public partial class MenuPrincipal : Page
	{
		public MenuPrincipal()
		{
			InitializeComponent();
            MenuPrincipalFenetre.Background = new ImageBrush(new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Images\\Background_Minerio.jpg", UriKind.RelativeOrAbsolute)));
        }
        private void MenuClick(object sender, RoutedEventArgs e)
		{
			Button inter = (Button)sender;
			switch ((string)inter.Content)
			{
				case "Jouer":	this.NavigationService.Navigate(new Uri("MINERIO.xaml", UriKind.Relative)); break;
				case "Option":	this.NavigationService.Navigate(new Uri("Option.xaml", UriKind.Relative)); break;
				case "Quitter":	Environment.Exit(1); break;
			}

		}
	}
}
