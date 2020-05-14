using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Probleme_info
{
    /// <summary>
    /// j'ai cree un classe de Myimage ou il ya tout les info de l'image taille , type ... etc 
    /// il ya aussi une matrice de pixel qui contient les piexel de l'image or chaque pixel est forme de Rouge Vert Bleu
    /// Pour les fonction , la classe est bien divier par region 
    /// la permiere ou il ya les fonction principal pour la gestion des donne de l'image pour les quelles j'ai fait des test unitaire 
    /// la deusieme est celle de la mainpulation de l'image il ya TD2(Agrandissement, rotation ...) 
    /// la troiseme pour le TD3 les flitre et 4eme pour la creation des forme et histogramme 
    /// finalement celle de mes innovation
    /// </summary>
    public class MyImage
    {
        /// Champs
        #region 
        private string typeDeLImage;
        private int tailleDuficher;
        private int tailleDuOffset;
        private int largeur;
        private int hauteur;
        private int nbDeBitParColeur;
        private Pixel[,] matriceRGB;
        private bool ficherExiste= true;
        #endregion
        /// Constructeur 
        /// j'ajoute le try catche avec la varible de bool le fiche exciste
        public MyImage(string myfileBMPouCSV)
        {
            byte[] myfileByte = new byte[0];
           
            ///pour lire un ficher CSV et le convertir a un tab sde byte
            if (myfileBMPouCSV[myfileBMPouCSV.Length - 1].Equals('v'))
            {
                try
                {
                    string[] myfileString;
                    myfileString = File.ReadAllLines(myfileBMPouCSV);
                    myfileByte = new byte[myfileString.Length * myfileString.Length];
                    int indexFlu = 0;
                    for (int i = 0; i < myfileString.Length; i++)
                    {
                        string[] s = Regex.Split(myfileString[i], ";");
                        for (int j = 0; j < s.Length; j++)
                        {
                            if (!s[j].Equals(""))
                            {
                                myfileByte[indexFlu] = Convert.ToByte(s[j]);
                                indexFlu++;
                            }
                        }
                    }
                    /// la recuperation des donne taille ....et image RGB
                    Inisialisation(myfileByte);
                }
                catch
                {
                    ficherExiste = false;
                    Console.WriteLine("Fiche n'existe pas ");
                }
                
            }
            ///pour lire un ficher BMP
            else
            {
                try
                {
                    myfileByte = File.ReadAllBytes(myfileBMPouCSV);
                    /// la recuperation des donne taille ....et image RGB
                    Inisialisation(myfileByte);
                }
                catch (Exception)
                {
                    ficherExiste = false;
                    Console.WriteLine("Fiche n'existe pas ");
                }
               
            }
            //for (int i = 0; i < 54; i++)
            //{
            //    Console.Write(myfileByte[i] + "\t |");
            //}
            //Console.WriteLine();
        }
        /// ce constructeur est pour la creation de l'image de rein
        public MyImage(int Largeur, int Hauteur)
        {
            this.largeur = Largeur;
            this.hauteur = Hauteur;
            this.typeDeLImage = "BM";
            this.tailleDuOffset = 54;
            this.tailleDuficher = hauteur * largeur * 3 + tailleDuOffset;
            this.nbDeBitParColeur = 24;
            matriceRGB = new Pixel[largeur, hauteur];
            for (int i = 0; i < this.matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < this.matriceRGB.GetLength(1); j++)
                {
                    matriceRGB[i, j] = new Pixel(255, 255, 255);
                }
            }
        }
        /// Proprieter 
        #region
        public string TypeDeLImage { get => typeDeLImage; set => typeDeLImage = value; }
        public int TailleDufiche { get => tailleDuficher; set => tailleDuficher = value; }
        public int TailleDuOffset { get => tailleDuOffset; set => tailleDuOffset = value; }
        public int Largeur { get => largeur; set => largeur = value; }
        public int Hauteur { get => hauteur; set => hauteur = value; }
        public int NbDeBitParColeur { get => nbDeBitParColeur; set => nbDeBitParColeur = value; }
        public Pixel[,] MatriceRGB { get => matriceRGB; set => matriceRGB = value; }
        public bool FicherExiste { get => ficherExiste; set => ficherExiste = value; }
        #endregion
        /// Methode
        #region foncition principal 
        public void Inisialisation(byte[] myfileByte)
        {
            this.typeDeLImage = ((char)myfileByte[0]).ToString() + ((char)myfileByte[1]).ToString();
            tailleDuficher = Convertir_Endian_To_Int(SortirUnTabDe4Bytes(myfileByte, 2, 5));
            tailleDuOffset = Convertir_Endian_To_Int(SortirUnTabDe4Bytes(myfileByte, 10, 13));
            largeur = Convertir_Endian_To_Int(SortirUnTabDe4Bytes(myfileByte, 18, 21));
            hauteur = Convertir_Endian_To_Int(SortirUnTabDe4Bytes(myfileByte, 22, 25));
            nbDeBitParColeur = Convertir_Endian_To_Int(SortirUnTabDe4Bytes(myfileByte, 28, 29));
            matriceRGB = new Pixel[largeur, hauteur];
            ConvertirToMatriceRGB(myfileByte);
        }
        ///Ca aide a diviser le header ou header info a un petit tableu pour le convertir  de endien en int 
        public byte[] SortirUnTabDe4Bytes(byte[] grandTab, int indexDebut, int indexfin)
        {
            byte[] tab = new byte[(indexfin - indexDebut) + 1];
            int j = 0;
            for (int i = indexDebut; i <= indexfin; i++)
            {
                tab[j] = grandTab[i];
                j++;
            }
            return tab;
        }
        ///Quellque fonction de convertir 
       public int Puissance (int valeur , int saPuissance )
        {
            int valeurSortir = 1;
            for (int i = 0; i < saPuissance; i++)
            {
                valeurSortir *= valeur;

            }
            return valeurSortir;
        }
        public int Convertir_Endian_To_Int(byte[] tab)
        {
            int laValeurEnINT = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                laValeurEnINT += tab[i] * Puissance(2, 8 * (i));
            }
            return laValeurEnINT;
        }
        public byte[] Convertir_Int_To_Endian(int val, int tailleTab)
        {
            byte[] tab = new byte[tailleTab];
            for (int i = (tab.Length - 1); i >= 0; i--)
            {
                tab[i] = (byte)(val / Puissance(2, 8 * (i)));
                val = (val % Puissance(2, 8 * (i)));
            }
            return tab;
        }
        public void ConvertirToMatriceRGB(byte[] tab)
        {
            int x = 0; int y = 0;
            for (int i = tailleDuOffset; i < tailleDuficher-2; i = i + 3)
            {
                this.matriceRGB[x, y] = new Pixel(tab[i], tab[i + 1], tab[i + 2]);
                x++;
                if (x == largeur)
                {
                    x = 0;
                    y++;
                }
            }
        }
        ///Ca serait a afficher pour m'aider a trouver mes fautes
        public void AfficheMatriceRGB()
        {
            for (int i = 0; i < matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < matriceRGB.GetLength(1); j++)
                {
                    Console.Write(matriceRGB[i, j].toString());
                }
                Console.WriteLine();
            }
        }
        public void Affiche(byte[] tab)
        {
            for (int i = 0; i < tab.Length; i++)
            {
                Console.Write(tab[i] + " ");
            }
            Console.WriteLine();
        }
        ///transformer l'image apres la mnipulation a une nouvelle image 
        public void From_Image_To_File(string file, int BMPouCSV)
        {
            byte[] myfileByte = new byte[tailleDuficher];
            ///la construction du header le harder
            myfileByte[0] = (byte)typeDeLImage[0];
            myfileByte[1] = (byte)typeDeLImage[1];
            byte[] tab = Convertir_Int_To_Endian(tailleDuficher, 4);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 2] = tab[i];
            tab = Convertir_Int_To_Endian(0, 4);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 6] = tab[i];
            tab = Convertir_Int_To_Endian(tailleDuOffset, 4);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 10] = tab[i];
            tab = Convertir_Int_To_Endian(tailleDuOffset - 14, 4);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 14] = tab[i];
            tab = Convertir_Int_To_Endian(largeur, 4);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 18] = tab[i];
            tab = Convertir_Int_To_Endian(hauteur, 4);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 22] = tab[i];
            tab = Convertir_Int_To_Endian(1, 2);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 26] = tab[i];
            tab = Convertir_Int_To_Endian(nbDeBitParColeur, 2);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 28] = tab[i];
            tab = Convertir_Int_To_Endian(0, 4);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 30] = tab[i];
            tab = Convertir_Int_To_Endian(largeur * hauteur * 3, 4);
            for (int i = 0; i < tab.Length; i++) myfileByte[i + 34] = tab[i];
            for (int i = 38; i < tailleDuOffset; i++) myfileByte[i] = 0;
            ///ici on comence l'image 
            int x = 0; int y = 0;
            for (int i = tailleDuOffset; i < myfileByte.Length-2; i = i + 3)
            {
                myfileByte[i] = (byte)matriceRGB[x, y].R1;
                myfileByte[i + 1] = (byte)matriceRGB[x, y].G1;
                myfileByte[i + 2] = (byte)matriceRGB[x, y].B1;
                x++;
                if (x == largeur)
                {
                    x = 0;
                    y++;
                }
            }
            /// pour choisir le format de sortir
            switch (BMPouCSV)
            {
                case 1:
                    File.WriteAllBytes(file+".bmp", myfileByte);
                    Process.Start(file + ".bmp");
                    break;
                case 2:
                    StreamWriter sWriter = null;
                    try
                    {
                        FileStream fileStream = new FileStream(file + ".csv", FileMode.Create, FileAccess.Write);
                        sWriter = new StreamWriter(fileStream);
                        for (int i = 0; i < 14; i++) sWriter.Write(myfileByte[i] + ";");
                        sWriter.WriteLine();
                        for (int i = 14; i < 54; i++) sWriter.Write(myfileByte[i] + ";");
                        sWriter.WriteLine();
                        for (int i = 54; i < myfileByte.Length; i++)
                        {
                            sWriter.Write(myfileByte[i] + ";");
                            if ((i - 53) % largeur == 0) sWriter.WriteLine();
                        }
                        Process.Start(file + ".csv");
                    }
                    finally
                    {
                        if (sWriter != null) sWriter.Close();
                    }
                    break;
                default:
                    File.WriteAllBytes(file + ".bmp", myfileByte);
                    Process.Start(file + ".bmp");
                    break;
            }
            //for (int i = 0; i < 54; i++)
            //{
            //    Console.Write(myfileByte[i] + "\t |");
            //}

        }
        #endregion
        #region Manipulation de l'image
        public void Rotation(int degree)
        {
            int nbDeTour = degree / 90;
            for (int k = 0; k < nbDeTour; k++)
            {
                Pixel[,] matriceIntermaire = new Pixel[this.hauteur, this.largeur];
                for (int i = 0; i < matriceIntermaire.GetLength(0); i++)
                {
                    for (int j = 0; j < matriceIntermaire.GetLength(1); j++)
                    {
                        matriceIntermaire[i, j] = this.MatriceRGB[(this.largeur - 1) - j, i];
                    }
                }
                int inter = this.hauteur;
                this.hauteur = this.largeur;
                this.largeur = inter;
                this.matriceRGB = matriceIntermaire;
            }
        }
        public void Miroir()
        {
            Pixel[,] matriceIntermaire = new Pixel[this.largeur, this.hauteur];

            for (int i = 0; i < matriceIntermaire.GetLength(0); i++)
            {
                for (int j = 0; j < matriceIntermaire.GetLength(1); j++)
                {
                    matriceIntermaire[i, j] = this.matriceRGB[(this.largeur - 1) - i, j];
                }
            }
            this.matriceRGB = matriceIntermaire;
        }
        public void NoirBlanc()
        {
            for (int i = 0; i < this.matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < this.matriceRGB.GetLength(1); j++)
                {
                    this.matriceRGB[i, j].NoirEtBlanc();
                }
            }
        }
        public void NuanceDuGris()
        {
            for (int i = 0; i < this.matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < this.matriceRGB.GetLength(1); j++)
                {
                    this.matriceRGB[i, j].DegreGris();
                }
            }
        }
        public void Superposition(MyImage LaDeusiemeImage)
        {
            int laGrandLargeur, laGrandHauteur;
            Pixel[,] laPetitLargeurImage, laPetitHauteurImage;
            Pixel[,] laGrandLargeurImage, laGrandHauteurImage;
            if (this.largeur > LaDeusiemeImage.Largeur)
            {
                laGrandLargeur = this.largeur;
                laPetitLargeurImage = LaDeusiemeImage.MatriceRGB;
                laGrandLargeurImage = this.MatriceRGB;
            }
            else
            {
                laGrandLargeur = LaDeusiemeImage.Largeur;
                laPetitLargeurImage = this.MatriceRGB;
                laGrandLargeurImage = LaDeusiemeImage.MatriceRGB;
            }

            if (this.hauteur > LaDeusiemeImage.Hauteur)
            {
                laGrandHauteur = this.hauteur;
                laPetitHauteurImage = LaDeusiemeImage.matriceRGB;
                laGrandHauteurImage = this.MatriceRGB;
            }
            else
            {
                laGrandHauteur = LaDeusiemeImage.Hauteur;
                laPetitHauteurImage = this.MatriceRGB;
                laGrandHauteurImage = LaDeusiemeImage.MatriceRGB;
            }
            Pixel[,] imageDeSortie = new Pixel[laGrandLargeur, laGrandHauteur];
            for (int i = 0; i < imageDeSortie.GetLength(0); i++)
            {
                for (int j = 0; j < imageDeSortie.GetLength(1); j++)
                {
                    if (j >= laPetitHauteurImage.GetLength(1) && i >= laPetitLargeurImage.GetLength(0) && laPetitHauteurImage != laPetitLargeurImage)// reverifier cette condition
                    {
                        imageDeSortie[i, j] = new Pixel(225, 225, 225);
                    }
                    else if (i >= laPetitLargeurImage.GetLength(0))
                    {
                        imageDeSortie[i, j] = laGrandLargeurImage[i, j];
                    }
                    else if (j >= laPetitHauteurImage.GetLength(1))
                    {
                        imageDeSortie[i, j] = laGrandHauteurImage[i, j];
                    }

                    else imageDeSortie[i, j] = this.matriceRGB[i, j].Superposition(LaDeusiemeImage.MatriceRGB[i, j]);

                }
            }
            this.largeur = laGrandLargeur;
            this.hauteur = laGrandHauteur;
            tailleDuficher = largeur * hauteur * 3 + tailleDuOffset;
            this.matriceRGB = imageDeSortie;
        }
        public void Agrandir(int Agrandissement)
        {
            Agrandissement = ((int)Agrandissement / 2) * 2;//pour s'assurer que ca sera tj divisible par 4
            Pixel[,] matriceIntermaire = new Pixel[this.largeur * Agrandissement, this.hauteur * Agrandissement];
            for (int i = 0; i < matriceIntermaire.GetLength(0); i++)
            {
                for (int j = 0; j < matriceIntermaire.GetLength(1); j++)
                {
                    int k = (int)((i * this.largeur) / matriceIntermaire.GetLength(0));
                    int l = (int)((j * this.hauteur) / matriceIntermaire.GetLength(1));
                    matriceIntermaire[i, j] = this.matriceRGB[k, l];
                }
            }
            this.hauteur = this.hauteur * Agrandissement;
            this.largeur = this.largeur * Agrandissement;
            this.tailleDuficher = this.hauteur * this.largeur * 3 + this.tailleDuOffset;
            this.matriceRGB = matriceIntermaire;
        }
        public void Retrecir(int retrecissement)
        {
            retrecissement = ((int)retrecissement / 2) * 2;//pour s'assurer que ca sera tj divisible par 4
            Pixel[,] matriceIntermaire = new Pixel[this.largeur / retrecissement, this.hauteur / retrecissement];

            for (int i = 0; i < matriceIntermaire.GetLength(0); i++)
            {
                for (int j = 0; j < matriceIntermaire.GetLength(1); j++)
                {

                    int k = (int)((i * this.largeur) / matriceIntermaire.GetLength(0));
                    int l = (int)((j * this.hauteur) / matriceIntermaire.GetLength(1));
                    matriceIntermaire[i, j] = this.matriceRGB[k, l];
                }
            }

            this.hauteur = this.hauteur / retrecissement;
            this.largeur = this.largeur / retrecissement;
            this.tailleDuficher = this.hauteur * this.largeur * 3 + this.tailleDuOffset;
            this.matriceRGB = matriceIntermaire;
        }
        #endregion
        #region Filtre
        private Pixel multiplication (int[,] matriceDeConvolution, int indexI, int IndexJ)
        {
            Pixel resulta = new Pixel(0,0,0);
            int diviseur = 0;
            int k = indexI - 1;
            int l = IndexJ - 1;
            for (int i = 0; i <matriceDeConvolution.GetLength(0); i++)
            {
                l = IndexJ - 1;
                for (int j = 0; j < matriceDeConvolution.GetLength(1); j++)
                {
                    resulta.R1 += (matriceDeConvolution[i, j] * this.matriceRGB[k, l].R1);
                    resulta.G1 += (matriceDeConvolution[i, j] * this.matriceRGB[k, l].G1);
                    resulta.B1 += (matriceDeConvolution[i, j] * this.matriceRGB[k, l].B1);
                    diviseur += matriceDeConvolution[i, j];
                    l++;
                }
                k++;
            }
            if (diviseur == 0)
            {
                diviseur = 3;
            }
            resulta.R1 = resulta.R1 / diviseur;
            resulta.G1 = resulta.G1 / diviseur;
            resulta.B1 = resulta.B1 / diviseur;
            if (resulta.R1 < 0) resulta.R1 = 0;
            if (resulta.G1 < 0) resulta.G1 = 0;
            if (resulta.B1 < 0) resulta.B1 = 0;
            if (resulta.R1 > 255) resulta.R1 = 255;
            if (resulta.G1 > 255) resulta.G1 = 255;
            if (resulta.B1 > 255) resulta.B1 = 255;
            return resulta;
        }
        public void Filtre(int ChoixDuFiltre)
        {
            int[,] matriceDeConvolution=new int[3,3];
            switch (ChoixDuFiltre)
            {
                case 1:
                    ///Flou - je demande le degre de flou pour le rendre le plu flu possible 
                    Console.Write("Veuillez Choisir le degree de flou desire en % ....");
                    int Reponse = Convert.ToInt32(Console.ReadLine())/10 ;
                    for (int i = 0; i < Reponse; i++)
                    {
                        Flou();

                    }

                    break;
                case 2:
                    ///Renforcement des bords
                     matriceDeConvolution = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
                    break;
                case 3:
                    ///Détection des bords
                    matriceDeConvolution = new int[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
                    break;
                case 4:
                    ///Repoussage 
                    matriceDeConvolution = new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
                    break;

            }
            if (ChoixDuFiltre!=1)
            {
                Pixel[,] matriceIntermaidaire = new Pixel[matriceRGB.GetLength(0), matriceRGB.GetLength(1)];
                for (int i = 0; i < matriceIntermaidaire.GetLength(0); i++)
                {
                    for (int j = 0; j < matriceIntermaidaire.GetLength(1); j++)
                    {
                        matriceIntermaidaire[i, j] = new Pixel(0, 0, 0);
                    }
                }
                ///ici ca faite le tour de la matrice RGB mais n'inculs pas les borne car il ont pas 8 pixel qui les entour 
                for (int i = 1; i < this.matriceRGB.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j < this.matriceRGB.GetLength(1) - 1; j++)
                    {
                        matriceIntermaidaire[i, j] = multiplication(matriceDeConvolution, i, j);
                    }

                }
                this.matriceRGB = matriceIntermaidaire;
            }
           
        }
        private void Flou()
        {
            int[,] matriceDeConvolation = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            Pixel[,] matriceIntermaidaire = matriceRGB;
            ///ici ca faite le tour de la matrice RGB mais n'inculs pas les borne car il ont pas 8 pixel qui les entour 
            for (int i = 1; i < this.matriceRGB.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < this.matriceRGB.GetLength(1) - 1; j++)
                {
                    matriceIntermaidaire[i, j] = multiplication(matriceDeConvolation, i, j);
                }

            }
            this.matriceRGB = matriceIntermaidaire;
        }
        #endregion
        #region Creation de l'image 
        public void Regtancle ()
        {
            int largDiviser = largeur / 11;
            int hautDiviser = hauteur / 11;
            for (int k = 0; k <7; k++)
            {
                for (int i = largDiviser * k; i < this.matriceRGB.GetLength(0)-(largDiviser * k); i++)
                {
                    for (int j = hautDiviser * k; j < this.matriceRGB.GetLength(1)-(hautDiviser * k); j++)
                    {
                        matriceRGB[i, j] = new Pixel(255, (byte)(51*k), 0);
                    }
                }
            }
           
        }
        public void Coeur()
        {
            for (int i = 0; i < matriceRGB.GetLength(0) ; i++)
            {
                for (int j = 0; j < matriceRGB.GetLength(1); j++)
                {
                    matriceRGB[i, j] = new Pixel(255, 104, 204);
                }
            }
            Pixel couleur = new Pixel(255, 255, 255);
            // formation du 1er triangle
            int l = matriceRGB.GetLength(1)-matriceRGB.GetLength(1)/3;
            for (int i = 0; i < matriceRGB.GetLength(0) / 2; i++)
            {
                for (int j =l ; j >= 0; j--)
                {
                    matriceRGB[i,j] = couleur;
                }
                l--;
            }
            l = (matriceRGB.GetLength(1) - matriceRGB.GetLength(1) / 3)+20;
            for (int i = matriceRGB.GetLength(0) / 2; i > matriceRGB.GetLength(0) / 4; i--)
            {
                for (int j = matriceRGB.GetLength(1) - 1; j >= l; j--)
                {
                    matriceRGB[i, j] = couleur;
                }
                l++;
            }
            l = matriceRGB.GetLength(1) - matriceRGB.GetLength(1) / 3;
            for (int i = 0; i < matriceRGB.GetLength(0) / 3; i++)
            {
                for (int j = matriceRGB.GetLength(1) - 1; j >= l; j--)
                {
                    matriceRGB[i, j] = couleur;
                }
                l++;
            }
            // finir la formation des 3 triangel
            Pixel[,] matriceMiroir = new Pixel[this.largeur, this.hauteur];
            for (int i = 0; i < matriceMiroir.GetLength(0); i++)
            {
                for (int j = 0; j < matriceMiroir.GetLength(1); j++)
                {
                    matriceMiroir[i, j] = this.matriceRGB[(this.largeur - 1) - i, j];
                }
            }
            
            for (int i = matriceRGB.GetLength(0) / 2; i < matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < matriceRGB.GetLength(1); j++)
                {
                    matriceRGB[i, j] = matriceMiroir[i, j];
                }
            }
        }
        public void triangle ()
        {
            int k = 1;
            int DoubleK = 0;
            int leSaut = matriceRGB.GetLength(1)/ (matriceRGB.GetLength(0) / 2);
            
            //int leSaut = 1;
            for (int j = 5; j < matriceRGB.GetLength(1); j++)
            {
                for (int i = k; i < matriceRGB.GetLength(0)-k; i++)
                {
                    int couleur = k;
                    if (k >= 255) couleur  = 255;
                    matriceRGB[i, j] = new Pixel(0, (byte)(255- couleur) ,(byte) couleur);
                }
                if (DoubleK % leSaut == 0) k++;
                DoubleK++;
            }
        }
        public void Crecle()
        {
            int Rayon ;
           byte k = 0;
            if (matriceRGB.GetLength(1) > matriceRGB.GetLength(0)) Rayon = matriceRGB.GetLength(0) / 2;
            else Rayon = matriceRGB.GetLength(1) / 2;
            for (int j = 0; j < matriceRGB.GetLength(1); j++)
            {
                for (int i = 0; i < matriceRGB.GetLength(0) ; i++)
                {
                    if (Puissance(i- matriceRGB.GetLength(0) / 2, 2)+ Puissance(j- matriceRGB.GetLength(1) / 2, 2)<=Puissance(Rayon,2))
                    {
                        matriceRGB[i, j] = new Pixel(0, 0, k);
                    }
                    k++;
                }
            }
        }
        public MyImage Histogramme(int choixDeHisto)
        {
            int[] tabHistoRouge = new int[256];
            int[] tabHistoBleu = new int[256];
            int[] tabHistoVert = new int[256];
            int maxRouge = 0;
            int maxBleu = 0;
            int maxVert = 0;
            int maxRGB = 0;
            for (int i = 0; i < matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < matriceRGB.GetLength(1); j++)
                {
                    tabHistoRouge[matriceRGB[i, j].R1]++;
                    tabHistoBleu[matriceRGB[i, j].B1]++;
                    tabHistoVert[matriceRGB[i, j].G1]++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (tabHistoRouge[i] > maxRouge) maxRouge = tabHistoRouge[i];
                if (tabHistoBleu[i] > maxBleu) maxBleu = tabHistoBleu[i];
                if (tabHistoVert[i] > maxVert) maxVert = tabHistoVert[i];
                if (maxRouge > maxRGB) maxRGB = maxRouge;
                if (maxBleu > maxRGB) maxRGB = maxBleu;
                if (maxVert > maxRGB) maxRGB = maxVert;
            }
            MyImage histogrammeRouge = new MyImage(tabHistoRouge.GetLength(0), maxRouge);
            for (int i = 0; i < histogrammeRouge.Largeur; i++)
            {
                for (int j = 0; j < tabHistoRouge[i]; j++)
                {
                    histogrammeRouge.MatriceRGB[i, j] = new Pixel(0, 0, 255);
                }
            }
            MyImage histogrammeBleu = new MyImage(tabHistoBleu.GetLength(0), maxBleu);
            for (int i = 0; i < histogrammeBleu.Largeur; i++)
            {
                for (int j = 0; j < tabHistoBleu[i]; j++)
                {
                    histogrammeBleu.MatriceRGB[i, j] = new Pixel(255, 0, 0);
                }
            }
            MyImage histogrammeVert = new MyImage(tabHistoVert.GetLength(0), maxVert);
            for (int i = 0; i < histogrammeVert.Largeur; i++)
            {
                for (int j = 0; j < tabHistoVert[i]; j++)
                {
                    histogrammeVert.MatriceRGB[i, j] = new Pixel(0, 255, 0);
                }
            }
            ///Afaire 
            MyImage histogrammeRGB = new MyImage((256 * 3), maxRGB);
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < maxRouge; j++)
                {
                    histogrammeRGB.MatriceRGB[i, j] = histogrammeRouge.MatriceRGB[i, j];
                }
                for (int j = 0; j < maxBleu; j++)
                {
                    histogrammeRGB.MatriceRGB[i+255, j] = histogrammeBleu.MatriceRGB[i, j];
                }
                for (int j = 0; j < maxVert; j++)
                {
                    histogrammeRGB.MatriceRGB[i +(2*255), j] = histogrammeVert.MatriceRGB[i, j];
                }
            }
            switch (choixDeHisto)
            {
                case 1:
                    return histogrammeRouge;
                case 2:
                    return histogrammeBleu;
                case 3:
                    return histogrammeVert;
                case 4:
                    return histogrammeRGB;
                default:
                    return histogrammeRGB;
            }
        }
        #endregion
        #region Innovation 1
        /// je veux que les borne alors on prend le dection des borne 
        ///et le mettre en filtre rouge/ bleu/ vert et decale et la remmetre sur l'image reel 
        public void Innov1()
        {
            Pixel[,] detectionDesBorn = FiltreRDB();
            Pixel[,] matDeSortie = new Pixel[Largeur, Hauteur];
           #region RDB Rouge
            for (int i = 0; i < this.matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < this.matriceRGB.GetLength(1); j++)
                {
                    detectionDesBorn[i,j].FiltreRouge();
                }
            }
            matDeSortie=SuperpositionFiltre(matriceRGB, detectionDesBorn);
            #endregion
           #region RDB Bleu
            detectionDesBorn = FiltreRDB();
            for (int i = 0; i < this.matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < this.matriceRGB.GetLength(1); j++)
                {
                    detectionDesBorn[i, j].FiltreBleu();
                }
            }
            Pixel[,] matriceIntermaidaire = new Pixel[detectionDesBorn.GetLength(0), detectionDesBorn.GetLength(1)];
            for (int i = 0; i < matriceIntermaidaire.GetLength(0); i++)
            {
                for (int j = 0; j < matriceIntermaidaire.GetLength(1); j++)
                {
                    if (j >= matriceIntermaidaire.GetLength(1) - 3 || i >= matriceIntermaidaire.GetLength(0) - 3)
                    {
                        matriceIntermaidaire[i, j] = new Pixel(0, 0, 0);
                    }
                    else matriceIntermaidaire[i, j] = detectionDesBorn[i + 3, j + 3];
                }
            }
            detectionDesBorn = matriceIntermaidaire;
            matDeSortie = SuperpositionFiltre(matDeSortie, detectionDesBorn);
            #endregion
           #region RDB Vert
            detectionDesBorn = FiltreRDB();
            for (int i = 0; i < this.matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < this.matriceRGB.GetLength(1); j++)
                {
                    detectionDesBorn[i, j].FiltreVert();
                }
            }
            for (int i = 0; i < matriceIntermaidaire.GetLength(0); i++)
            {
                for (int j = 0; j < matriceIntermaidaire.GetLength(1); j++)
                {
                    if (j <= 2 || i <= 2)
                    {
                        matriceIntermaidaire[i, j] = new Pixel(0, 0, 0);
                    }
                    else matriceIntermaidaire[i, j] = detectionDesBorn[i - 2, j - 2];
                }
            }
            detectionDesBorn = matriceIntermaidaire;
            matDeSortie = SuperpositionFiltre(matDeSortie, detectionDesBorn);
