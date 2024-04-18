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
    public partial class MINERIO : Page
    {
        public MINERIO()
        {
            InitializeComponent();
        }
        private void GenererClicker(object sender, RoutedEventArgs e)
        {
            MINERIOBackOffice.DonneesMap(Convert.ToInt32(tailleMapVal.Text), Convert.ToInt32(frequenceVal.Text));
            this.NavigationService.Navigate(new AffichageJeu());
        }
        private void RetourClicker(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}