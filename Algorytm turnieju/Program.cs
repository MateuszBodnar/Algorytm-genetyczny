using System;
using System.Collections.Generic;
class Osobnik
{
    public List<byte> chromosom;
    public List<double> wartosc_parametru;
    public int Zd_min;
    public int Zd_max;
    public int dlugosc_chromosomu;
    public int ilosc_parametrow;
    public double ocena;

    public static Random random = new();


    public Osobnik(int Zd_min, int Zd_max, int dlugosc_chromosomu, int ilosc_parametrow)
    {
        this.wartosc_parametru = new List<double>(ilosc_parametrow);
        this.Zd_min = Zd_min;
        this.Zd_max = Zd_max;
        this.dlugosc_chromosomu = dlugosc_chromosomu;
        this.ilosc_parametrow = ilosc_parametrow;
        this.GenerujChromosom();
        this.Dekodowanie();
        this.FunkcjaPrzystosowania();
    }

    public void GenerujChromosom()
    {
        chromosom = new List<byte>();
        for (int i = 0; i < dlugosc_chromosomu * ilosc_parametrow; i++)
        {
            chromosom.Add((byte)random.Next(0, 2));
        }
    }

    public void Kodowanie()
    {
        double ctmp = 0;
        double Zd = Zd_max - Zd_min;

        for (int i = 0; i < ilosc_parametrow; i++)
        {
            wartosc_parametru[i] = Math.Max(Zd_min, Math.Min(Zd_max, wartosc_parametru[i]));

            ctmp = Math.Round(((wartosc_parametru[i] - Zd_min) / Zd) * (Math.Pow(2, dlugosc_chromosomu) - 1));

            for (int b = 0; b < dlugosc_chromosomu; b++)
            {
                 chromosom.Add((byte)(Math.Floor(ctmp / Math.Pow(2, b)) % 2));
            }
        }
    }

    public void Dekodowanie()
    {
        wartosc_parametru.Clear();
        double Zd = Zd_max - Zd_min;

        for (int i = 0; i < ilosc_parametrow; i++)
        {
            double ctmp = 0;
            for (int b = 0; b < dlugosc_chromosomu; b++)
            {
                ctmp += Convert.ToDouble(chromosom[(i * dlugosc_chromosomu + b)]) * Math.Pow(2, dlugosc_chromosomu - 1 - b);
            }
            wartosc_parametru.Add(Zd_min + (ctmp / (Math.Pow(2, dlugosc_chromosomu) - 1)) * Zd);
        }


    }

    public void FunkcjaPrzystosowania()
    {
        ocena = Math.Sin(wartosc_parametru[0] * 0.05) + Math.Sin(wartosc_parametru[1] * 0.05) + 0.4 * Math.Sin(wartosc_parametru[0] * 0.15) * Math.Sin(wartosc_parametru[1] * 0.15);
    }

    public void PokazOsobnika()
    {
        Console.WriteLine($"Osobnik: Chromosom: {string.Join("", chromosom)}, wartosc parametru: {string.Join(" | ", wartosc_parametru)}, ocena: {string.Join(" | ", ocena)}");
    }

}

class Turniej
{
    public List<Osobnik> populacja;
    public List<Osobnik> nowa_populacja;
    public int RozmiarTurnieju;
    public int LiczbaOsobinkow;

    public static Random random = new Random();

    public Turniej(int rozmiarTurnieju, int liczbaOsobnikow)
    {
        this.RozmiarTurnieju = rozmiarTurnieju;
        this.LiczbaOsobinkow = liczbaOsobnikow;
    }
    public static List<Osobnik> GenerujPopulacje(int liczbaOsobnikow, int Zd_min, int Zd_max, int dlugosc_chromosomu, int ilosc_parametrow)
    {
        List<Osobnik> populacja = new List<Osobnik>();

        for (int i = 0; i < liczbaOsobnikow; i++)
        {
            Osobnik osobnik = new Osobnik(Zd_min, Zd_max, dlugosc_chromosomu, ilosc_parametrow);
            populacja.Add(osobnik);
        }

        return populacja;
    }

    public void OperatorSelekcjiTurniejowej()
    {
        List<Osobnik> sklad_turnieju = new List<Osobnik>(RozmiarTurnieju);

        for (int i = 0; i < LiczbaOsobinkow - 1; i++)
        {
    

            for (int j = 0; j < RozmiarTurnieju; j++)
            {
                int index = random.Next(0, LiczbaOsobinkow);
                sklad_turnieju.Add(populacja[index]);
            }

            Osobnik najlepszy_z_turnieju = sklad_turnieju[0];

            foreach (var osobnik in sklad_turnieju)
            {
                if (osobnik.ocena > najlepszy_z_turnieju.ocena)
                {
                    najlepszy_z_turnieju.ocena = osobnik.ocena;
                }
            }

            nowa_populacja.Add(najlepszy_z_turnieju);
            sklad_turnieju.Clear();
        }

        Osobnik najlepszy = populacja[0];

        foreach (var osobnik in populacja)
        {
            if (osobnik.ocena > najlepszy.ocena)
            {
                najlepszy = osobnik;
            }
        }

        nowa_populacja.Add(najlepszy);

        foreach(var osobnik in nowa_populacja)
        {
            int punkt = random.Next(0, osobnik.chromosom.Count);

            osobnik.chromosom[punkt] = (byte)(1 - osobnik.chromosom[punkt]);

            osobnik.Dekodowanie();
            osobnik.FunkcjaPrzystosowania();
        }
        
        populacja.Clear();
        populacja.AddRange(nowa_populacja);
        nowa_populacja.Clear();

    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Osobnik> populacja = Turniej.GenerujPopulacje(9, 0, 100, 3, 2);

        foreach (var osobnik in populacja)
        {
            osobnik.PokazOsobnika();
        }

        Turniej turniej = new Turniej(2, 9);
        turniej.populacja = populacja;
        turniej.nowa_populacja = new List<Osobnik>();

        for (int i = 0; i < 20; i++)
        {
            turniej.OperatorSelekcjiTurniejowej();
        }

        Console.WriteLine();

        foreach (var osobnik in populacja)
        {
            osobnik.PokazOsobnika();
        }
    }
}