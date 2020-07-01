using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

/*
 Huijaava arvauspeli

 Ohjelma esittää olevansa peli, jonka tavoitteena on arvata oikea luku ohjelman antamien vihjeiden mukaan.
 Todellisuudessa asiat ovat kuitenkin toisin...

 */

namespace Arvauspeli
{
    class Program
    {
        private static Random randomizer = new Random();    // Alustetaan ensimmäiseksi satunnaislukugeneraattori, jota ohjelman kaikki metodit
                                                            // voivat käyttää
        private static void Main(string[] args)
        {

            // Intrografiikat ja pelaajan nimen varmistus

            DrawColorText("Arvauspeli 1.0\n");
            DrawColorText("--------------\n\n");
            Console.WriteLine("Syötä nimesi:");
            string name = Console.ReadLine();
            Console.Write("Hei ");
            DrawColorText(name);
            Console.Write(" ja tervetuloa pelaamaan arvauspeliä!\n");

            // Päävalikko

            string input = "";
            while (input != "4")
            {
                Console.WriteLine("Tee valintasi syöttämällä numero:");
                DrawColorText("1) Uusi peli\n");
                Console.WriteLine("2) Säännöt");
                Console.WriteLine("3) Pistetilasto");
                Console.WriteLine("4) Lopeta");
                Console.CursorVisible = false;
                Console.Write("_");
                input = Console.ReadKey().KeyChar.ToString();
                Console.Clear();
                switch (input)
                {
                    case "1":
                        StartGame();
                        break;
                    case "2":
                        ShowRules();
                        break;
                    case "3":
                        ShowScore();
                        break;
                    default:
                        Console.WriteLine("En tunnistanut komentoa, yritä uudestaan");
                        break;
                        // Lopettamiselle ei tarvita omaa keissiä koska valikkosilmukka katkeaa kun se on valittu
                }
            }

            Console.Clear();
            Console.WriteLine($"Kiitos pelaamisesta ja nähdään uudestaan, {name}!");
            Console.CursorVisible = true;
            Console.ReadLine();

        }

        private static void ShowScore()
        {
            Console.WriteLine(
                "PARHAAT PISTEET\n" +
                "---------------\n" +
                "Teppo        10\n" +
                "Teppo         4\n" +
                "Teppo         3\n" +
                "\n" +
                "Paina ENTER palataksesi valikkoon.");
            Console.ReadKey();
            Console.Clear();
        }

        private static void ShowRules()
        {
            Console.WriteLine(
                "PELIN SÄÄNNÖT\n" +
                "-------------\n" +
                "1. Pelin alussa arvotaan luku pelaajan määrittämien ehtojen mukaisesti.\n" +
                "2. Omalla vuorollaan pelaaja arvaa mistä numerosta on kyse.\n" +
                "3. Jos pelaaja arvaa väärin, hän saa vihjeen siitä onko oikea luku suurempi vai pienempi.\n" +
                "4. Oikein arvattuaan pelaaja saa pisteitä riippuen siitä, kuinka monta arvausta jäi käyttämättä.\n" +
                "5. Onnea matkaan! :)\n" +
                "\n" +
                "Paina ENTER palataksesi valikkoon.");
            Console.ReadKey();
            Console.Clear();

        }

