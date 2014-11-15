using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stapp
{
    public class numeros
    {
        
        public string conversornum (string numero)
        {
            string[] letras = new string[] {"A","B","C","D","E","F","G","H","I","J" };
           string[] unidades = new string[]{"cero", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve" };
           int[] unidades2 = new int[]{0,1,2,3,4,5,6,7,8,9};
           string[] diez = new string[]{ "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho","diecinueve" };
           int[] diez2 = new int[]{10,11,12,13,14,15,16,17,18,19};
           string[] decenas = new string[] { "veinti","veinte","treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa"};
           int[] decenas2 = new int[] { 20,20,30,40,50,60,70,80,90 };
           string[] centenas = new string[]{ "Ciento", "Doscientos", "Trescientos", "Cuatrocientos", "Quinientos", "Seiscientos", "Setecientos","Ochocientos","Novecientos" };
           int[] centenas2 = new int[] { 100,200,300,400,500,600,700,800,900};
           bool s=true;
           int num = 0,num3=0,num4=0;
           string num2 = "";
           string[] numero2 = new string[4];
           numero2 = numero.Split(' ');
           try
           {
               if (numero2.Length == 1)
               {
                   num2 = numero;
                   
                   s = false;

               }
               if (numero2.Length == 2)
               {
                   for (int i = 0; i < unidades.Length; i++)
                   {
                       s = numero2[0].Contains(Convert.ToString(unidades2[i]));
                       if (s == true)
                       {
                           num2 = numero;
                           s = false;
                       }

                       if(s==true)
                       {
                           s = numero2[0].Contains(letras[i]);
                           if (s == true)
                           {
                               s = numero2[1].Contains(Convert.ToString(unidades2[i]));
                               if (s == true)
                               {
                                   num3 = unidades2[i];
                               }

                               s = numero2[1].Contains(unidades[i]);
                               if (s == true)
                               {
                                   num3 = unidades2[i];
                               }

                           }
                       }

                   }
                   if (s==true)
                   {
                       for (int i = 1; i < unidades.Length; i++)
                       {

                           s = numero2[1].Contains(Convert.ToString(unidades2[i]));
                           if (s == true)
                           {
                               num4 = unidades2[i];
                           }

                           s = numero2[1].Contains(unidades[i]);
                           if (s == true)
                           {
                               num4 = unidades2[i];
                           }

                           num2 = numero2[0] + num3 + num4;
                           s = false;
                       }
                   }



               }
               if (s == true)
               {
                   for (int i = 0; i < decenas.Length; i++)
                   {
                       s = numero2[0].Contains(centenas[i]);
                       if (s == true)
                       {
                           num = centenas2[i];
                       }
                   }

                   for (int i = 0; i < decenas.Length; i++)
                   {
                       s = numero2[1].Contains(decenas[i]);
                       if (s == true)
                       {
                           num = num + decenas2[i];
                       }
                   }
                   for (int i = 0; i < diez.Length; i++)
                   {
                       s = numero2[1].Contains(diez[i]);
                       if (s == true)
                       {
                           num = num + diez2[i];
                       }
                   }
                   for (int i = 0; i < unidades.Length; i++)
                   {
                       s = numero2[1].Contains(unidades[i]);
                       if (s == true)
                       {
                           num = num + unidades2[i];
                       }
                       if (numero2.Length == 4)
                       {
                           s = numero2[3].Contains(unidades[i]);
                           if (s == true)
                           {
                               num = num + unidades2[i];
                           }
                       }
                   }
                   num2 = Convert.ToString(num);
                   if (numero2.Length == 3)
                   {
                       num2 = num2 + numero2[2];
                   }
               }


           }
            catch
           {
               throw new Exception("error");
           }


           return num2;
        }
    }
}