#endregion
            matriceRGB = matDeSortie;
        }
        private Pixel[,] SuperpositionFiltre(Pixel[,] mat1, Pixel[,] mat2)
        {
            Pixel[,] imageDeSortie = new Pixel[Largeur, Hauteur];
            for (int i = 0; i < imageDeSortie.GetLength(0); i++)
            {
                for (int j = 0; j < imageDeSortie.GetLength(1); j++)
                {
                    if (mat2[i, j].R1 < 60 && mat2[i, j].B1 < 60 && mat2[i, j].G1 < 60)
                    {
                        imageDeSortie[i, j] = mat1[i, j];
                    }

                    else imageDeSortie[i, j] = mat2[i, j];

                }
            }
            return imageDeSortie;
        }
        private Pixel[,] FiltreRDB()
        {
           int [,] matriceDeConvolution = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
           Pixel[,] matriceIntermaidaire = new Pixel[matriceRGB.GetLength(0), matriceRGB.GetLength(1)];
                for (int i = 0; i < matriceIntermaidaire.GetLength(0); i++)
                {
                    for (int j = 0; j < matriceIntermaidaire.GetLength(1); j++)
                    {
                        matriceIntermaidaire[i, j] = new Pixel(0, 0, 0);
                    }
                }
                ///ici ca faite le tour de la matrice RGB mais n'inculs pas les borne car il ont pas 8 pixel qui les entour 
                for (int i = 1; i < this.matriceRGB.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j < this.matriceRGB.GetLength(1) - 1; j++)
                    {
                        matriceIntermaidaire[i, j] = multiplication(matriceDeConvolution, i, j);
                    }

                }
                return matriceIntermaidaire;
        }
        #endregion
        #region Innovation 2
        /// moitie l'image , l'autre moitie l'image mirroir et un peut de flou au mileu
        /// on change un peut je vais faire une nouvelle image qui esta le meme hauteur mais largeur =2 x(largeur -largeur/3)
        public void Innov2()
        {
            Pixel[,] matriceMiroir = new Pixel[this.largeur, this.hauteur];
            Pixel [,] MatResulta = new Pixel[2*(this.largeur - (this.largeur/3)), this.hauteur];
            for (int i = 0; i < matriceMiroir.GetLength(0); i++)
            {
                for (int j = 0; j < matriceMiroir.GetLength(1); j++)
                {
                    matriceMiroir[i, j] = this.matriceRGB[(this.largeur - 1) - i, j];
                }
            }
            int milieuLargeur = MatResulta.GetLength(0) / 2;
            //int milieuLargeur = largeur / 2;
            //for (int i = 0; i < milieuLargeur; i++)
            //{
            //    for (int j = 0; j < matriceMiroir.GetLength(1); j++)
            //    {
            //        this.matriceRGB[i, j] = matriceMiroir[i, j];
            //    }
            //}
            for (int i = 0; i < milieuLargeur; i++)
            {
                for (int j = 0; j < MatResulta.GetLength(1); j++)
                {
                    MatResulta[i, j] = matriceMiroir[i, j];
                }
            }
            int index = this.largeur / 3;
            for (int i = milieuLargeur; i < MatResulta.GetLength(0); i++)
            {
                for (int j = 0; j < MatResulta.GetLength(1); j++)
                {
                    MatResulta[i, j] = this.matriceRGB[index, j];
                }
                index++;
            }
            this.matriceRGB = MatResulta;
            this.largeur = MatResulta.GetLength(0);
            this.tailleDuficher = largeur * hauteur * 3 + 54;
            int[,] matriceDeConvolation = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            for (int i = milieuLargeur-20; i < milieuLargeur+20 ; i++)
            {
                for (int j = 1; j < matriceMiroir.GetLength(1)-1; j++)
                {
                    this.matriceRGB[i, j] = multiplication(matriceDeConvolation, i, j);
                }
            }
        }
        #endregion
        #region Innovation 3 
        ///ici je vais faire comme un miroir endesous l'image 
        ///en applique le filtre noir sur l'image de rotation 180 et de mettre MAIS pas tous en desous de limage reel 
        public void Innvo3()
        {
            int[,] matriceDeConvolution = new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
            Pixel[,] matriceIntermaidaire = new Pixel[largeur, hauteur ];
            Pixel[,] matrFinal = new Pixel[largeur, hauteur + (hauteur / 3)];
            int l ;
            for (int i = 0; i < matriceRGB.GetLength(0); i++)
            {
                l = (hauteur / 3);
                for (int j = 0;  j < matriceRGB.GetLength(1);  j++)
                {
                    matrFinal[i, l] = matriceRGB[i, j];
                    l++;
                }
            }
            /// rotation 180 du filtre noir 
            for (int i = 0; i < matriceIntermaidaire.GetLength(0); i++)
            {
                for (int j = 0; j < matriceIntermaidaire.GetLength(1); j++)
                {
                    matriceIntermaidaire[i, j] = this.MatriceRGB[i, (this.hauteur - 1) - j];
                }
            }
            this.matriceRGB = matriceIntermaidaire;
            ///filtre noir sur l'image 
            for (int i = 1; i < this.matriceRGB.GetLength(0) - 1; i++)
             {
              for (int j = 1; j < this.matriceRGB.GetLength(1) - 1; j++)
              {
                        matriceIntermaidaire[i, j] = multiplication(matriceDeConvolution, i, j);
              }
            }
            Flou();
            Flou();
            Flou();
            int k = hauteur - hauteur / 3;
            for (int i = 0; i < matrFinal.GetLength(0); i++)
            {
                for (int j = 0; j < hauteur / 3; j++)
                {
                  matrFinal[i, j] = matriceRGB[i, k];
                    k++;
                }
                k= hauteur- hauteur / 3;
            }
            this.matriceRGB = matrFinal;
            this.hauteur = hauteur + (hauteur / 3);
            tailleDuficher = largeur * hauteur * 3 + 54;
        }
        #endregion
        #region Innovation 4
        /// <summary>
        /// pour le permie temps je retrecie mon image 4 fois dans la matriceRetrecie 
        /// Ensuite je repetre cette matrice retrecie tout au long de une nouvel matrice que j'ai agrendi 
        /// et je agrandi l'image elle meme dans la matriceRGB
        /// a la fin je superpose les 2 matrice celle repeter et celle agrendi pour avoir ma innovation
        /// </summary>
        /// <param name="DegreeAgrendirETrepeter"></param>
        public void Innov4(int DegreeAgrendirETrepeter)
        {
            Pixel[,] matriceRetrecie = new Pixel[this.largeur / 4, this.hauteur / 4];

            for (int i = 0; i < matriceRetrecie.GetLength(0); i++)
            {
                for (int j = 0; j < matriceRetrecie.GetLength(1); j++)
                {

                    int k = (int)((i * this.largeur) / matriceRetrecie.GetLength(0));
                    int l = (int)((j * this.hauteur) / matriceRetrecie.GetLength(1));
                    matriceRetrecie[i, j] = this.matriceRGB[k, l];
                }
            }
            Pixel[,] matriceRepeter = new Pixel[this.largeur * DegreeAgrendirETrepeter, this.hauteur * DegreeAgrendirETrepeter];
            for (int k = 0; k < matriceRepeter.GetLength(0); k+= matriceRetrecie.GetLength(0)) 
            {
                for (int l = 0; l < matriceRepeter.GetLength(1); l += matriceRetrecie.GetLength(1))
                {
                    for (int i = 0; i < matriceRetrecie.GetLength(0); i++)
                    {
                        for (int j = 0; j < matriceRetrecie.GetLength(1); j++)
                        {
                            matriceRepeter[i + k, j + l] = matriceRetrecie[i, j];
                            matriceRepeter[i + k, j + l] = matriceRepeter[i + k, j + l].Superposition(new Pixel (0,0,0));

                        }
                    }
                }
            }
            Pixel[,] matriceAgrandi = new Pixel[this.largeur * DegreeAgrendirETrepeter, this.hauteur * DegreeAgrendirETrepeter];
            for (int i = 0; i < matriceAgrandi.GetLength(0); i++)
            {
                for (int j = 0; j < matriceAgrandi.GetLength(1); j++)
                {
                    int k = (int)((i * this.largeur) / matriceAgrandi.GetLength(0));
                    int l = (int)((j * this.hauteur) / matriceAgrandi.GetLength(1));
                    matriceAgrandi[i, j] = this.matriceRGB[k, l];
                }
            }
            this.hauteur = this.hauteur * DegreeAgrendirETrepeter;
            this.largeur = this.largeur * DegreeAgrendirETrepeter;
            this.tailleDuficher = this.hauteur * this.largeur * 3 + this.tailleDuOffset;
            this.matriceRGB = matriceAgrandi;
            ///Superposition des 2 image celle repeter et celle agrandi 
            for (int i = 0; i < matriceRGB.GetLength(0); i++)
            {
                for (int j = 0; j < matriceRGB.GetLength(1); j++)
                {
                    matriceRGB[i, j]= matriceRGB[i, j].Superposition(matriceRepeter[i, j]);
                }
            }
        }
        #endregion
    }
}