        private static void StartGame()
        {
            int maxGuess = -1;
            int minGuess = 0;

            List<int> guessedNumbers = new List<int>();

            int randomizerAnimationLength = 3000;

            Console.CursorVisible = true;
            string guessInput;

            while (maxGuess < 1) // Valintasilmukka pyörii niin pitkään, kunnes pelaaja on syöttänyt kelvollisen luvun
            {
                Console.WriteLine("Syötä korkein arvottava luku:");
                guessInput = Console.ReadLine();
                try
                {
                    maxGuess = int.Parse(guessInput);
                    if (maxGuess < 1) Console.WriteLine("Syötä positiivinen kokonaisluku!");
                }
                catch
                {
                    Console.WriteLine("Ei kelvannut! Yritä uudestaan.");
                }
            }

            Console.Write(
                "\n" +
                "Arvotaan luku ... ");

            AnimateRandomizer(maxGuess + 1, randomizerAnimationLength); // Pieni animaatio, jonka tarkoituksena hämätä pelaajaa
            Console.WriteLine("Valmis!               ");
            Console.WriteLine("ÉNTER aloittaa pelin.");
            Console.ReadLine();

            // Seuraavaksi matematiikkaa:
            // Jos tietokone pelaisi reilusti, luvun arvaamiseen tarvittavien arvauksien määrän voi laskea 
            // kaavalla log2 n, jossa n on lukujoukon koko.
            // Lisätietoa: https://fi.wikipedia.org/wiki/Puolitushaku

            double guessesLeft = Math.Floor(Math.Log(maxGuess, 2)); // Floor valittu jotta pelaaja ei saa vahingossakaan riittävän montaa arvausta
            if (guessesLeft < 1) guessesLeft = 1;

            Console.Clear();
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            if (guessesLeft > 1) Console.WriteLine($"Saat {guessesLeft} arvausta.");
            else DrawColorText("Saat vain yhden arvauksen!\n");

            while (guessesLeft > 0)
            {
                DrawGuessGUI(guessedNumbers, guessesLeft);
                string guess = Console.ReadLine();
                Console.Clear();
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                try
                {
                    int guessedNumber = int.Parse(guess);
                    if (!guessedNumbers.Contains(guessedNumber))
                    {
                        guessedNumbers.Add(guessedNumber);

                        // Koska luku arvotaan vasta pelin loputtua, ohjelman tavoitteena on antaa mahdollisimman huonoja vihjeitä.
                        // Jos pelaaja antaa mahdollisten lukujen keskiarvoa pienemmän luvun, ohjelma väittää, että luku on suurempi.
                        // Ja päinvastoin.

                        int leftSize, rightSize;
                        leftSize = guessedNumber - minGuess;
                        rightSize = maxGuess - guessedNumber;

                        if (leftSize > rightSize)
                        {
                            Console.Write("Väärin! Oikea numero on ");
                            DrawColorText("pienempi!\n");
                            maxGuess = guessedNumber - 1;
                        }
                        else
                        {
                            Console.Write("Väärin! Oikea numero on ");
                            DrawColorText("suurempi!\n");
                            minGuess = guessedNumber + 1;
                        }

                        guessesLeft--;

                    }
                    else Console.WriteLine("Syötä luku jota et ole vielä arvannut!");
                }
                catch
                {
                    Console.WriteLine("Syötä kelvollinen luku!");
                }
            }

            DrawColorText("HÄVISIT PELIN.\n");
            Console.Write("Oikea vastaus oli ");

            while (true) //FIXME: Loputtoman luupin riski
            {
                int fakeNumber = randomizer.Next(minGuess, maxGuess + 1);
                if (!guessedNumbers.Contains(fakeNumber))
                {
                    DrawColorText(fakeNumber.ToString() + ".");
                    break;
                }
            }

            Console.WriteLine(
                "\n" +
                "Ei pisteitä tällä kertaa :( \n" +
                "\n" +
                "ENTER palaa valikkoon.\n");
            Console.ReadLine();
            Console.Clear();
        }


        private static void DrawGuessGUI(List<int> guessedNumbers, double guessesLeft)
        {
            /*  Käyttöliittymän mallaus
             * 
             *  1. Ylin rivi ns. statusviesteille
             *  2. Oikealle piirretään taulukko, jossa listataan tehdyt arvaukset
             * 
             *  
             * 
             *  Väärin! Oikea luku on suurempi!                
             *  Syötä seuraava arvaus:                         Arvaukset
             *  _                                              1
             *                                                 ?
             *                                                 ?
             *                                                 ?
             * 
             * 
             * 
             */

            //FIXME: Tuntuu melko turhalta lähettää lista ja guessesLeft kun metodia kutsutaan,
            //       noista muuttujista voisi tehdä... julkisia? static? jotain?

            Console.CursorVisible = false;

            int rightColumn = Console.WindowWidth / 2;
            Console.CursorLeft = rightColumn;
            Console.CursorTop = 1;
            Console.Write("Arvaukset");

            foreach (var item in guessedNumbers)
            {
                Console.CursorLeft = rightColumn;
                Console.CursorTop++;
                DrawColorText(item.ToString());
            }

            for (int i = 0; i < guessesLeft - 1; i++)
            {
                Console.CursorLeft = rightColumn;
                Console.CursorTop++;
                DrawColorText("?", false, true);
            }

            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            Console.Write("Syötä seuraava arvaus:");
            Console.CursorLeft = 0;
            Console.CursorTop = 2;
            Console.CursorVisible = true;
        }

        // Apufunktioita

        // Animoi arpakoneen pyöritystä
        private static void AnimateRandomizer(int maxValue, float animationLength)
        {
            Stopwatch timer = new Stopwatch();

            int startTop = Console.CursorTop;
            int startLeft = Console.CursorLeft;

            Console.CursorVisible = false;

            timer.Start();
            while (timer.ElapsedMilliseconds < animationLength)
            {
                string text = randomizer.Next(1, maxValue).ToString().PadLeft(maxValue.ToString().Length);
                DrawRainbowText(text, true);
                Console.SetCursorPosition(startLeft, startTop);
                Thread.Sleep(33);
            }
            timer.Stop();

            Console.CursorVisible = true;
        }

        // Tulostaa annetun tekstin satunnaisesti väritettynä kirjain kerrallaan
        private static void DrawRainbowText(string text, bool drawBackground = false)
        {
            foreach (char letter in text)
            {
                DrawColorText(letter.ToString(), true, drawBackground);
            }
        }

        // Tulostaa annetun tekstin satunnaisesti väritettynä
        private static void DrawColorText(string text, bool colorizeForeground = true, bool colorizeBackground = false)
        {

            if (colorizeForeground)
            {
                Console.ForegroundColor = (ConsoleColor)randomizer.Next(1, 16); // 0 on musta joten emme käytä sitä
            }

            if (colorizeBackground)
            {
                Console.BackgroundColor = (ConsoleColor)randomizer.Next(1, 16);
            }

            Console.Write(text);
            Console.ResetColor();
        }
    }
}