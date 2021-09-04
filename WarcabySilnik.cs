﻿using System;
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

        


        private int[] liczbyPól = new int[3];

        private void obliczLiczbyPól()
        {
            for (int i = 0; i < liczbyPól.Length; ++i) liczbyPól[i] = 0;

            for (int i = 0; i < SzerokośćPlanszy; ++i)
                for (int j = 0; j < WysokośćPlanszy; ++j)
                    liczbyPól[plansza[i, j]]++;
        }

        public int LiczbaPustychPól { get { return liczbyPól[0]; } }
        public int LiczbaPólGracz1 { get { return liczbyPól[1]; } }
        public int LiczbaPólGracz2 { get { return liczbyPól[2]; } }

        public int pozycjaXPodniesionegoKamienia;
        public int pozycjaYPodniesionegoKamienia;

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
            if (plansza[poziomo, pionowo] != NumerGraczaWykonującegoNastępnyRuch) return -1;

            pozycjaXPodniesionegoKamienia = poziomo;
            pozycjaYPodniesionegoKamienia = pionowo;
            //MessageBox.Show("3");
            return 1;
        }

        protected int PrzenieśKamień(int poziomo, int pionowo, bool tylkoTest)
        {
            //czy współrzędne prawidłowe
           // MessageBox.Show("Dane: ",poziomo.ToString()+ pionowo.ToString());
            if (!czyWspółrzędnePolaPrawidłowe(poziomo, pionowo))
                throw new Exception("nieprawidłowe współrzędne pola");

            //czy pole nie jest zajęte
            if (plansza[poziomo, pionowo] != 0) return -1;
            //MessageBox.Show("wywołano");
            //sprawdzaniePowrawnościRuchu
            bool położenieKamieniaMożliwe = plansza[poziomo, pionowo] == 0;
            położenieKamieniaMożliwe = true;
            int ileKamieniZbitych = -1;
            //połorzenie
            if (położenieKamieniaMożliwe)
            {
                int zmianaX = poziomo - pozycjaXPodniesionegoKamienia;
                int zmianaY = pionowo - pozycjaYPodniesionegoKamienia;
                if (zmianaX == 0 || zmianaY == 0)
                {
                    //MessageBox.Show("0");
                    return -1;
                }
                else if ((zmianaX == 1 || zmianaX == -1) && (zmianaY == 1 || zmianaY == -1))
                {
                    plansza[pozycjaXPodniesionegoKamienia, pozycjaYPodniesionegoKamienia] = 0;
                    plansza[poziomo, pionowo] = NumerGraczaWykonującegoNastępnyRuch;
                    zmieńBierzącegoGracza();
                    obliczLiczbyPól();
                    //MessageBox.Show("1","Przesuniento");
                    return 0;
                }
                else if ((zmianaX == 1 || zmianaX == -1) || (zmianaY == 1 || zmianaY == -1))
                {
                    //MessageBox.Show("0");
                    return -1;
                }
                else if ((zmianaX == 2 || zmianaX == -2) && (zmianaY == 2 || zmianaY == -2))
                {
                    if (plansza[zmianaX / 2 + pozycjaXPodniesionegoKamienia, zmianaY / 2 + pozycjaYPodniesionegoKamienia]
                        == numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch))
                    {
                        plansza[zmianaX / 2 + pozycjaXPodniesionegoKamienia,
                            zmianaY / 2 + pozycjaYPodniesionegoKamienia] = 0;
                        plansza[pozycjaXPodniesionegoKamienia, pozycjaYPodniesionegoKamienia] = 0;
                        plansza[poziomo, pionowo] = NumerGraczaWykonującegoNastępnyRuch;
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
            //koniec if-u

            //zmiana gracza jeśli ruch wykonany
            //if (ileKamieniZbitych > -1 && !tylkoTest )
            //    zmieńBierzącegoGracza();
            obliczLiczbyPól();
            zmieńBierzącegoGracza();
            //MessageBox.Show("x");
            return ileKamieniZbitych;
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