using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Warcaby
{
    public class WarcabySilnik : ISilnikDlaDwóchGraczy
    {
        public int SzerokośćPlanszy { get; private set; }
        public int WysokośćPlanszy { get; private set; }
        

        private int[,] plansza;
        public int NumerGraczaWykonującegoNastępnyRuch { get; private set; } = 1;

        


        private int[] liczbyPól = new int[5];

        private void obliczLiczbyPól()
        {
            for (int i = 0; i < liczbyPól.Length; ++i) liczbyPól[i] = 0;

            for (int i = 0; i < SzerokośćPlanszy; ++i)
                for (int j = 0; j < WysokośćPlanszy; ++j)
                    liczbyPól[plansza[i, j]]++;
        }

        public int LiczbaPustychPól { get { return liczbyPól[0]; } }
        public int LiczbaPólGracz1 { get { return liczbyPól[1] + liczbyPól[3]; } }
        public int LiczbaPólGracz2 { get { return liczbyPól[2] + liczbyPól[4]; } }
        public int LiczbaDamekGracz1 { get { return liczbyPól[3]; } }
        public int LiczbaDamekGracz2 { get { return liczbyPól[4]; } }

        public int pozycjaXPodniesionegoKamienia;
        public int pozycjaYPodniesionegoKamienia;

        public bool czyDamka = false;

        public int pozycjaXZbitegoKamienia;
        public int pozycjaYZbitegoKamienia;

        public int nowaPozycjaXKamienia;
        public int nowaPozycjaYKamienia;

        private static int numerPrzeciwnika(int numerGracza)
        {
            return (numerGracza == 1) ? 2 : 1;
        }

        private bool czyWspółrzędnePolaPrawidłowe(int poziomo, int pionowo)
        {
            return poziomo >= 0 && poziomo < 8 &&
                   pionowo >= 0 && pionowo < 8;
        }

        public int PobierzStanPola(int poziomo, int pionowo)
        {
            if (!czyWspółrzędnePolaPrawidłowe(poziomo, pionowo))
                throw new Exception("Nieprawidłowe współrzędne pola");
            return plansza[poziomo, pionowo];
        }



        private void czyśćPlansze()
        {
            for (int i = 0; i < SzerokośćPlanszy; i++)
                for (int j = 0; j < WysokośćPlanszy; j++)
                    if (j <= 2 && ((j % 2 == 0 && i % 2 == 0) || (j % 2 != 0 && i % 2 != 0)))
                        plansza[i, j] = 2;
                    else if (j >= 5 && ((j %2 == 0 && i % 2 == 0)  || (j % 2 != 0 && i % 2 != 0)))
                        plansza[i, j] = 1;
                    else
                        plansza[i, j] = 0;
        }

        public WarcabySilnik(int numerGraczaRozpoczynającego,
                             int szerokośćPlanszy = 8, int wysokośćPlanszy = 8)
        {
            if (numerGraczaRozpoczynającego < 1 || numerGraczaRozpoczynającego > 2)
                throw new Exception("Nieprawidłowy numer gracza rozpoczynającego grę");

            SzerokośćPlanszy = szerokośćPlanszy;
            WysokośćPlanszy = wysokośćPlanszy;
            plansza = new int[SzerokośćPlanszy, WysokośćPlanszy];

            czyśćPlansze();

            NumerGraczaWykonującegoNastępnyRuch = numerGraczaRozpoczynającego;
            obliczLiczbyPól();
        }

        public void wczytajPlansze(int[,] nowaPlansza)
        {
            plansza = nowaPlansza;
            obliczLiczbyPól();
        }

        public void wszytajGracza(int NumerNowegoGracza)
        {
            NumerGraczaWykonującegoNastępnyRuch = NumerNowegoGracza;
        }

        private void zmieńBierzącegoGracza()
        {
            NumerGraczaWykonującegoNastępnyRuch =
                numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch);
        }

        protected int PodnieśKamień(int poziomo, int pionowo, bool tylkoTest)
        {
            //czy współrzędne prawidłowe
            if (!czyWspółrzędnePolaPrawidłowe(poziomo, pionowo))
                throw new Exception("nieprawidłowe współrzędne pola");

            //czy pole należy do gracza
            if (plansza[poziomo, pionowo] != NumerGraczaWykonującegoNastępnyRuch &&
                plansza[poziomo, pionowo] != NumerGraczaWykonującegoNastępnyRuch+2) return -1;
            if (plansza[poziomo, pionowo] ==
                NumerGraczaWykonującegoNastępnyRuch + 2)
            {
                czyDamka = true;
                //MessageBox.Show("wybrano damke");
            }
            else czyDamka = false;

            pozycjaXPodniesionegoKamienia = poziomo;
            pozycjaYPodniesionegoKamienia = pionowo;
            //MessageBox.Show("Posniesiono");
            return 1;
        }

        protected int PrzenieśKamień(int poziomo, int pionowo, bool tylkoTest)
        {
            //czy współrzędne prawidłowe
            if (!czyWspółrzędnePolaPrawidłowe(poziomo, pionowo))
                throw new Exception("nieprawidłowe współrzędne pola");

            //czy pole nie jest zajęte
            if (plansza[poziomo, pionowo] != 0) return -1;
            //MessageBox.Show(poziomo.ToString() + pionowo.ToString());
            //sprawdzaniePowrawnościRuchu
            bool położenieKamieniaMożliwe = plansza[poziomo, pionowo] == 0;
            
            int ileKamieniZbitych = -1;

            //połorzenie
            if (położenieKamieniaMożliwe)
            {
                int zmianaX = poziomo - pozycjaXPodniesionegoKamienia;
                int zmianaY = pionowo - pozycjaYPodniesionegoKamienia;
                if (czyDamka)
                {
                    
                    if (zmianaX == 0 || zmianaY == 0)
                    {
                        //MessageBox.Show("0");
                        return -1;
                    }
                    else if  (zmianaX == zmianaY || zmianaX * -1 == zmianaY || zmianaX == zmianaY * -1)
                    {
                        if (zmianaX > 0 && zmianaY > 0)
                        {
                            int j = pozycjaYPodniesionegoKamienia + 1;
                            for (int i = pozycjaXPodniesionegoKamienia + 1; i < poziomo; i++)
                            {
                                    if ((plansza[i, j] ==
                                        numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch)
                                        || plansza[i, j] == numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch) + 2) &&
                                        plansza[i + 1, j + 1] == 0)
                                    {
                                        //MessageBox.Show("Znaleziono przeciwnika 1");
                                        plansza[i, j] = 0;
                                    }
                                    else if (plansza[i, j] != 0)
                                        return -1;
                                j++;
                            }
                        }
                        else if (zmianaX > 0 && zmianaY < 0)
                        {
                            int j = pozycjaYPodniesionegoKamienia - 1;
                            for (int i = pozycjaXPodniesionegoKamienia + 1; i < poziomo; i++)
                            {
                                

                                if ((plansza[i, j] ==
                                    numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch)
                                    || plansza[i, j] == numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch) + 2) &&
                                    plansza[i + 1, j - 1] == 0)
                                {
                                    //MessageBox.Show("Znaleziono przeciwnika 2");
                                    plansza[i, j] = 0;
                                }
                                else if (plansza[i, j] != 0)
                                {
                                    return -1;
                                }
                                j--;
                            }
                        }
                        else if (zmianaX <0 && zmianaY > 0)
                        {
                            int j = pozycjaYPodniesionegoKamienia + 1;
                            for (int i = pozycjaXPodniesionegoKamienia-1; i > poziomo; i--)
                            {
                                    if ((plansza[i, j] ==
                                        numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch)
                                        || plansza[i, j] == numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch) + 2) &&
                                        plansza[i - 1, j + 1] == 0)
                                    {
                                        //MessageBox.Show("Znaleziono przeciwnika 3");
                                        plansza[i, j] = 0;
                                    }
                                    else if (plansza[i, j] !=0)
                                        return -1;
                                j++;
                             }
                        }
                        else if (zmianaX < 0 && zmianaY < 0)
                        {
                            int j = pozycjaYPodniesionegoKamienia - 1;
                            for (int i = pozycjaXPodniesionegoKamienia-1; i > poziomo; i--)
                            {
                                    if ((plansza[i, j] ==
                                        numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch)
                                        || plansza[i, j] == numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch) + 2) &&
                                        plansza[i - 1, j - 1] == 0)
                                    {
                                        //MessageBox.Show("Znaleziono przeciwnika 4");
                                        plansza[i, j] = 0;
                                    }
                                    else if (plansza[i, j] !=0)
                                        return -1;
                                j--;
                            }
                        }
                        plansza[pozycjaXPodniesionegoKamienia, pozycjaYPodniesionegoKamienia] = 0;
                        plansza[poziomo, pionowo] = NumerGraczaWykonującegoNastępnyRuch+2;
                        zmieńBierzącegoGracza();
                        obliczLiczbyPól();
                        
                        //MessageBox.Show("1","Przesuniento");
                        return 1;
                    }
                    else if ((zmianaX == 1 || zmianaX == -1) || (zmianaY == 1 || zmianaY == -1))
                    {
                        //MessageBox.Show("0");
                        return -1;
                    }
                    else
                    {
                        //MessageBox.Show("2");
                        return -1;
                    }
                }
                else if(!czyDamka )
                {
                    
                    if (zmianaX == 0 || zmianaY == 0)
                    {
                        //MessageBox.Show("0");
                        return -1;
                    }
                    else if (((NumerGraczaWykonującegoNastępnyRuch==1&& zmianaY == -1)||
                        (NumerGraczaWykonującegoNastępnyRuch == 2 && zmianaY == 1))
                        && (zmianaX == 1 || zmianaX == -1))  
                    {
                        plansza[pozycjaXPodniesionegoKamienia, pozycjaYPodniesionegoKamienia] = 0;
                        if (CzyWPozycjiDoDamki(pionowo)) plansza[poziomo, pionowo] = NumerGraczaWykonującegoNastępnyRuch + 2;
                        else plansza[poziomo, pionowo] = NumerGraczaWykonującegoNastępnyRuch;
                        zmieńBierzącegoGracza();
                        obliczLiczbyPól();
                        //MessageBox.Show("1","Przesuniento");
                        return 0;
                    }            
                    else if (( zmianaY == -2|| zmianaY == 2)
                             && (zmianaX == 2 || zmianaX == -2))
                    {
                        int poleDoZbicia = plansza[zmianaX / 2 + pozycjaXPodniesionegoKamienia, zmianaY / 2 + pozycjaYPodniesionegoKamienia];
                        if (poleDoZbicia == numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch) ||
                            poleDoZbicia == numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch)+2)
                        {
                            plansza[zmianaX / 2 + pozycjaXPodniesionegoKamienia,
                                zmianaY / 2 + pozycjaYPodniesionegoKamienia] = 0;
                            plansza[pozycjaXPodniesionegoKamienia, pozycjaYPodniesionegoKamienia] = 0;
                            if (CzyWPozycjiDoDamki(pionowo)) plansza[poziomo, pionowo] = NumerGraczaWykonującegoNastępnyRuch + 2;
                            else plansza[poziomo, pionowo] = NumerGraczaWykonującegoNastępnyRuch;
                            zmieńBierzącegoGracza();
                            obliczLiczbyPól();
                            return 1;
                        }
                    }
                    else
                    {
                        //MessageBox.Show("2");
                        return -1;
                    }

                    
                }
            }   
            //koniec if-u

            //zmiana gracza jeśli ruch wykonany
            //if (ileKamieniZbitych > -1 && !tylkoTest )
            //    zmieńBierzącegoGracza();
            obliczLiczbyPól();
            zmieńBierzącegoGracza();
            //MessageBox.Show("x");
            return ileKamieniZbitych;
        }

        private bool CzyWPozycjiDoDamki(int pionowo)
        {
            if (NumerGraczaWykonującegoNastępnyRuch == 1 && pionowo==0)
            {
                //MessageBox.Show("Nowa damka");
                return true;
            }
            else if (NumerGraczaWykonującegoNastępnyRuch == 2 && pionowo == 7)
            {
                //MessageBox.Show("Nowa damka");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool PrzenieśKamień(int poziomo, int pionowo)
        {
            return PrzenieśKamień(poziomo, pionowo, false) > -1;
        }

        public bool PodnieśKamień(int poziomo, int pionowo)
        {
            return PodnieśKamień(poziomo, pionowo, false) > -1;
        }

        private bool czyBierzącyGraczMożeWykonaćRuch()
        {
            return true;
            int liczbaPoprawnychPól = 0;
            for (int k =0;k< LiczbaPólGracz1; k++)
                for (int i = 0; i < SzerokośćPlanszy; ++i)
                    for (int j = 0; j < WysokośćPlanszy; ++j)
                        if (plansza[i, j] == NumerGraczaWykonującegoNastępnyRuch)
                        {
                            for(int l=-1;l<2;l++)
                                for (int h = -1; h < 2; h++)
                                    if (!(l==0||h==0))
                                        if( PrzenieśKamień(i + l, j + h, true) > 0)
                                            liczbaPoprawnychPól++;
                        }

            return liczbaPoprawnychPól > 0;
        }

        public void Pasuj()
        {
            if (czyBierzącyGraczMożeWykonaćRuch())
                throw new Exception("Gracz nie może oddać ruchu, jeżeli wykonanie ruchu jest możliwe");

            zmieńBierzącegoGracza();
        }

        public SytułacjaNaPlanszy ZbadajSytułacjeNaPlanszy()
        {
            if (LiczbaPólGracz1 == 0 || LiczbaPólGracz2 == 0) return SytułacjaNaPlanszy.JedenZGraczyNieMaPól;

            //Badanie możliwości ruchu bierzącego gracza
            bool czyMożliwyRuch = czyBierzącyGraczMożeWykonaćRuch();
            if (czyMożliwyRuch) return SytułacjaNaPlanszy.RuchJestMożliwy;
            else
            {
                //badanie możliwości ruchu przeciwnika
                zmieńBierzącegoGracza();
                bool czyMożliwyRuchOponenta = czyBierzącyGraczMożeWykonaćRuch();
                zmieńBierzącegoGracza();
                if (czyMożliwyRuchOponenta)
                    return SytułacjaNaPlanszy.BieżącyGraczNieMożeWykonaćRuchu;
                else return SytułacjaNaPlanszy.ObajGraczeNieMogąWykonaćRuchu;
            }
        }

        public int NumerGraczaMającegoPrzewagę
        {
            get
            {
                if (LiczbaPólGracz1 == LiczbaPólGracz2) return 0;
                else return (LiczbaPólGracz1 > LiczbaPólGracz2) ? 1 : 2;
            }
        }

    }
}