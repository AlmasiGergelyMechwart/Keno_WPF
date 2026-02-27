using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KENO
{
    public class NapiKeno
    {
        int ev;
        int het;
        int nap;
        DateOnly huzasDatum;
        List<int> szamok;

        public int Ev { get => ev; set => ev = value; }
        public int Het { get => het; set => het = value; }
        public int Nap { get => nap; set => nap = value; }
        public DateOnly HuzasDatum { get => huzasDatum; set => huzasDatum = value; }
        public List<int> Szamok { get => szamok; set => szamok = value; }

        public NapiKeno(string csvLine)
        {
            string[] parts = csvLine.Split(';');
            if (parts.Length < 5) throw new ArgumentException("A CSV sor túl rövid: " + csvLine);

            if (!int.TryParse(parts[0], out ev) || ev < 0) throw new ArgumentException("Érvénytelen év: " + parts[0]);
            if (!int.TryParse(parts[1], out het) || het < 1 || het > 53) throw new ArgumentException("Érvénytelen hét: " + parts[1]); // Tudom, a feladatban 52-t írt max limitnek, de ettől függetlenül az adatok között van 53 a heteknél..
            if (!int.TryParse(parts[2], out nap) || nap < 1 || nap > 7) throw new ArgumentException("Érvénytelen nap: " + parts[2]);
            if (!DateOnly.TryParse(parts[3], CultureInfo.InvariantCulture, out huzasDatum)) throw new ArgumentException("Érvénytelen Húzásdátum:" + parts[3]);

            szamok = new List<int>();
            for (int i = 4; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out int szam)) throw new ArgumentException("Érvénytelen húzás: " + parts[i]);
                szamok.Add(szam);
            }
        }

        public int TalalatSzam(List<int> tippek)
        {
            return tippek.Count(tipp => szamok.Contains(tipp));
        }

        public bool Helyes {
            get {
                return szamok.Count == 20 && szamok.ToHashSet<int>().Count == 20;
            }
        }
    }
}
