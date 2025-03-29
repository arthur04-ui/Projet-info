using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace Rendu_2
{
    internal class Graphe
    {
        #region Attributs
        Dictionary<int, List<int>> liste_adjacence;
        Noeud[] tab_noeud;
        Lien[] tab_lien;
        Dictionary<int, Point> positions_sommets;
        Dictionary<(int, int), double> poids_aretes;
        #endregion

        #region Acces
        public Noeud[] Tab_noeud
        {
            get { return tab_noeud; }
            set { tab_noeud = value; }
        }
        public Lien[] Tab_lien
        {
            get { return tab_lien; }
            set { tab_lien = value; }
        }
        public Dictionary<(int,int), double > Poids_aretes
        {
            get { return poids_aretes; }
            set { poids_aretes = value; }
        }
        #endregion

        #region Constructeur
        public Graphe()
        {
            this.tab_noeud = new Noeud[330];
            this.tab_lien = new Lien[330];
            for (int i = 0; i < tab_noeud.Length; i++)
            {
                this.tab_noeud[i] = new Noeud(i);
                //this.tab_noeud[i].afficher_noeud();
            }
            for (int i = 0; i < tab_lien.Length; i++)
            {
                this.tab_lien[i] = new Lien(i);
                //this.tab_lien[i].afficher_lien();
            }
            for (int i = 0; i < tab_lien.Length; i++)
            {
                for (int j = 0; j < tab_lien.Length; j++)
                {
                    if (tab_noeud[i].Nom == tab_noeud[j].Nom && i != j)
                    {
                        tab_lien[i].Destination.Add(Int32.Parse(tab_noeud[j].Identifiant));
                    }
                }
            }
            this.liste_adjacence = new Dictionary<int, List<int>>();
            this.poids_aretes = new Dictionary<(int, int), double>();
            for (int i = 0; i < tab_lien.Length; i++)
            {
                for (int j = 0; j < tab_lien[i].Destination.Count; j++)
                {
                    if (!this.liste_adjacence.ContainsKey(this.tab_lien[i].Sommet))
                    {
                        this.liste_adjacence[this.tab_lien[i].Sommet] = new List<int>();
                    }
                    this.liste_adjacence[this.tab_lien[i].Sommet].Add(this.tab_lien[i].Destination[j]);
                }
            }
            for (int i = 0; i < tab_lien.Length; i++)
            {
                for (int j = 0; j < tab_lien[i].Destination.Count; j++)
                {
                    int pos_s1 = 0;
                    int pos_s2 = 0;
                    for (int k = 0; k < tab_noeud.Length; k++)
                    {
                        if (Int32.Parse(tab_noeud[k].Identifiant) == tab_lien[i].Sommet)
                        {
                            pos_s1 = k;
                        }
                    }
                    for (int k = 0; k < tab_noeud.Length; k++)
                    {
                        if (Int32.Parse(tab_noeud[k].Identifiant) == tab_lien[i].Destination[j])
                        {
                            pos_s2 = k;
                        }
                    }
                    poids_aretes[(tab_lien[i].Sommet, tab_lien[i].Destination[j])] = tab_noeud[pos_s1].distance_noeud(tab_noeud[pos_s2]);
                }
            }
            positions_sommets = new Dictionary<int, Point>();
        }
        #endregion

        #region Fonctions
        /// <summary>
        /// Afficher la liste d'adjacence
        /// </summary>
        /// <returns>Un string contenant la liste d'adjacence</returns>
        public string afficher_liste_adjacence()
        {
            string chaine = "Liste d'adjacence: ";
            foreach (var key in liste_adjacence.Keys)
            {
                chaine = chaine + "\n" + "Sommet " + key + ": ";
                foreach (var destination in this.liste_adjacence[key])
                {
                    chaine = chaine + destination + " ";
                }
            }
            return chaine;
        }
        public (List<int> chemin, double taille) BellmanFord(int source, int destination)
        {
            // Dictionnaires pour stocker les distances, les prédécesseurs et les chemins
            Dictionary<int, double> distances = new Dictionary<int, double>();
            Dictionary<int, int?> predecessors = new Dictionary<int, int?>();

            // Initialisation : distance infinie sauf pour la source
            foreach (var sommet in liste_adjacence.Keys)
            {
                distances[sommet] = double.PositiveInfinity;
                predecessors[sommet] = null;
            }
            distances[source] = 0;

            // Relaxation des arêtes |V| - 1 fois
            int nombreSommets = tab_noeud.Length;
            for (int i = 0; i < nombreSommets - 1; i++)
            {
                foreach (var u in liste_adjacence.Keys)
                {
                    foreach (var v in liste_adjacence[u])
                    {
                        double poids = poids_aretes.ContainsKey((u, v)) ? poids_aretes[(u, v)] : 1.0; // Poids par défaut = 1 si absent
                        if (distances[u] != double.PositiveInfinity && distances[u] + poids < distances[v])
                        {
                            distances[v] = distances[u] + poids;
                            predecessors[v] = u;
                        }
                    }
                }
            }

            // Détection de cycles négatifs
            foreach (var u in liste_adjacence.Keys)
            {
                foreach (var v in liste_adjacence[u])
                {
                    double poids = poids_aretes.ContainsKey((u, v)) ? poids_aretes[(u, v)] : 1.0;
                    if (distances[u] != double.PositiveInfinity && distances[u] + poids < distances[v])
                    {
                        throw new Exception("Cycle négatif détecté !");
                    }
                }
            }

            // Reconstituer le chemin du sommet source au sommet destination
            List<int> chemin = new List<int>();
            for (int? node = destination; node != null; node = predecessors[node.Value])
            {
                chemin.Insert(0, node.Value);
            }

            // Si le chemin ne commence pas par le sommet source, cela signifie que le sommet est inaccessible
            if (chemin[0] != source)
            {
                return (new List<int>(), double.PositiveInfinity); // Aucun chemin trouvé
            }

            // Retourner la liste des sommets et la taille (distance) du chemin
            return (chemin, distances[destination]);
        }


        #region Fonctions d'affichage

        /// <summary>
        /// Dessiner le graphe en fonction des coordonnées des noeuds
        /// </summary>
        public void DessinerGraphe()
        {
            Form form = new Form
            {
                Text = "Visualisation du Graphe",
                Size = new Size(800, 600),
                BackColor = System.Drawing.Color.White
            };

            // Générer les positions des noeuds à partir de leurs coordonnées lat/long
            genererPositionsNoeuds();

            form.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Dessiner les connexions (arêtes)
                foreach (var sommet in liste_adjacence)
                {
                    foreach (var voisin in sommet.Value)
                    {
                        Point start = positions_sommets[sommet.Key];
                        Point end = positions_sommets[voisin];

                        g.DrawLine(Pens.Black, start, end);
                    }
                }

                // Dessiner les noeuds (stations)
                foreach (var sommet in liste_adjacence.Keys)
                {
                    Point position = positions_sommets[sommet];
                    Rectangle nodeRect = new Rectangle(position.X - 5, position.Y - 5, 10, 10);
                    g.FillEllipse(System.Drawing.Brushes.Blue, nodeRect);
                    g.DrawEllipse(Pens.Black, nodeRect);
                    g.DrawString(sommet.ToString(), new Font("Arial", 8), System.Drawing.Brushes.Black, position.X + 5, position.Y - 5);
                }
            };
            System.Windows.Forms.Application.Run(form);
        }

        /// <summary>
        /// Génère les positions des noeuds à partir de leurs coordonnées GPS
        /// </summary>
        private void genererPositionsNoeuds()
        {
            int espacementX = 35;  // Espacement horizontal (on peut l'ajuster pour mieux positionner)
            int espacementY = 40;  // Espacement vertical (ajustable aussi)
            int xDepart = 50;      // Position X de départ
            int yDepart = 50;      // Position Y de départ

            // Assumer que les noeuds ont des coordonnées latitude/longitude et les convertir en positions à l'écran
            foreach (var sommet in liste_adjacence.Keys)
            {
                // Utilisation des coordonnées latitude/longitude de chaque noeud (latitude = lat, longitude = long)
                double latitude = Double.Parse(tab_noeud[sommet].Lat);
                double longitude = Double.Parse(tab_noeud[sommet].Lon);

                // Conversion simple des coordonnées en coordonnées écran (à adapter selon ton cas)
                // (Les coordonnées GPS ne sont pas directement utilisables en pixels sur une fenêtre, donc on fait un calcul simple ici)
                int xPos = xDepart + (int)(longitude * espacementX);
                int yPos = yDepart + (int)(latitude * espacementY);

                positions_sommets[sommet] = new Point(xPos, yPos);
            }
        }

        #endregion





        //public void dessiner_graphe()
        //{
        //    var form = new Form()
        //    {
        //        Text = "Visualisation du Graphe",
        //        Size = new Size(1200, 900),
        //        BackColor = System.Drawing.Color.White
        //    };

        //    generer_positions_sommets();

        //    form.Paint += (sender, e) =>
        //    {
        //        Graphics g = e.Graphics;
        //        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        //        Font font = new Font("Arial", 9, FontStyle.Bold);

        //        int nodeRadius = 15;
        //        HashSet<Tuple<int, int>> drawnEdges = new HashSet<Tuple<int, int>>();

        //        foreach (var sommet in liste_adjacence)
        //        {
        //            foreach (var voisin in sommet.Value)
        //            {
        //                var edge = Tuple.Create(Math.Min(sommet.Key, voisin), Math.Max(sommet.Key, voisin));
        //                if (drawnEdges.Contains(edge)) continue;
        //                drawnEdges.Add(edge);

        //                Point start = positions_sommets[sommet.Key];
        //                Point end = positions_sommets[voisin];

        //                PointF newEnd = AdjustEndPosition(start, end, nodeRadius);

        //                int midX = (start.X + end.X) / 2;
        //                int midY = (start.Y + end.Y) / 2;
        //                int curveOffset = Math.Abs(start.X - end.X) > 50 ? 30 : 20; // Réduction de l'écart
        //                Point controlPoint = new Point(midX, midY - curveOffset);

        //                using (GraphicsPath path = new GraphicsPath())
        //                {
        //                    path.AddBezier(start, controlPoint, controlPoint, newEnd);
        //                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, 1))
        //                    {
        //                        g.DrawPath(pen, path);

        //                        if (est_orientee(sommet.Key, voisin))
        //                        {
        //                            DrawArrowOnCurve(g, pen, newEnd, controlPoint);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var sommet in liste_adjacence.Keys)
        //        {
        //            Point position = positions_sommets[sommet];
        //            Rectangle nodeRect = new Rectangle(position.X - nodeRadius, position.Y - nodeRadius, nodeRadius * 2, nodeRadius * 2);
        //            g.FillEllipse(System.Drawing.Brushes.Blue, nodeRect);
        //            g.DrawEllipse(Pens.Black, nodeRect);
        //            g.DrawString(sommet.ToString(), font, System.Drawing.Brushes.White, position.X - 8, position.Y - 8);
        //        }
        //    };

        //    System.Windows.Forms.Application.Run(form);
        //}
        //private void generer_positions_sommets()
        //{
        //    int espacementX = 35; // Espacement légèrement réduit
        //    int espacementY = 40; // Espacement légèrement réduit
        //    int xDepart = 0;      // Suppression de l'espace initial
        //    int yDepart = 0;      // Suppression de l'espace initial
        //    int yPos = yDepart;

        //    int[][] lignes = new int[][]
        //    {
        //Enumerable.Range(1, 19).ToArray(),
        //Enumerable.Range(20, 25).ToArray(),
        //Enumerable.Range(45, 21).ToArray(),
        //Enumerable.Range(66, 4).ToArray(),
        //Enumerable.Range(70, 26).ToArray(),
        //Enumerable.Range(96, 18).ToArray(),
        //Enumerable.Range(114, 28).ToArray(),
        //Enumerable.Range(142, 29).ToArray(),
        //Enumerable.Range(171, 9).ToArray(),
        //Enumerable.Range(181, 29).ToArray(),
        //Enumerable.Range(210, 31).ToArray(),
        //Enumerable.Range(241, 21).ToArray(),
        //Enumerable.Range(262, 12).ToArray(),
        //Enumerable.Range(274, 26).ToArray(),
        //Enumerable.Range(300, 20).ToArray(),
        //Enumerable.Range(320, 13).ToArray()
        //    };

        //    foreach (var ligne in lignes)
        //    {
        //        int xPos = xDepart;
        //        foreach (var sommet in ligne)
        //        {
        //            positions_sommets[sommet] = new Point(xPos, yPos);
        //            xPos += espacementX;
        //        }
        //        yPos += espacementY;
        //    }
        //}
        //private PointF AdjustEndPosition(Point start, Point end, int radius)
        //{
        //    double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
        //    return new PointF(
        //        end.X - (float)(radius * Math.Cos(angle)),
        //        end.Y - (float)(radius * Math.Sin(angle))
        //    );
        //}
        //private bool est_orientee(int start, int end)
        //{
        //    return liste_adjacence.ContainsKey(start) && liste_adjacence[start].Contains(end);
        //}
        //private void DrawArrowOnCurve(Graphics g, System.Drawing.Pen pen, PointF end, Point control)
        //{
        //    double arrowLength = 8;
        //    double arrowAngle = Math.PI / 6;

        //    double angle = Math.Atan2(end.Y - control.Y, end.X - control.X);

        //    PointF arrow1 = new PointF(
        //        (float)(end.X - arrowLength * Math.Cos(angle - arrowAngle)),
        //        (float)(end.Y - arrowLength * Math.Sin(angle - arrowAngle))
        //    );

        //    PointF arrow2 = new PointF(
        //        (float)(end.X - arrowLength * Math.Cos(angle + arrowAngle)),
        //        (float)(end.Y - arrowLength * Math.Sin(angle + arrowAngle))
        //    );

        //    g.DrawLine(pen, end, arrow1);
        //    g.DrawLine(pen, end, arrow2);
        //}
        #endregion
    }
}