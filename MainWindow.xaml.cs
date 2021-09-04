using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PiotrMay.WpfUtils;
using System.Windows.Threading;
using System;
using System.Collections;
using static Warcaby.PlanszaDlaDwóchGraczy;


namespace Warcaby
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string wersja = "1.2.0.1";

        MediaPlayer odtważacz;
        private string ścierzkaKliknięcia;
        private string ścierzkaWygranej;
        private string ścierzkaRemisu;

        private ISilnikDlaDwóchGraczy silnik = new WarcabySilnik(1, 8, 8);

        private int etapRuchu = 0;

        string[] nazwyGraczy = { "", "Biały", "Czarny" };
        //private DispatcherTimer timer;

        string[] ruchyGraczaBiałego;
        int indexNastepnegoRuchuB = 0;
        string[] ruchyGraczaCzarnego;
        int indexNastepnegoRuchuC = 0;

        private void uzgodnijZawartośćPlanszy()
        {
            for (int i = 0; i < silnik.SzerokośćPlanszy; i++)
                for (int j = 0; j < silnik.WysokośćPlanszy; j++)
                {
                    planszaKontrolka.ZaznaczRuch(new WspółrzędnePola(i, j),
                                                    (StanPola)silnik.PobierzStanPola(i, j));
                }

            przyciskKolorGracza.Background =
                planszaKontrolka.PędzelDlaStanu((StanPola)silnik.NumerGraczaWykonującegoNastępnyRuch);
            LiczbaBierekBiałych.Text = "Liczba bierek białych "  + silnik.LiczbaPólGracz1.ToString();
            LiczbaBierekCzarnych.Text = "Liczba bierek czarnych " + silnik.LiczbaPólGracz2.ToString();
            //MessageBox.Show("zagds" + silnik.NumerGraczaWykonującegoNastępnyRuch.ToString());
        }

        private static string symbolPola(int poziomo, int pionowo)
        {
            if (poziomo > 25 || pionowo > 8) return "(" + poziomo.ToString() + "," + pionowo.ToString() + ")";
            return "" + "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[poziomo] + "123456789"[pionowo];
        }

        private void Podniesienie(object sender, PlanszaEventArgs e, int numerGracza)
        {
            int klikniętePoziomo = e.WspółrzędnePola.Poziomo;
            int klikniętePionowo = e.WspółrzędnePola.Pionowo;
            
            if(silnik.PodnieśKamień(klikniętePoziomo, klikniętePionowo))
            {
                etapRuchu = 1;
                //MessageBox.Show("podniesiono " + klikniętePoziomo.ToString() + " " + klikniętePionowo.ToString());
                StanRuchu.Text = "Podniesiono";
                ostatnioKlikniętePoziomo = klikniętePoziomo;
                ostatnioKlikniętePionowo = klikniętePionowo;
            }
        }

        int ostatnioKlikniętePionowo=0;
        int ostatnioKlikniętePoziomo =0;

        private void Połorzenie(object sender, PlanszaEventArgs e,int numerGracza)
        {
            StanRuchu.Text = "zaczento";
            int klikniętePoziomo = e.WspółrzędnePola.Poziomo;
            int klikniętePionowo = e.WspółrzędnePola.Pionowo;
            //MessageBox.Show("podniesiono " + klikniętePoziomo.ToString() + " " + klikniętePionowo.ToString());
            if (silnik.PrzenieśKamień(klikniętePoziomo, klikniętePionowo))
            {
                //MessageBox.Show("połorzonmo " + klikniętePoziomo.ToString() + " " + klikniętePionowo.ToString());
                StanRuchu.Text = "Połorzono";
                uzgodnijZawartośćPlanszy();
                //przyciskKolorGracza.Background = PlanszaDlaDwóchGraczy.gra
                //lista ruchów
                switch (numerGracza)
                {
                    case 1:
                        listaRuchówBiały.Items.Add(symbolPola(ostatnioKlikniętePoziomo, ostatnioKlikniętePionowo)
                            + " > " + symbolPola(klikniętePoziomo, klikniętePionowo));
                        ruchyGraczaBiałego[indexNastepnegoRuchuB] = (symbolPola(ostatnioKlikniętePoziomo, ostatnioKlikniętePionowo)
                            + " > " + symbolPola(klikniętePoziomo, klikniętePionowo));
                        indexNastepnegoRuchuB++;
                        break;
                    case 2:
                        listaRuchówCzarny.Items.Add(symbolPola(ostatnioKlikniętePoziomo, ostatnioKlikniętePionowo)
                            + " > " + symbolPola(klikniętePoziomo, klikniętePionowo));
                        ruchyGraczaCzarnego[indexNastepnegoRuchuC] = (symbolPola(ostatnioKlikniętePoziomo, ostatnioKlikniętePionowo)
                            + " > " + symbolPola(klikniętePoziomo, klikniętePionowo));
                        indexNastepnegoRuchuC++;
                        break;
                }

                listaRuchówBiały.SelectedIndex = listaRuchówBiały.Items.Count - 1;
                listaRuchówCzarny.SelectedIndex = listaRuchówCzarny.Items.Count - 1;

                SytułacjaNaPlanszy sytułacjaNaPlanszy =
                    silnik.ZbadajSytułacjeNaPlanszy();
                bool koniecGry = false;
                switch (sytułacjaNaPlanszy)
                {
                    case SytułacjaNaPlanszy.BieżącyGraczNieMożeWykonaćRuchu:
                        MessageBox.Show("Gracz " + nazwyGraczy[silnik.NumerGraczaWykonującegoNastępnyRuch]
                            + " " + "musi oddać ruch");
                        silnik.Pasuj();
                        uzgodnijZawartośćPlanszy();
                        break;
                    case SytułacjaNaPlanszy.ObajGraczeNieMogąWykonaćRuchu:
                        MessageBox.Show("Obydwaj Gracze Nie mogą wykonać ruchu");
                        koniecGry = true;
                        break;
                    case SytułacjaNaPlanszy.JedenZGraczyNieMaPól:
                        koniecGry = true;
                        break;
                }

                //koniec gry- informacja o wyniku
                if (koniecGry)
                {
                    int numerZwycięzcy = silnik.NumerGraczaMającegoPrzewagę;
                    if (numerZwycięzcy != 0)
                    {
                        odtważacz.Open(new Uri(ścierzkaWygranej, UriKind.RelativeOrAbsolute));
                        odtważacz.Play();
                        MessageBox.Show("Wygrał gracz" + " " + nazwyGraczy[numerZwycięzcy], 
                            Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Remis", Title, 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        odtważacz.Open(new Uri(ścierzkaRemisu, UriKind.RelativeOrAbsolute));
                        odtważacz.Play();
                    }
                    if (MessageBox.Show("Cze zacząć gre od nowa?", "Reversi",
                        MessageBoxButton.YesNo, MessageBoxImage.Question,
                        MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        przygotowaniePlanszyDoNowejGry(1, silnik.SzerokośćPlanszy,
                            silnik.WysokośćPlanszy);
                    }
                    else
                    {
                        planszaKontrolka.IsEnabled = false;
                        przyciskKolorGracza.IsEnabled = false;
                    }
                }
                /*else
                {
                    odtważacz.Open(new Uri(scierzka1, UriKind.RelativeOrAbsolute));
                    odtważacz.Play();
                    if (graPrzeciwkoKomputerowi && silnik.NumerGraczaWykonującegoNastępnyRuch == 2)
                    {
                        if (timer == null)
                        {
                            timer = new DispatcherTimer();
                            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
                            timer.Tick +=
                                (_sender, _e) => { timer.IsEnabled = false; wykonajNajlepszyRuch(); };
                        }
                        timer.Start();
                        odtważacz.Open(new Uri(scierzka1, UriKind.RelativeOrAbsolute));
                        odtważacz.Play();
                    }
                }*/
            }
            etapRuchu = 0;
            uzgodnijZawartośćPlanszy();
        }

        private void PlanszaKontrolka_KliknięciePola(object sender, PlanszaEventArgs e)
        {
            //planszaKontrolka.CzyjePole(e.WspółrzędnePola);

            odtważacz.Open(new Uri(ścierzkaKliknięcia, UriKind.RelativeOrAbsolute));
            odtważacz.Play();
            int klikniętePoziomo = e.WspółrzędnePola.Poziomo;
            int klikniętePionowo = e.WspółrzędnePola.Pionowo;

            //wykonanie ruchu
            int zapamiętanyNumerGracza = silnik.NumerGraczaWykonującegoNastępnyRuch;

            if(etapRuchu == 0)
            {
                Podniesienie(sender, e, zapamiętanyNumerGracza);
            }
            else if(etapRuchu == 1)
            {
                Połorzenie(sender, e, zapamiętanyNumerGracza);
            }
            else
            {
                throw new Exception("Błąd podczas Kładzenia");
            }

             
            
        }

        private void przygotowaniePlanszyDoNowejGry(int numerGraczaRozpoczynającego,
                                           int szerokośćPlanszy = 8, int wysokośćPlanszy = 8)
        {
            silnik =
                new WarcabySilnik(numerGraczaRozpoczynającego,
                            szerokośćPlanszy, wysokośćPlanszy);
            listaRuchówBiały.Items.Clear();
            listaRuchówCzarny.Items.Clear();
            uzgodnijZawartośćPlanszy();
            planszaKontrolka.IsEnabled = true;
            przyciskKolorGracza.IsEnabled = true;
            ruchyGraczaBiałego = new string[60];
            indexNastepnegoRuchuB = 0;
            ruchyGraczaCzarnego = new string[60];
            indexNastepnegoRuchuC = 0;
        }
        
        private void PrzyciskKolorGracza_Click(object sender, RoutedEventArgs e)
        {
            StanRuchu.Text = "Kolej gracza" + silnik.NumerGraczaWykonującegoNastępnyRuch;
        }

        public MainWindow()
        {
            InitializeComponent();

            okno.Top = Ustawienia.CzytajTop();
            okno.Left = Ustawienia.CzytajLeft();

            odtważacz = new MediaPlayer();
            ścierzkaKliknięcia = System.IO.Path.GetFullPath("Klik.mp3");
            ścierzkaWygranej = System.IO.Path.GetFullPath("Win.mp3");
            ścierzkaRemisu = System.IO.Path.GetFullPath("Lose.mp3");
            ruchyGraczaBiałego = new string[60];
            ruchyGraczaCzarnego = new string[60];


            uzgodnijZawartośćPlanszy();
        }

        private void DostępneWkrótce()
        {
            MessageBox.Show("Ten element będzie dostępny niebawem", "Uwaga!", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void MenuItem_NowaGra_Click(object sender, RoutedEventArgs e)
        {
            przygotowaniePlanszyDoNowejGry(1,8,8);
        }

        private void MenuItem_ZapiszJako_Click(object sender, RoutedEventArgs e)
        {
            DostępneWkrótce();
        }

        private void MenuItem_Zapisz_Click(object sender, RoutedEventArgs e)
        {
            Ustawienia.SaveGameToNewFile(PlanszaDoTablicy(), silnik.NumerGraczaWykonującegoNastępnyRuch,
                ruchyGraczaBiałego, ruchyGraczaCzarnego);
        }

        private void MenuItem_Autor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Autor: Piotr May");
        }

        private void MenuItem_Zasady_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start("https://www.morele.net/wiadomosc/warcaby-jak-grac-zasady-jakie-wybrac/18478/");
            MessageBox.Show("Gracze na zmiane poruszają się pionami po ukosie,do przodu i do tyłu," +
                "\naby zbić pion przeciwnika, nalerzy nad nim przeskoczyć i wylądować na pustym polu, \n" +
                "możliwość tworzenia damek i bicia kilku pionów naraz zostanie dodana wkrótce.", 
                "Zasady Gry Warcaby", MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private void MenuItem_Zamknij_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Czy napewno chcesz zamknąć aplikcje?", "Zamykanie...",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void MenuItem_Wersja_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBox.Show("Wersja "+wersja + "\nO nowszą werse prosić u autora","Informacje o aplikacji");
        }

        private void MenuItem_Cofnij_Click(object sender, RoutedEventArgs e)
        {
            DostępneWkrótce();
        }

        private void MenuItem_Powtórz_Click(object sender, RoutedEventArgs e)
        {
            DostępneWkrótce();
        }

        private void MenuItem_ZmieńNazwy_Click(object sender, RoutedEventArgs e)
        {
            DostępneWkrótce();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_TATA_Click(object sender, RoutedEventArgs e)
        {
            odtważacz.Open(new Uri(ścierzkaWygranej, UriKind.RelativeOrAbsolute));
            odtważacz.Play();
        }

        private void MenuItem_Wczytaj_Click(object sender, RoutedEventArgs e)
        {
            silnik.wczytajPlansze(Ustawienia.LoadGameFromFile("Ustawienia.xml"));
            silnik.wszytajGracza(Ustawienia.LoadPlayerFormFile("Ustawienia.xml"));
            ruchyGraczaBiałego= Ustawienia.LoadListWhite("Ustawienia.xml");
            listaRuchówBiały.Items.Clear();
            int i = 0;
            while (ruchyGraczaBiałego[i] != null && ruchyGraczaBiałego[i] != " " 
                && ruchyGraczaBiałego[i] != ""&&i<60)
            {
                listaRuchówBiały.Items.Add(ruchyGraczaBiałego[i]);
                i++;
            }
            ruchyGraczaCzarnego = Ustawienia.LoadListBlack("Ustawienia.xml");
            listaRuchówCzarny.Items.Clear();
            i = 0;
            while (ruchyGraczaCzarnego[i] != null && ruchyGraczaCzarnego[i] != " "
                && ruchyGraczaCzarnego[i] != "" && i < 60)
            {
                listaRuchówCzarny.Items.Add(ruchyGraczaCzarnego[i]);
                i++;
            }
            uzgodnijZawartośćPlanszy();
        }

        private int[,] PlanszaDoTablicy()
        {
            int[,] planszaZapisana = new int[silnik.SzerokośćPlanszy, silnik.WysokośćPlanszy];
            for (int i = 0; i < silnik.SzerokośćPlanszy; i++)
                for (int j = 0; j < silnik.WysokośćPlanszy; j++)
                {
                    planszaZapisana[i, j] = silnik.PobierzStanPola(i, j);
                }
            return planszaZapisana;
        }

        private void Okno_Closed(object sender, EventArgs e)
        {
            int[,] planszaZapisana = PlanszaDoTablicy();

            Ustawienia.ZapiszPozycje(okno.Top, okno.Left);
            if(MessageBox.Show("Czy zapisać gre?","???", MessageBoxButton.YesNo,MessageBoxImage.Question
                , MessageBoxResult.Yes)== MessageBoxResult.Yes)
            {
                Ustawienia.SaveGameToNewFile(planszaZapisana, silnik.NumerGraczaWykonującegoNastępnyRuch,
                ruchyGraczaBiałego, ruchyGraczaCzarnego);
            }
            
        }
    }
}
