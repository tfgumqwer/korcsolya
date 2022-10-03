using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hellsinki2017
{

    class adat
    {
        public string Név;
        public string Orszag;
        public float Technikai;
        public float Komponens;
        public int Hibabont;
        public adat(string input)
        {
            var temp = input.Split(";");
            Név = temp[0];
            Orszag = temp[1];
            Technikai = float.Parse(temp[2].Replace(".", ","));
            Komponens = float.Parse(temp[3].Replace(".", ","));
            Hibabont = int.Parse(temp[4]);
        }
    }

    class Program
    {
        static List<adat> donto;
        static List<adat> rovidprogram;

        static void Main(string[] args)
        {
            donto = File.ReadAllLines("donto.csv").Skip(1).Select(i => new adat(i)).ToList();
            rovidprogram = File.ReadAllLines("rovidprogram.csv").Skip(1).Select(i => new adat(i)).ToList();
            Console.WriteLine($"Rövidprogramba elindult versenyzők száma: {rovidprogram.Count()}");
            var hun = donto.Where(i => i.Orszag == "HUN");
            Console.WriteLine(hun.Count() == 0 ? "Nem jutott be a magyar" : "Be jutott a magyar");
            Console.Write("Kérem a versenyző nevét: ");
            var input = Console.ReadLine();
            try
            {
                Console.WriteLine("A versenyzű pontszama: " + ÖsszPontszam(input));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Dictionary<string, int> hashmap = new Dictionary<string, int>();
            foreach (var item in donto)
            {
                if (hashmap.ContainsKey(item.Orszag)) hashmap[item.Orszag]++;
                else hashmap.Add(item.Orszag, 1);
            }
            foreach (var item in hashmap.Where(i => i.Value > 1))
            {
                Console.WriteLine($"{item.Key}: {item.Value} versenyző");
            }
            using (var outfile = new StreamWriter("vegeredmeny.csv"))
            {
                var list = new List<(string nev, string orszag, float pont)>();
                foreach (var item in donto)
                {
                    list.Add((item.Név, item.Orszag, ÖsszPontszam(item.Név)));
                }
                var sort = list.OrderBy(i => i.pont).Reverse().ToList();
                for (int i = 0; i < sort.Count(); i++)
                {
                    var j = sort[i];
                    outfile.WriteLine($"{i + 1};{j.nev};{j.orszag};{j.pont}");
                }
            }
        }

        static float ÖsszPontszam(string name)
        {
            var sum = 0f;
            var dontoInst = donto.Find(i => i.Név == name);
            var inst = rovidprogram.Find(i => i.Név == name);
            if (inst is null) throw new Exception("Ilyen nevű indulo nem volt");
            if (dontoInst is not null) sum += dontoInst.Technikai + dontoInst.Komponens - dontoInst.Hibabont;
            sum += inst.Technikai + inst.Komponens - inst.Hibabont;
            return sum;
        }
    }
}