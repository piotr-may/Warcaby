using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warcaby
{
    public enum SytułacjaNaPlanszy
    {
        RuchJestMożliwy,
        BieżącyGraczNieMożeWykonaćRuchu,
        ObajGraczeNieMogąWykonaćRuchu,
        JedenZGraczyNieMaPól
    }

    public interface ISilnikDlaDwóchGraczy
    {
        int SzerokośćPlanszy { get;}
        int WysokośćPlanszy { get;}

        int NumerGraczaWykonującegoNastępnyRuch { get;}
        int NumerGraczaMającegoPrzewagę { get; }
        int PobierzStanPola(int poziomo, int pionowo);
        bool PodnieśKamień(int poziomo, int pionowo);
        bool PrzenieśKamień(int poziomo, int pionowo);
        void wczytajPlansze(int[,] nowaPlansza);
        void wszytajGracza(int NumerNowegoGracza);

        int LiczbaPustychPól { get; }
        int LiczbaPólGracz1 { get; }
        int LiczbaPólGracz2 { get; }

        void Pasuj();

        SytułacjaNaPlanszy ZbadajSytułacjeNaPlanszy();
    }

    public interface ISilnikGryDlaJednegoGracza : ISilnikDlaDwóchGraczy
    {
        void ProponujNajlepszyRuch(out int najlepszyRuchPoziomo,
                                   out int najlepszyRuchPionowo);
    }
}
