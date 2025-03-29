using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Windows.Media.Media3D;

namespace Rendu_2
{
    internal class Lien
    {
        #region Attributs
        int sommet;
        List<int> destination;
        #endregion

        #region Accès
        public int Sommet
        {
            get { return sommet; }
            set { sommet = value; }
        }
        public List<int> Destination
        {
            get { return destination; }
            set { destination = value; }
        }
        #endregion

        #region Constructeur
        public Lien(int i)
        {
            string filePath = "MetroParis.xlsx";
            i = i + 2;
            destination = new List<int>();
            if (File.Exists(filePath))
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[1];
                    string sommet_val = Convert.ToString(worksheet.Cells[i, 1].Value);
                    string val1 = Convert.ToString(worksheet.Cells[i, 3].Value);
                    string val2 = Convert.ToString(worksheet.Cells[i, 4].Value);
                    this.sommet = Int32.Parse(sommet_val);
                    if (val1 != null && val1 != "" && val1 != " ")
                    {
                        destination.Add(Int32.Parse(val1));
                    }
                    if (val2 != null && val2 != "" && val2 != " ")
                    {
                        destination.Add(Int32.Parse(val2));
                    }
                }
            }
            else
            {
                Console.WriteLine("Fichier introuvable !");
            }
        }
        #endregion

        #region Fonctions
        public void afficher_lien()
        {
            Console.Write(this.sommet + ": ");
            for (int i = 0; i < this.destination.Count; i++)
            {
                Console.Write(this.destination[i] + " ");
            }
            Console.WriteLine();
        }
        #endregion 
    }
}