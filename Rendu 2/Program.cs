using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using OfficeOpenXml;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Rendu_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Graphe g = new Graphe();
            g.DessinerGraphe();
            //int valeur = 0;
            //string val = "";
            //do
            //{
            //    valeur = 0;
            //    val = "";
            //    while (val != "1" && val != "2" && val != "3" && val != "4" && val != "5")
            //    {
            //        Console.Clear();
            //        Console.WriteLine("Bienvenu dans la gestion des itinéraires.");
            //        Console.Write("Que voulez-vous faire: " +
            //            "\nChercher un itinéraire_______________________tappez 1" +
            //            "\nConnaitre la distance entre deux station_____tappez 2" +
            //            "\nConnaitre le temps entre deux stations_______tappez 3" +
            //            "\nAfficher les stations d'une ligne____________tappez 4" +
            //            "\nSortir du programme__________________________tappez 5" + tapez 6
            //            "\nRéponse: ");
            //        val = Console.ReadLine();
            //    }
            //    valeur = Int32.Parse(val);
            //    switch (valeur)
            //    {
            //        case 1:
            //            itineraire(g);
            //            break;
            //        case 2:
            //            distance(g);
            //            break;
            //        case 3:
            //            temps(g);
            //            break;
            //        case 4:
            //            stations(g);
            //            break;
            //        case 5:
            //            break;
            //    }
            //    Console.Clear();
            //} while (valeur != 5);
            //Console.WriteLine(g.afficher_liste_adjacence());
        }

        #region Fonctions
        public static void stations(Graphe g)
        {
            string numero_ligne = "";
            while (numero_ligne != "1" && numero_ligne != "2" && numero_ligne != "3" && numero_ligne != "3bis" && numero_ligne != "4" && numero_ligne != "5" && numero_ligne != "6" && numero_ligne != "7" && numero_ligne != "7bis" && numero_ligne != "8" && numero_ligne != "9" && numero_ligne != "10" && numero_ligne != "11" && numero_ligne != "12" && numero_ligne != "13" && numero_ligne != "14")
            {
                Console.Clear();
                Console.Write("Quel ligne voulez vous regarder: " +
                    "\nLigne 1________tappez 1" +
                    "\nLigne 2________tappez 2" +
                    "\nLigne 3________tappez 3" +
                    "\nLigne 3bis_____tappez 3bis" +
                    "\nLigne 4________tappez 4" +
                    "\nLigne 5________tappez 5" +
                    "\nLigne 6________tappez 6" +
                    "\nLigne 7________tappez 7" +
                    "\nLigne 7bis_____tappez 7bis" +
                    "\nLigne 8________tappez 8" +
                    "\nLigne 9________tappez 9" +
                    "\nLigne 10_______tappez 10" +
                    "\nLigne 11_______tappez 11" +
                    "\nLigne 12_______tappez 12" +
                    "\nLigne 13_______tappez 13" +
                    "\nLigne 14_______tappez 14" +
                    "\nRéponse: ");
                numero_ligne = Console.ReadLine();
            }
            Console.Clear();
            Console.WriteLine("Ligne " + numero_ligne + ":");
            for (int i = 0; i < g.Tab_noeud.Length; i++)
            {
                if (g.Tab_noeud[i].Ligne == numero_ligne)
                {
                    Console.WriteLine(g.Tab_noeud[i].Nom);
                }
            }
            Console.Write("Appuyez sur entrer pour sortir.");
            Console.ReadLine();
            Console.Clear();
        }
        public static void itineraire(Graphe g)
        {
            Console.Clear();
            Console.WriteLine("Liste des stations: ");
            for (int i = 0; i < g.Tab_noeud.Length; i++)
            {
                Console.WriteLine(g.Tab_noeud[i].Nom + " sur la ligne " + g.Tab_noeud[i].Ligne + "_____tappez " + g.Tab_noeud[i].Identifiant);
            }
            bool test = false;
            string station = "";
            while (test == false)
            {
                Console.Write("Entrez le numéro de votre station de départ: ");
                station = Console.ReadLine();
                test = verifier_station(station);
            }
            int station_depart = Int32.Parse(station);
            test = false;
            while (test == false)
            {
                Console.Write("Entrez le numéro de votre station d'arrivée: ");
                station = Console.ReadLine();
                test = verifier_station(station);
            }
            int station_arrivee = Int32.Parse(station);
            var resultat = g.BellmanFord(station_arrivee,station_depart);
            Console.Clear();
            Console.WriteLine("Votre itinéraire: ");
            for (int i = resultat.chemin.Count - 1;i >= 0; i--)
            {
                int pos = pos_station(resultat.chemin[i], g);
                Console.WriteLine(g.Tab_noeud[pos].Nom + " sur la ligne " + g.Tab_noeud[pos].Ligne + ",");
            }
            double temps = (resultat.taille / 32);
            for (int i = 0; i < resultat.chemin.Count; i++)
            {
                temps = temps + 0.0042;
            }
            double temps_heure = Math.Truncate(temps);
            double temps_minute_virgule = Math.Round(temps - temps_heure, 2);
            double temps_minute_entier = (temps_minute_virgule % 1) * 100;
            int temps_minute = (int)temps_minute_entier;
            if (temps_heure > 0)
            {
                Console.WriteLine("Votre trajet va durer " + temps_heure + "h" + temps_minute + "min");
            }
            else
            {
                Console.WriteLine("Votre trajet va durer " + temps_minute + "min");
            }
            Console.Write("Appuyez sur entrer pour sortir.");
            Console.ReadLine();
            Console.Clear();
        }
        public static void distance(Graphe g)
        {
            Console.Clear();
            Console.WriteLine("Liste des stations: ");
            for (int i = 0; i < g.Tab_noeud.Length; i++)
            {
                Console.WriteLine(g.Tab_noeud[i].Nom + " sur la ligne " + g.Tab_noeud[i].Ligne + "_____tappez " + g.Tab_noeud[i].Identifiant);
            }
            bool test = false;
            string station = "";
            while (test == false)
            {
                Console.Write("Entrez le numéro de votre station de départ: ");
                station = Console.ReadLine();
                test = verifier_station(station);
            }
            int station_depart = Int32.Parse(station);
            test = false;
            while (test == false)
            {
                Console.Write("Entrez le numéro de votre station d'arrivée: ");
                station = Console.ReadLine();
                test = verifier_station(station);
            }
            int station_arrivee = Int32.Parse(station);
            Console.Clear();
            var resultat = g.BellmanFord(station_arrivee, station_depart);
            int pos_1 = pos_station(station_depart, g);
            int pos_2 = pos_station(station_arrivee, g);
            double distance = resultat.taille;
            if (distance < 1)
            {
                distance = distance * 1000;
                int dist = (int)distance;
                Console.WriteLine("La distance entre " + g.Tab_noeud[pos_1].Nom + " et " + g.Tab_noeud[pos_2].Nom + " est de " + dist + "m.");
            }
            else
            {
                distance = Math.Round(distance,2);
                Console.WriteLine("La distance entre " + g.Tab_noeud[pos_1].Nom + " et " + g.Tab_noeud[pos_2].Nom + " est de " + distance + "km.");
            }
            Console.Write("Appuyez sur entrer pour sortir.");
            Console.ReadLine();
            Console.Clear();
        }
        public static void temps(Graphe g)
        {
            Console.Clear();
            Console.WriteLine("Liste des stations: ");
            for (int i = 0; i < g.Tab_noeud.Length; i++)
            {
                Console.WriteLine(g.Tab_noeud[i].Nom + " sur la ligne " + g.Tab_noeud[i].Ligne + "_____tappez " + g.Tab_noeud[i].Identifiant);
            }
            bool test = false;
            string station = "";
            while (test == false)
            {
                Console.Write("Entrez le numéro de votre station de départ: ");
                station = Console.ReadLine();
                test = verifier_station(station);
            }
            int station_depart = Int32.Parse(station);
            test = false;
            while (test == false)
            {
                Console.Write("Entrez le numéro de votre station d'arrivée: ");
                station = Console.ReadLine();
                test = verifier_station(station);
            }
            int station_arrivee = Int32.Parse(station);
            Console.Clear();
            var resultat = g.BellmanFord(station_arrivee, station_depart);
            int pos_1 = pos_station(station_depart, g);
            int pos_2 = pos_station(station_arrivee, g);
            double temps = (resultat.taille / 32);
            for (int i = 0; i < resultat.chemin.Count; i++)
            {
                temps = temps + 0.0042;
            }
            double temps_heure = Math.Truncate(temps);
            double temps_minute_virgule = Math.Round(temps - temps_heure, 2);
            double temps_minute_entier = (temps_minute_virgule % 1) * 100;
            int temps_minute = (int)temps_minute_entier;
            if (temps_heure > 0)
            {
                Console.WriteLine("Le temps entre les deux stations est de " + temps_heure + "h" + temps_minute + "min.");
            }
            else
            {
                Console.WriteLine("Le temps entre les deux stations est de " + temps_minute + "min.");
            }
            Console.Write("Appuyez sur entrer pour sortir.");
            Console.ReadLine();
            Console.Clear();
        }
        public static int pos_station(int identifiant, Graphe g)
        {
            int position = 0;
            for (int i = 0;  i < g.Tab_noeud.Length; i++)
            {
                if (Int32.Parse(g.Tab_noeud[i].Identifiant) == identifiant)
                {
                    position = i;
                }
            }
            return position;
        }
        public static bool verifier_station(string val)
        {
            bool test = false;
            try
            {
                int valeur = Int32.Parse(val);
                if (valeur > 0 && valeur <= 332 && valeur != 180 && valeur != 317)
                {
                    test = true;
                }
            }
            catch
            {
                test = false;
            }
            return test;
        }
        #endregion
    }
}