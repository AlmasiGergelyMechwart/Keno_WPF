using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KenoGUI
{
    public class Szelveny
    {
        int tipus;
        int szorzo;
        List<int> tippek;

        public int Tipus { get => tipus; set => tipus = value; }
        public int Szorzo { get => szorzo; set => szorzo = value; }
        public List<int> Tippek { get => tippek; set => tippek = value; }

        public Szelveny(string line)
        {
            Match match = Regex.Match(line, @"^KN(?<tipus>[0-9]|10),(?<szorzo>[0-5])!(?<tippek>((?:[1-9]|[1-7]\d|80),?){1,10}(?<!,)$)");

            if (!match.Success ||
                !int.TryParse(match.Groups["tipus"].Value, out tipus) ||
                !int.TryParse(match.Groups["szorzo"].Value, out szorzo)
            )
                throw new ArgumentException("A sor nem jó formátumú");

            string[] parts = match.Groups["tippek"].Value.Split(',');
            if (tipus != parts.Length) throw new ArgumentException($"A tippek száma nem megfelelő");

            tippek = new List<int>();
            foreach (string part in parts)
            {
                if (!int.TryParse(part, out int tipp)) throw new ArgumentException("A sor nem jó formátumú");
                tippek.Add(tipp);
            }
        }

        public override string ToString()
        {
            return $"KN{Tipus},{Szorzo}!{string.Join(',', Tippek)}";
        }
    }
}
