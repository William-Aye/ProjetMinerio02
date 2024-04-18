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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace ProjetMinerio02
{
    public partial class MINERIOBackOffice : Page
    {
        private bool gauche, droite, haut, bas;
        private int rotation = 0;
        private int vitesseCamera = 10;
        private int colonneMatrice, ligneMatrice;
        private Usine usineSelectionner = new Usine("boiler", null, "Images/Boiler.png"); //remplacer S par detectionS
        private TapisRoulant roulant = new TapisRoulant();
        private static Case[,] carte; //à remplacer par ProprieteMap
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private static int TailleMapVal = 0, FrequenceVal = 0;
        private List<Ressource> ressourceDispo = new List<Ressource>();
        private List<Recette> recettesDispo = new List<Recette>();
        private List<Usine> usineDispo = new List<Usine>();
        private ListeOption option = new ListeOption("Fichier_csv\\Option_Touche.csv");
        private Usine coffreVente = new Usine("CoffreVente", "CoffreVente.png");
        private Dictionary<Ressource, int> ressourceVendu = new Dictionary<Ressource, int>();
        private int argentTotal = 0;

        private int chronos = 0;

        public MINERIOBackOffice()
        {
            InitializeComponent();
            GenererEtAfficherMap();
            grid.Focus();
            gameTimer.Interval = TimeSpan.FromMilliseconds(25);
            gameTimer.Tick += GameTimerEvent;
            gameTimer.Start();
        }
        private void GenererEtAfficherMap()
        {
            ressourceDispo = Ressource.RechercherRessource();
            recettesDispo = Recette.RechercherRecette(ressourceDispo);
            usineDispo = Usine.RechercheUsineCSV(recettesDispo);

            carte = new Case[TailleMapVal, TailleMapVal];

            int nbDeGisement = (TailleMapVal * FrequenceVal);
            Random alea = new Random();
            while (nbDeGisement > 0)
            {
                for (int i = 0; i < TailleMapVal; i++)
                    for (int j = 0; j < TailleMapVal; j++)
                    {
                        int var = alea.Next(TailleMapVal);
                        if (nbDeGisement > 0 && var == 1)
                        {
                            if (carte[i, j] == null)
                            {

                                if (nbDeGisement > (TailleMapVal * FrequenceVal) / 2)
                                    carte[i, j] = new Case("GisementFer");
                                else carte[i, j] = new Case("GisementCharbon");
                                nbDeGisement--;
                            }
                            else if (!carte[i, j].Gisement)
                            {
                                if (nbDeGisement > (TailleMapVal * FrequenceVal) / 2)
                                    carte[i, j] = new Case("GisementFer");
                                else carte[i, j] = new Case("GisementCharbon");
                                nbDeGisement--;
                            }
                        }
                        else if (carte[i, j] == null) carte[i, j] = new Case();
                    }
            }

            for (int i = 0; i < TailleMapVal; i++)
            {
                RowDefinition row = new RowDefinition();
                ColumnDefinition column = new ColumnDefinition();
                grid.RowDefinitions.Add(row);
                grid.ColumnDefinitions.Add(column);
            }
            int count = 1;
            for (int i = 0; i < TailleMapVal; i++)
                for (int j = 0; j < TailleMapVal; j++)
                {
                    string sol = "Sol";
                    System.Windows.Controls.Image bmpImage = new System.Windows.Controls.Image();
                    bmpImage.Source = new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Images/{sol}.png", UriKind.Absolute));
                    Grid.SetRow(bmpImage, i);
                    Grid.SetColumn(bmpImage, j);
                    grid.Children.Add(bmpImage);

                    if (carte[i, j].Gisement)
                    {
                        bmpImage = new System.Windows.Controls.Image();
                        bmpImage.Source = new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Images/{carte[i, j].Ress.Nom}.png", UriKind.Absolute));
                        Grid.SetRow(bmpImage, i);
                        Grid.SetColumn(bmpImage, j);
                        grid.Children.Add(bmpImage);
                    }

                    Button MyControl1 = new Button();
                    MyControl1.Name = $"Bouton{i}_{j}";
                    MyControl1.MouseEnter += SourisRentrer;
                    MyControl1.Background = Brushes.Transparent;
                    MyControl1.BorderBrush = Brushes.Transparent;
                    MyControl1.Click += BoutonCliquer;
                    Grid.SetRow(MyControl1, i);
                    Grid.SetColumn(MyControl1, j);
                    grid.Children.Add(MyControl1);

                    count++;
                }
        }
        private static string typeChoisi;
        public static string TypeChoisi
        {
            get { return typeChoisi; }
            set
            {
                switch (value)
                {
                    case "TapisRoulant": typeChoisi = value; break;
                    case "Boiler": typeChoisi = value; break;
                    case "MineurFer": typeChoisi = value; break;
                    case "FourPlaqueFer": typeChoisi = value; break;
                    case "MineurCharbon": typeChoisi = value; break;
                    case "Sauvegarder": typeChoisi = value; break;
                }
            }

        }
        private void BoutonCliquer(object sender, RoutedEventArgs e)
        {
            #region CommunicationToolBarre
            if (typeChoisi == "TapisRoulant")
            {
                roulant = new TapisRoulant();
                usineSelectionner = null;
            }
            else
            {
                foreach (Usine usineActuelle in usineDispo)
                    if (usineActuelle.Nom == typeChoisi)
                        usineSelectionner = usineActuelle;
                roulant = null;
            }
            #endregion

            BitmapImage imageRotation = new BitmapImage();
            imageRotation.BeginInit();
            imageRotation.UriSource = new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Images/{typeChoisi}.png", UriKind.Absolute);

			if (typeChoisi == "TapisRoulant")
				switch (rotation)
                {
                case 90: imageRotation.Rotation = Rotation.Rotate90; roulant.Rotation90(1); break;
                case 180: imageRotation.Rotation = Rotation.Rotate180; roulant.Rotation90(2); break;
                case 270: imageRotation.Rotation = Rotation.Rotate270; roulant.Rotation90(3); break;
                }

            imageRotation.EndInit();

            System.Windows.Controls.Image bmpImage = new System.Windows.Controls.Image();
            bmpImage.BeginInit();
            bmpImage.Source = (ImageSource)imageRotation;
            bmpImage.EndInit();
            Grid.SetRow(bmpImage, ligneMatrice);
            Grid.SetColumn(bmpImage, colonneMatrice);
            grid.Children.Add(bmpImage);

            Button MyControl = new Button();
            MyControl.Name = $"Bouton{ligneMatrice}_{colonneMatrice}";
            MyControl.FontSize = 1000;
            MyControl.Content = typeChoisi;
            MyControl.MouseEnter += SourisRentrer;
            MyControl.Background = Brushes.Transparent;
            MyControl.BorderBrush = Brushes.Transparent;
            MyControl.Click += BoutonCliquer;
            Grid.SetRow(MyControl, ligneMatrice);
            Grid.SetColumn(MyControl, colonneMatrice);
            grid.Children.Add(MyControl);

            if (usineSelectionner != null)
                carte[ligneMatrice, colonneMatrice].UsineCase = usineSelectionner;
            else
                carte[ligneMatrice, colonneMatrice].Roulot = roulant;
        }
        private void SourisRentrer(object sender, System.EventArgs e)
        {
            var test = (Button)sender;
            string[] position = test.Name.Split("_");
            colonneMatrice = Convert.ToInt32(position[1]);
            ligneMatrice = Convert.ToInt32(position[0].Substring(6));
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            chronos++;
            if (chronos == 4) chronos = 0;

            #region DéplacementCaméra
            grid.RenderTransform = gridTranslation;
            int vitCamX = 0, vitCamY = 0;

            if (gauche) vitCamX = vitesseCamera;
            if (droite) vitCamX = -vitesseCamera;
            if (haut) vitCamY = vitesseCamera;
            if (bas) vitCamY = -vitesseCamera;

            DoubleAnimation anim1 = new DoubleAnimation(gridTranslation.X, gridTranslation.X + vitCamX, TimeSpan.FromMilliseconds(25));
            DoubleAnimation anim2 = new DoubleAnimation(gridTranslation.Y, gridTranslation.Y + vitCamY, TimeSpan.FromMilliseconds(25));
            gridTranslation.BeginAnimation(TranslateTransform.XProperty, anim1);
            gridTranslation.BeginAnimation(TranslateTransform.YProperty, anim2);
            #endregion

            #region ActualisationUsine
            Ressource ressRecueParTap = new Ressource();
            Ressource ressEnvoyeeParTap = new Ressource();
            for (int i = 0; i < carte.GetLength(0); i++)
                for (int j = 0; j < carte.GetLength(1); j++)
                {
                    Case caseDeCarte = carte[i, j];

                    if (caseDeCarte.UsineCase != null)
                    {
                        if (caseDeCarte.UsineCase.Nom.Substring(0, 6) == "Mineur"
                            && caseDeCarte.Gisement)
                        {
                            if (caseDeCarte.Ress == null)
                                caseDeCarte.Ress = caseDeCarte.UsineCase.Recette.ProduitTransformee;
                            if (caseDeCarte.Ress.Nom.Substring(8) == caseDeCarte.UsineCase.Nom.Substring(6))
                                caseDeCarte.Ress = caseDeCarte.UsineCase.Recette.ProduitTransformee;
                        }
                        else if (caseDeCarte.UsineCase.Nom == "CoffreVente"
                                 && caseDeCarte.Ress != null)
                        {
                            if (ressourceVendu.ContainsKey(caseDeCarte.Ress))
                            {
                                ressourceVendu[caseDeCarte.Ress] += 1;
                                argentTotal += caseDeCarte.Ress.ValeurMarch;
                                caseDeCarte.Ress = null;
                            }
                            else
                            {
                                ressourceVendu.Add(caseDeCarte.Ress, 1);
                                argentTotal += caseDeCarte.Ress.ValeurMarch;
                                caseDeCarte.Ress = null;
                            }
                        }
                        else if (caseDeCarte.Ress != null
                              && caseDeCarte.UsineCase.Recette.ProduitBrut.ContainsKey(caseDeCarte.Ress)
                              && caseDeCarte.UsineCase.NbRessource.ContainsKey(caseDeCarte.Ress))
                        {
                            int temp;
                            caseDeCarte.UsineCase.NbRessource.TryGetValue(caseDeCarte.Ress, out temp);
                            caseDeCarte.UsineCase.NbRessource[caseDeCarte.Ress] = temp + 1;
                        }
                        else if(caseDeCarte.Ress != null
                             && caseDeCarte.UsineCase.Recette.ProduitBrut.ContainsKey(caseDeCarte.Ress))
                            caseDeCarte.UsineCase.NbRessource.Add(caseDeCarte.Ress, 1);
                        int recetteRemplie = 0;
                        foreach (KeyValuePair<Ressource, int> ressourceActuelleUsine in caseDeCarte.UsineCase.NbRessource)
                        {
                            MessageBox.Show("test" + caseDeCarte.UsineCase.Recette.ProduitBrut.ContainsKey(ressourceActuelleUsine.Key), "test", MessageBoxButton.OK);
                            if (!caseDeCarte.Gisement
                                && caseDeCarte.UsineCase.Recette != null
                                && caseDeCarte.UsineCase.Recette.ProduitBrut.ContainsKey(ressourceActuelleUsine.Key)
                                && caseDeCarte.UsineCase.Recette.ProduitBrut[ressourceActuelleUsine.Key] == ressourceActuelleUsine.Value)
                                recetteRemplie++;
                        }
                        if (caseDeCarte.UsineCase.Nom.Substring(0, 6) != "Mineur"
                            && caseDeCarte.UsineCase.Recette != null
                            && recetteRemplie == caseDeCarte.UsineCase.Recette.ProduitBrut.Count)
                        {
                            caseDeCarte.UsineCase.NbRessource.Clear();
                            caseDeCarte.Ress = caseDeCarte.UsineCase.Recette.ProduitTransformee;
                        }
                    }
                    //Tout ce code permet de vérifier si dans chaque usine il y a le bon nombre de ressource pour pouvoir effectuer la rec 
                    //a assez de ressource pour la recette on créée le produit transformée et ont vide les ressources brute, si par contre on n'   pas a            z 
                    //de res
                    #endregion

                    #region Actualisation Tapis
                    else if (caseDeCarte.Roulot != null)
                    {
                        #region Vers Tapis
                        if (chronos%4 == 0)
                            for (int k = 0; k < 4; k++)
                            {
                                int iPlus = i, jPlus = j;
                                char res = 'A';
                                switch (k)
                                {
                                    case 0: res = 'N'; iPlus--; break;
                                    case 1: res = 'E'; jPlus--; break;
                                    case 2: res = 'S'; iPlus++; break;
                                    case 3: res = 'O'; jPlus++; break;
                                }
                                if (caseDeCarte.Ress != null && (caseDeCarte.Roulot.Ress.Count < 4))
                                {
                                    caseDeCarte.Roulot.AjoutRess(caseDeCarte.Ress);
                                    caseDeCarte.Ress = null;
                                }
                                else if ((iPlus > 0 && iPlus < TailleMapVal && jPlus > 0 && jPlus < TailleMapVal)
                                    && (carte[iPlus, jPlus].UsineCase != null && carte[i, j].Roulot.DirectionSortie != res)
                                    && (carte[iPlus, jPlus].Ress == carte[iPlus, jPlus].UsineCase.Recette.ProduitTransformee)
                                    && (caseDeCarte.Roulot.Ress.Count < 4))
                                {
                                    caseDeCarte.Roulot.AjoutRess(carte[iPlus, jPlus].Ress);
                                    carte[iPlus, jPlus].Ress = null;
                                }
                            }
                        #endregion

                        #region Depuis Tapis
                        if (caseDeCarte.Roulot.TimerTap[3] > 0)
                        {
                            int iPlus = i, jPlus = j;
                            switch (caseDeCarte.Roulot.DirectionSortie)
                            {
                                case 'N': iPlus--; break;
                                case 'E': jPlus--; break;
                                case 'S': iPlus++; break;
                                case 'O': jPlus++; break;
                            }

                            if (carte[iPlus, jPlus].Ress == null)
                            {
								if (carte[iPlus, jPlus].UsineCase != null)
                                {
                                    foreach (KeyValuePair<Ressource, int> ProduitBrutRecette in carte[iPlus, jPlus].UsineCase.Recette.ProduitBrut)
                                        if (caseDeCarte.Roulot.DerniereRess() == ProduitBrutRecette.Key)
                                        {
                                            carte[iPlus, jPlus].Ress = caseDeCarte.Roulot.SupprRess();
                                            caseDeCarte.Roulot.TimerTap[3] = 0;
                                        }
                                }
                                else if (carte[iPlus, jPlus].Roulot != null)
                                {
									carte[iPlus, jPlus].Ress = caseDeCarte.Roulot.SupprRess();
									caseDeCarte.Roulot.TimerTap[3] = 0;
								}
							}
                        }
                        #endregion

                        #region Affichage Ressource sur Tapis
                        for (int k = 0; k < caseDeCarte.Roulot.Ress.Count; k++)
                        {
                            Ressource stockage = caseDeCarte.Roulot.Ress.Dequeue();
                            if (stockage.Nom != "")
                            {
                                System.Windows.Controls.Image bmpImage = new System.Windows.Controls.Image();
                                bmpImage.Source = new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Images/{stockage.Nom}.png", UriKind.Absolute));
                                bmpImage.Height = 20;
                                Grid.SetRow(bmpImage, i);
                                Grid.SetColumn(bmpImage, j);
                                grid.Children.Add(bmpImage);

                                Button MyControl1 = new Button();
                                MyControl1.Name = $"Bouton{i}_{j}";
                                MyControl1.MouseEnter += SourisRentrer;
                                MyControl1.Background = Brushes.Transparent;
                                MyControl1.BorderBrush = Brushes.Transparent;
                                MyControl1.Click += BoutonCliquer;
                                Grid.SetRow(MyControl1, i);
                                Grid.SetColumn(MyControl1, j);
                                grid.Children.Add(MyControl1);
                            }
                            caseDeCarte.Roulot.Ress.Enqueue(stockage);
                        }

                        caseDeCarte.Roulot.TempsQuiPasse();
                        #endregion
                    }
                }
            #endregion
        }

        #region DeplacementCam
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == option.ListeTouche[0].ToucheAssocier)
                haut = true;
            else if (e.Key == option.ListeTouche[1].ToucheAssocier)
                bas = true;
            else if (e.Key == option.ListeTouche[2].ToucheAssocier)
                gauche = true;
            else if (e.Key == option.ListeTouche[3].ToucheAssocier)
                droite = true;
            else if (e.Key == option.ListeTouche[4].ToucheAssocier)
            {
                if (rotation == 270)
                    rotation = 0;
                else
                    rotation += 90;
            }
            else if (e.Key == option.ListeTouche[5].ToucheAssocier)
                vitesseCamera = 20;
        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == option.ListeTouche[0].ToucheAssocier)
                haut = false;
            else if (e.Key == option.ListeTouche[1].ToucheAssocier)
                bas = false;
            else if (e.Key == option.ListeTouche[2].ToucheAssocier)
                gauche = false;
            else if (e.Key == option.ListeTouche[3].ToucheAssocier)
                droite = false;
            else if (e.Key == option.ListeTouche[5].ToucheAssocier)
                vitesseCamera = 10;
        }
        #endregion

        #region Zoom
        private void Zoom(double val)
        {
            if (gridZoom.ScaleX > 0.9 && gridZoom.ScaleY > 0.9)
            {
                gridZoom.ScaleX += val;
                gridZoom.ScaleY += val;
            }
            else
            {
                gridZoom.ScaleX = 1;
                gridZoom.ScaleY = 1;
            }
            if (gridZoom.ScaleX < 10 && gridZoom.ScaleY < 10)
            {
                gridZoom.ScaleX += val;
                gridZoom.ScaleY += val;
            }
            else
            {
                gridZoom.ScaleX = 9.9;
                gridZoom.ScaleY = 9.9;
            }
        }
        private void RouletteSouris(object sender, MouseWheelEventArgs e)
        {
            Zoom((e.Delta > 0) ? 0.1 : -0.1);
        }
        #endregion

        #region Transfert Données
        public static void SauvegardeMap()
        {
            StreamWriter fichierEcriture = new StreamWriter("Fichier_csv/SauvegardeMap.csv");
            for (int i = 0; i < TailleMapVal; i++)
            {
                for (int j = 0; j < TailleMapVal; j++)
                {
                    fichierEcriture.Write(carte[i, j].Gisement + ";");
                    if (carte[i, j].Ress != null)
                        fichierEcriture.Write(carte[i, j].Ress.Nom + ";");
                    if (carte[i, j].Roulot != null)
                        fichierEcriture.Write(carte[i, j].Roulot.DirectionSortie + ";");
                    if (carte[i, j].UsineCase != null)
                        fichierEcriture.Write(carte[i, j].UsineCase.Nom);
                    fichierEcriture.WriteLine();
                }
                fichierEcriture.WriteLine("FinLigne");
            }
            fichierEcriture.Close();
        }
        public static void DonneesMap(int tailleMapVal, int frequenceVal)
        {
            if (TailleMapVal == 0
                && FrequenceVal == 0)
            {
                TailleMapVal = tailleMapVal;
                FrequenceVal = frequenceVal;
            }
        }
        #endregion
    }
}
