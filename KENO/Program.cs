using System.Globalization;

namespace KENO
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<NapiKeno> huzasok = new List<NapiKeno>();
            foreach (string csvLine in File.ReadAllLines("data/Huzasok.csv").Skip(1))
            {
                huzasok.Add(new NapiKeno(csvLine));
            }
            Console.WriteLine("6. feladat: Állomány beolvasása sikeres!");

            //huzasok.GroupBy(huzas => huzas.Szamok.Count).ToList().ForEach(group => Console.WriteLine($"{group.Key}: {group.Count()}"));
            Console.WriteLine("7. fealadat: Hibás sorok száma: " + huzasok.RemoveAll(x => !x.Helyes));

            Console.WriteLine("9. feladat: Nyeremény számítása");

            List<int> inputTippek = new List<int>();
            while (true)
            {
                Console.Write("Kérem a tippjét! Vesszővel elválasztva sorolja fel a számokat:");

                string? input = Console.ReadLine();
                if (input == null) continue;

                inputTippek.Clear();
                string[] parts = input.Split(',');
                foreach (string part in parts)
                {
                    if (int.TryParse(part, out int tipp))
                    {
                        inputTippek.Add(tipp);
                    } else
                    {
                        break;
                    }
                }

                if (inputTippek.Count >= 1 && inputTippek.Count <= 10) break;

                Console.WriteLine("A játéktípus 1..10 lehet!");
            }

            int fogadasiOsszeg;
            while (true)
            {
                Console.Write("Kérem a fogadási összeget! :");

                string? input = Console.ReadLine();
                if (int.TryParse(input, out fogadasiOsszeg) && fogadasiOsszeg > 0 && fogadasiOsszeg < 1000 && fogadasiOsszeg % 200 == 0) break;

                Console.WriteLine("Hibás összeg!");
            }

            int nyeremeny = fogadasiOsszeg * Szorzo(huzasok[0], inputTippek);
            if (nyeremeny > 0)
            {
                Console.WriteLine("Nyereménye:" + nyeremeny);
            } else
            {
                Console.WriteLine("Sajnos nem nyert!");
            }


            Console.WriteLine("10. feladat");
            Console.WriteLine("8-as játék 2020-ban, tét:4X [17,28,32,44,54,63,72,75]");
            int osszVeszteseg = 0;
            int osszNyereseg = 0;
            List<int> tippek2020 = [17, 28, 32, 44, 54, 63, 72, 75];
            foreach (NapiKeno jatek in huzasok.Where(x => x.Ev == 2020))
            {
                int nyereseg = 800 * Szorzo(jatek, tippek2020);

                osszVeszteseg += 800;
                osszNyereseg += nyereseg;

                if (nyereseg > 0)
                    Console.WriteLine($"{jatek.HuzasDatum:yyyy.MM.dd} - {nyereseg}");
            }
            Console.WriteLine($"Összesen {osszVeszteseg} Ft-ot költött Kenóra");
            Console.WriteLine($"Összesen {osszNyereseg} Ft-ot nyert");
        }

        static int Szorzo(NapiKeno keno, List<int> tippek)
        {
            Dictionary<String, int> nyeroParok = new Dictionary<string, int>(){
                    {"10-10",1000000}, {"10-9",8000}, {"10-8",350}, {"10-7",30}, {"10-6",3}, {"10-5",1}, {"10-0",2},
                    {"9-9",100000}, {"9-8",1200}, {"9-7",100}, {"9-6",12}, {"9-5",3}, {"9-0",1},
                    {"8-8",20000}, {"8-7",350}, {"8-6",25}, {"8-5",5}, {"8-0",1},
                    {"7-7",5000}, {"7-6",60}, {"7-5",6}, {"7-4",1}, {"7-0",1},
                    {"6-6",500}, {"6-5",20}, {"6-4",3}, {"6-0",1},
                    {"5-5",200}, {"5-4",10}, {"5-3",2},
                    {"4-4",100}, {"4-3",2},
                    {"3-3",15}, {"3-2",1},
                    {"2-2",6},
                    {"1-1",2}
                };
            int jatekTipus = tippek.Count;
            int talalatokSzama = keno.TalalatSzam(tippek);
            string kulcs = jatekTipus + "-" + talalatokSzama;

            if (nyeroParok.Keys.Contains(kulcs))
                return nyeroParok[kulcs];
            else
                return 0;
        }
    }
}
