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
using PiotrMay.WpfUtils;

namespace Warcaby
{
    /// <summary>
    /// Interaction logic for PlanszaDlaDwóchGraczy.xaml
    /// </summary>
    public partial class PlanszaDlaDwóchGraczy : UserControl
    {
        #region Typy pomocnicze
        public enum StanPola { Puste = 0, Gracz1 = 1, Gracz2 = 2, DamaGracza1 = 3, DamaGracza2 = 4 }

        public int Szerokość
        {
            get
            {
                return szerokość;
            }
            set
            {
                twórzPlansze(value, wysokość);
            }
        }

        public int Wysokość
        {
            get
            {
                return wysokość;
            }
            set
            {
                twórzPlansze(szerokość, value);
            }
        }


        public struct WspółrzędnePola
        {
            public int Poziomo, Pionowo;

            public WspółrzędnePola(int poziomo, int pionowo)
            {
                this.Poziomo = poziomo;
                this.Pionowo = pionowo;
            }
        }
        #endregion

        private int szerokość, wysokość;
        private StanPola[,] planszaStany;
        private Button[,] planszaPrzyciski;

        private void twórzPlansze(int szerokość, int wysokość)
        {
            this.szerokość = szerokość;
            this.wysokość = wysokość;

            //podział siatki na wiersze i kolumny
            planszaSiatka.ColumnDefinitions.Clear();
            for (int i = 0; i < szerokość; i++)
                planszaSiatka.ColumnDefinitions.Add(new ColumnDefinition());
            planszaSiatka.RowDefinitions.Clear();
            for (int j = 0; j < wysokość; j++)
                planszaSiatka.RowDefinitions.Add(new RowDefinition());

            //twożenie tabliby stanów
            planszaStany = new StanPola[szerokość, wysokość];
            for (int i = 0; i < szerokość; i++)
                for (int j = 0; j < wysokość; j++)
                    planszaStany[i, j] = StanPola.Puste;

            //tworzenie przycisków
            planszaPrzyciski = new Button[szerokość, wysokość];
            for (int i = 0; i < szerokość; i++)
                for (int j = 0; j < wysokość; j++)
                {
                    Button przycisk = new Button();
                    przycisk.Margin = new Thickness(0);
                    planszaSiatka.Children.Add(przycisk);
                    Grid.SetColumn(przycisk, i);
                    Grid.SetRow(przycisk, j);
                    przycisk.Tag = new WspółrzędnePola { Poziomo = i, Pionowo = j };
                    przycisk.Click += new RoutedEventHandler(
                        (s, e) =>
                        {
                            Button klikniętyPrzycisk = s as Button;
                            WspółrzędnePola współrzędne =
                            (WspółrzędnePola)klikniętyPrzycisk.Tag;
                            int klikPoziomo = współrzędne.Poziomo;
                            int klikniętePionowo = współrzędne.Pionowo;
                            onKliknięciePola(współrzędne);
                        });
                    planszaPrzyciski[i, j] = przycisk;
                }
            zmieńKoloryWszystkichPrzycisków();
        }

        public PlanszaDlaDwóchGraczy()
        {
            InitializeComponent();

            twórzPlansze(8, 8);
        }

        #region kolory
        private SolidColorBrush pędzelPustegoPola = Brushes.LightCoral;
        private SolidColorBrush pędzelGracza1 = Brushes.BlanchedAlmond;
        private SolidColorBrush pędzelGracza2 = Brushes.SlateGray;
        private SolidColorBrush pędzelDamkiGracza1 = Brushes.BlanchedAlmond;
        private SolidColorBrush pędzelDamkiGracza2 = Brushes.SlateGray;

        public SolidColorBrush PędzelDlaStanu(StanPola stanPola)
        {
            switch (stanPola)
            {
                default:
                case StanPola.Puste: return pędzelPustegoPola;
                case StanPola.Gracz1: return pędzelGracza1;
                case StanPola.Gracz2: return pędzelGracza2;
                case StanPola.DamaGracza1: return pędzelDamkiGracza1;
                case StanPola.DamaGracza2: return pędzelDamkiGracza2;
            }
        }

        private void zmieńKoloryWszystkichPrzycisków()
        {
            for (int i = 0; i < szerokość; i++)
                for (int j = 0; j < wysokość; j++)
                {
                    planszaPrzyciski[i, j].Background = PędzelDlaStanu(planszaStany[i, j]);
                }
        }

        public Color KolorPustegoPola
        {
            get
            {
                return pędzelPustegoPola.Color;
            }
            set
            {
                pędzelPustegoPola = new SolidColorBrush(value);
                zmieńKoloryWszystkichPrzycisków();
            }
        }
        public Color KolorGracza1
        {
            get
            {
                return pędzelGracza1.Color;
            }
            set
            {
                pędzelGracza1 = new SolidColorBrush(value);
                zmieńKoloryWszystkichPrzycisków();
            }
        }
        public Color KolorGracza2
        {
            get
            {
                return pędzelGracza2.Color;
            }
            set
            {
                pędzelGracza2 = new SolidColorBrush(value);
                zmieńKoloryWszystkichPrzycisków();
            }
        }
        public Color KolorDamkiGracza1
        {
            get
            {
                return pędzelDamkiGracza1.Color;
            }
            set
            {
                pędzelDamkiGracza1 = new SolidColorBrush(value);
                zmieńKoloryWszystkichPrzycisków();
            }
        }

        public Color KolorDamkiGracza2
        {
            get
            {
                return pędzelDamkiGracza2.Color;
            }
            set
            {
                pędzelDamkiGracza2 = new SolidColorBrush(value);
                zmieńKoloryWszystkichPrzycisków();
            }
        }

        #endregion

        #region Zmiana stanu pól
        public void ZaznaczRuch(WspółrzędnePola współrzędnePola, StanPola stanPola)
        {
            planszaStany[współrzędnePola.Poziomo, współrzędnePola.Pionowo] = stanPola;
            planszaPrzyciski[współrzędnePola.Poziomo, współrzędnePola.Pionowo].Background =
                PędzelDlaStanu(stanPola);
        }

        public void ZaznaczPodpowiedź(WspółrzędnePola współrzędnePola, StanPola stanPola)
        {
            if (stanPola == StanPola.Puste)
                throw new Exception("Nie można zaznaczyć podpowiedzi dla stanu pustego pola");
            SolidColorBrush pędzelPodpowiedzi =
                PędzelDlaStanu(stanPola).Lerp(pędzelPustegoPola, 0.5f);
            planszaPrzyciski[współrzędnePola.Poziomo, współrzędnePola.Pionowo].Background =
                pędzelPodpowiedzi;
        }
        #endregion

        public void CzyjePole(WspółrzędnePola współrzędnePol)
        {
            MessageBox.Show(planszaStany[współrzędnePol.Poziomo, współrzędnePol.Pionowo].ToString());
        }

        #region Zdarzenie
        public class PlanszaEventArgs : RoutedEventArgs
        {
            public WspółrzędnePola WspółrzędnePola;
        }

        public delegate void PlanszaEventHandler(object sender, PlanszaEventArgs e);

        public event PlanszaEventHandler KliknięciePola;

        protected virtual void onKliknięciePola(WspółrzędnePola współrzędnePola)
        {
            if (KliknięciePola != null)
                KliknięciePola(this, new PlanszaEventArgs
                    {
                WspółrzędnePola = współrzędnePola
                    });
        }
        #endregion
    }
}
