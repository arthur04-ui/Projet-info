using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Rendu_2
{
    internal class Noeud
    {
        #region Attributs
        string nom;
        string ligne;
        string lon;
        string lat;
        string identifiant;
        #endregion

        #region Accès
        public string Lat
        {
            get { return lat; }
            set { lat = value; }
        }
        public string Lon
        {
            get { return lon; }
            set { lon = value; }
        }
        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }
        public string Identifiant
        {
            get { return identifiant; }
            set { identifiant = value; }
        }
        public string Ligne
        {
            get { return ligne; }
            set { ligne = value; }
        }
        #endregion

        #region Constructeur
        public Noeud(int ligne)
        {
            string filePath = "MetroParis.xlsx";
            ligne = ligne + 2;
            if (File.Exists(filePath))
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo("MetroParis.xlsx")))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rows = worksheet.Dimension.Rows;
                    int cols = worksheet.Dimension.Columns;
                    this.nom = Convert.ToString(worksheet.Cells[ligne, 3].Value);
                    this.ligne = Convert.ToString(worksheet.Cells[ligne, 2].Value);
                    this.lon = Convert.ToString(worksheet.Cells[ligne, 4].Value);
                    this.lat = Convert.ToString(worksheet.Cells[ligne, 5].Value);
                    this.identifiant = Convert.ToString(worksheet.Cells[ligne, 1].Value);
                }
            }
            else
            {
                Console.WriteLine("Fichier introuvable !");
            }
        }
        #endregion

        #region Fonctions
        public void afficher_noeud()
        {
            Console.WriteLine("Noeud: " + this.nom + ", " + this.ligne + ", " + this.lon + ", " + this.lat + ", " + this.identifiant);
        }
        public double distance_noeud(Noeud noeud2)
        {
            double distance = 0;
            double lat1 = Math.PI / 180 * double.Parse(this.lat, CultureInfo.InvariantCulture);
            double lat2 = Math.PI / 180 * double.Parse(noeud2.Lat, CultureInfo.InvariantCulture);
            double lon1 = Math.PI / 180 * double.Parse(this.lon, CultureInfo.InvariantCulture);
            double lon2 = Math.PI / 180 * double.Parse(noeud2.Lon, CultureInfo.InvariantCulture);
            distance = 2 * 6371 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(lat2 - lat1) / 2, 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin((lon2 - lon1) / 2), 2)));
            return distance;
        }
        #endregion
    }
}