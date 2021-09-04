using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace Warcaby
{
    public static class Ustawienia
    {
        public static double CzytajTop()
        {
            Properties.Settings ustawienia = Properties.Settings.Default;
            return ustawienia.Top;
        }

        public static double CzytajLeft()
        {
            Properties.Settings ustawienia = Properties.Settings.Default;
            return ustawienia.Left;
        }

        public static void ZapiszPozycje(double top, double left)
        {
            Properties.Settings ustawienia = Properties.Settings.Default;
            ustawienia.Top = top;
            ustawienia.Left = left;
            ustawienia.Save();
        }

        public static void SaveGameToNewFile(int[,] plansza,int NumerNastępnegoGracza, 
                                                string[] listaRB, string[] listaRC)
        {
            XDocument xml = new XDocument();
            XDeclaration declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement parameters = new XElement("parameters");
            XElement planszaGry = new XElement("plansza");
            XElement gracz = new XElement("gracz");
            XElement NastępnyGracz = new XElement("następny", NumerNastępnegoGracza);
            XElement ruchyBiały =new XElement("ruchyBiały");
            XElement ruchyCzarny = new XElement("ruchyCzarny");

            //zapisywanie planszy
            for (int i = 0; i < 8; i++)
            {
                XElement rząd = new XElement("rząd" + i.ToString());
                for (int j = 0; j <8; j++)
                {
                    XElement komórka = new XElement("komórka" + i.ToString() + j.ToString(), plansza[i,j]);
                    rząd.Add(komórka);
                }
                planszaGry.Add(rząd);
            }

            //zapisywanie list ruchów
            for(int i=0; i<listaRB.Length;i++)
            {
                XElement ruch = new XElement("ruchB" +i.ToString(),listaRB[i]);
                ruchyBiały.Add(ruch);
            }

            for (int i = 0; i < listaRC.Length; i++)
            {
                XElement ruch = new XElement("ruchC" + i.ToString(), listaRC[i]);
                ruchyCzarny.Add(ruch);
            }


            parameters.Add(planszaGry);
            gracz.Add(NastępnyGracz);
            parameters.Add(gracz);
            parameters.Add(ruchyBiały);
            parameters.Add(ruchyCzarny);
            xml.Declaration = declaration;
            xml.Add(parameters);

            xml.Save("Ustawienia.xml");
        }

        public static void SaveGameToLastFile()
        {

        }

        public static int[,] LoadGameFromFile(string filePath)
        {
            int[,] plansza = new int[8, 8];
            try
            {
                XDocument xml = XDocument.Load(filePath);               
                XElement planszaGry = xml.Root.Element("plansza");
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        plansza[i,j] = int.Parse(planszaGry.Element("rząd" + i.ToString()).Element("komórka"
                            + i.ToString() + j.ToString()).Value);
                    }
            }
            catch
            {
               throw new Exception("Błąd podczas wszytywania pliku");
            }
            return plansza;
        }

        public static int LoadPlayerFormFile(string filePath)
        {
            int NastępnyGracz = 1;
            try
            {
                XDocument xml = XDocument.Load(filePath);
                XElement Gracz = xml.Root.Element("gracz");
                NastępnyGracz = int.Parse(Gracz.Element("następny").Value);
                
            }
            catch
            {
                throw new Exception("Błąd podczas wszytywania pliku2");
            }
            return NastępnyGracz;
        }

        public static string[] LoadListWhite(string filePath)
        {
            string[] listaRuchówBiałego = new string[60];
            int i = 0;
            try
            {
                XDocument xml = XDocument.Load(filePath);
                XElement ruchyB = xml.Root.Element("ruchyBiały");
                for (i = 0; i < listaRuchówBiałego.Length; i++)
                    listaRuchówBiałego[i] = ruchyB.Element("ruchB" + i.ToString()).Value;

            }
            catch
            {
                throw new Exception("Błąd podczas wszytywania pliku" + i.ToString());
            }
            return listaRuchówBiałego;
        }

        public static string[] LoadListBlack(string filePath)
        {
            string[] listaRuchówCzarnego = new string[60];
            try
            {
                XDocument xml = XDocument.Load(filePath);
                XElement ruchyC = xml.Root.Element("ruchyCzarny");
                for (int i = 0; i < listaRuchówCzarnego.Length; i++)
                    listaRuchówCzarnego[i] = ruchyC.Element("ruchC" + i.ToString()).Value;

            }
            catch
            {
                throw new Exception("Błąd podczas wszytywania pliku2");
            }
            return listaRuchówCzarnego;
        }
    }
}
