using System.Windows.Media;

namespace PiotrMay.WpfUtils
{
    static class MieszanieKolorów
    {
        public static Color Lerp(this Color kolor, Color inntKolor, double waga)
        {
            byte r = (byte)(waga * kolor.R + (1 - waga) * inntKolor.R);
            byte g = (byte)(waga * kolor.G + (1 - waga) * inntKolor.G);
            byte b = (byte)(waga * kolor.B + (1 - waga) * inntKolor.B);
            return Color.FromRgb(r, g, b);
        }

        public static SolidColorBrush Lerp(this SolidColorBrush pędzel,
                                            SolidColorBrush innyPędzel, double waga)
        {
            return new SolidColorBrush(Lerp(pędzel.Color, innyPędzel.Color, waga));
        }
    }
}
