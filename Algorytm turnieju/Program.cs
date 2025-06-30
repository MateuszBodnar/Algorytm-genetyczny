using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    public Osobnik(Osobnik inny)
    {
        this.chromosom = new List<byte>(inny.chromosom);
        this.wartosc_parametru = new List<double>(inny.wartosc_parametru);
        this.Zd_min = inny.Zd_min;
        this.Zd_max = inny.Zd_max;
        this.dlugosc_chromosomu = inny.dlugosc_chromosomu;
        this.ilosc_parametrow = inny.ilosc_parametrow;
        this.ocena = inny.ocena;
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
    public List<Osobnik> stara_populacja;
    public List<Osobnik> nowa_populacja;
    public int RozmiarTurnieju;
    public int LiczbaOsobinkow;

    public static Random random = new Random();

    public Turniej(int rozmiarTurnieju, int liczbaOsobnikow)
    {
        this.populacja = new List<Osobnik>();
        this.nowa_populacja = new List<Osobnik>();
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
                    najlepszy_z_turnieju = osobnik;
                }
            }
            nowa_populacja.Add(new Osobnik(najlepszy_z_turnieju));
            sklad_turnieju.Clear();

        }
       
    }
    
    public void HotDeck()
    {
        Osobnik najlepszy = populacja[0];

        foreach (var osobnik in populacja)
        {
            if (osobnik.ocena > najlepszy.ocena)
            {
                najlepszy = osobnik;
            }
        }

        nowa_populacja.Add(najlepszy);
    }

    public void Mutacja()
    {
        foreach (var osobnik in nowa_populacja)
        {
            int punkt = random.Next(0, osobnik.chromosom.Count);

            osobnik.chromosom[punkt] = (byte)(1 - osobnik.chromosom[punkt]);

            osobnik.Dekodowanie();
            osobnik.FunkcjaPrzystosowania();
        }
    }

    public void MaxSredniaWartosc()
    {
        Osobnik najwiekszaWartosc = nowa_populacja[0];
        double srednia = 0;

        foreach (var osobnik in nowa_populacja)
        {
            if (osobnik.ocena > najwiekszaWartosc.ocena)
            {
                najwiekszaWartosc = osobnik;
            }
        }

        foreach (var osobnik in nowa_populacja)
        {
            srednia += osobnik.ocena;
        }



        Console.WriteLine($" Największa wartość: {string.Join(" ", Math.Round(najwiekszaWartosc.ocena,2))}, Średnia wartość: {string.Join(" | ",  Math.Round(srednia/nowa_populacja.Count,2))}");
    }

    public void OperatorKrzyżowania()
    {
        for (int i = 0; i < nowa_populacja.Count - 1; i += 2)
        {
            Osobnik rodzic1 = nowa_populacja[i];
            Osobnik rodzic2 = nowa_populacja[i + 1];

            // Liczba parametrów na podstawie jednego rodzica
            int liczbaParametrow = rodzic1.ilosc_parametrow;

            // Długość całego chromosomu
            int punkt_ciecia = random.Next(1, rodzic1.chromosom.Count - 1);

            // Tworzymy dzieci
            List<byte> chromosom_dziecka1 = new List<byte>();
            List<byte> chromosom_dziecka2 = new List<byte>();

            // Jedno dziecko to początek rodzica1 + koniec rodzica2
            chromosom_dziecka1.AddRange(rodzic1.chromosom.GetRange(0, punkt_ciecia));
            chromosom_dziecka1.AddRange(rodzic2.chromosom.GetRange(punkt_ciecia, rodzic2.chromosom.Count - punkt_ciecia));

            // Drugie dziecko to początek rodzica2 + koniec rodzica1
            chromosom_dziecka2.AddRange(rodzic2.chromosom.GetRange(0, punkt_ciecia));
            chromosom_dziecka2.AddRange(rodzic1.chromosom.GetRange(punkt_ciecia, rodzic1.chromosom.Count - punkt_ciecia));

            // Tworzymy osobniki potomne
            Osobnik dziecko1 = new Osobnik(rodzic1);
            dziecko1.chromosom = chromosom_dziecka1;
            dziecko1.Dekodowanie();
            dziecko1.FunkcjaPrzystosowania();

            Osobnik dziecko2 = new Osobnik(rodzic2);
            dziecko2.chromosom = chromosom_dziecka2;
            dziecko2.Dekodowanie();
            dziecko2.FunkcjaPrzystosowania();

            // Dodajemy dzieci do nowej populacji
            nowa_populacja.Add(dziecko1);
            nowa_populacja.Add(dziecko2);
        }
    }
}



class Program
{
    static void Main(string[] args)
    {
        List<Osobnik> populacja = Turniej.GenerujPopulacje(9, 0, 100, 3, 2);

        Console.WriteLine("PIerwsza Popaulacja.");

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
            turniej.Mutacja();
            turniej.HotDeck();
            turniej.MaxSredniaWartosc();
            turniej.populacja.Clear();
            populacja.AddRange(turniej.nowa_populacja);
            turniej.nowa_populacja.Clear();
        }

        Console.WriteLine();

        foreach (var osobnik in populacja)
        {
            osobnik.PokazOsobnika();
        }
    }
}