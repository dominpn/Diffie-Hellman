using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace POD_lab9
{
    class Uczestnik //powinna byc klasa dziedziczaca po klasie Uczestnik
    {
        public int liczba_udostepniona;
        public int p;
        public int g;
        private int liczba_tajna;
        private int klucz_sesyjny;
        private int klucz_sesyjny2; //pole używane tylko przy ataku MITM dla atakującego
        public int get_klucz_sesyjny()
        {
            return klucz_sesyjny;
        }
        public int get_klucz_sesyjny2() //metoda używana tylko przy ataku MITM dla atakującego
        {
            return klucz_sesyjny2;
        }
        public Uczestnik(int p, int g, System.Random los)
        {
            this.p = p;
            this.g = g;
            liczba_tajna = los.Next(5000, 10000);
            liczba_udostepniona = (int)BigInteger.ModPow(g, liczba_tajna, p);
        }
        public void oblicz_klucz_sesyjny(int liczba_II_uczestnik)
        {
            klucz_sesyjny = (int)BigInteger.ModPow(liczba_II_uczestnik, liczba_tajna, p);
        }
        public void oblicz_klucz_sesyjny2(int liczba_II_uczestnik)
        {
            klucz_sesyjny2 = (int)BigInteger.ModPow(liczba_II_uczestnik, liczba_tajna, p);
        }

    }
    class Program
    {
        static bool Czy_pierwsza(int n)
        {
            if (n < 2)
                return false;
            for (int i = 2; i * i <= n; i++)
                if (n % i == 0)
                    return false;
            return true;
        }
        static void Atak_MITM(Uczestnik Alicja, Uczestnik Bob, Uczestnik Ewa)
        {
            // publiczne liczby p i q są przechwycone przez atakującego
            // atakujący - Ewa oblicza klucz sesyjny na podstawie udostępnionej liczby przez Alicję
            Ewa.oblicz_klucz_sesyjny(Alicja.liczba_udostepniona);
            //Alicja oblicza klucz sesyjny za pomocą liczby otrzymanej przez Ewę, która podszywa się pod Boba
            Alicja.oblicz_klucz_sesyjny(Ewa.liczba_udostepniona);
            //Bob otrzymuje liczbę udostępnioną od Ewy, którą uważa za liczbę Alicji
            Bob.oblicz_klucz_sesyjny(Ewa.liczba_udostepniona);
            //Ewa za pomocą liczby udostępnionej przez Boba, oblicza drugi klucz sesyjny
            Ewa.oblicz_klucz_sesyjny2(Bob.liczba_udostepniona);
            //Ewa jest w posiadaniu dwóch kluczy sesyjnych, który używa do komunikacji z Alicją i Bobem podszywając się pod jedną ze stron
            Console.WriteLine("\n\n\n\nKomunikacja z atakiem");
            Console.WriteLine("Wartosc klucz sesyjnego Alicji wynosi: " + Alicja.get_klucz_sesyjny() + " klucz sesyjny Ewy dla Alicji: "+Ewa.get_klucz_sesyjny());
            Console.WriteLine("Wartosc klucz sesyjnego Boba wynosi: " + Bob.get_klucz_sesyjny() + " klucz sesyjny Ewy dla Boba: " + Ewa.get_klucz_sesyjny2());
        }
        static void Main(string[] args)
        {
            System.Random los = new Random(DateTime.Now.Millisecond);
            int p, g;
            do
            {
                p = los.Next(5000, 10000);
                g = los.Next(5000, 10000);
            } while ((Czy_pierwsza(p) == false && Czy_pierwsza(g) == false));
            Uczestnik Alicja = new Uczestnik(p,g,los);
            Uczestnik Bob = new Uczestnik(p, g, los);
            Uczestnik Ewa = new Uczestnik(p, g, los);
            Alicja.oblicz_klucz_sesyjny(Bob.liczba_udostepniona);
            Bob.oblicz_klucz_sesyjny(Alicja.liczba_udostepniona);
            if (Alicja.get_klucz_sesyjny() == Bob.get_klucz_sesyjny())
            {
                Console.WriteLine("Normalna komunikacja");
                Console.WriteLine("Klucz sesyjny jest taki sam\n" + "Wartosc klucz sesyjnego Alicji wynosi: " + Alicja.get_klucz_sesyjny());
                Console.WriteLine("Wartosc klucz sesyjnego Boba wynosi: " + Bob.get_klucz_sesyjny());
            }
            else
                Console.WriteLine("Klucze sesyjne sa rozne");
            Atak_MITM(Alicja, Bob, Ewa);
            Console.ReadKey();
        }
    }
}
