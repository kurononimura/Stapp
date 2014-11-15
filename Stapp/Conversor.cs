using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stapp
{
    public class Conversor
    {
        public double pi = 3.14159265358979;

        /* Ellipsoid model constants (actual values here are for WGS84) */
        public double sm_a = 6378137.0;
        public double sm_b = 6356752.314;
        public double sm_EccSquared = 6.69437999013e-03;
        public double UTMScaleFactor = 0.9996;

        public double DegToRad(double deg)
        {
            return (deg / 180.0 * pi);
        }

        public double RadToDeg(double rad)
        {
            return (rad / pi * 180.0);
        }

        public  double ArcLengthOfMeridian(double phi)
        {
            var n = (sm_a - sm_b) / (sm_a + sm_b);
            var alpha = ((sm_a + sm_b) / 2.0) * (1.0 + (Math.Pow(n, 2.0) / 4.0) + (Math.Pow(n, 4.0) / 64.0));
            var beta = (-3.0 * n / 2.0) + (9.0 * Math.Pow(n, 3.0) / 16.0) + (-3.0 * Math.Pow(n, 5.0) / 32.0);
            var gamma = (15.0 * Math.Pow(n, 2.0) / 16.0) + (-15.0 * Math.Pow(n, 4.0) / 32.0);
            var delta = (-35.0 * Math.Pow(n, 3.0) / 48.0) + (105.0 * Math.Pow(n, 5.0) / 256.0);
            var epsilon = (315.0 * Math.Pow(n, 4.0) / 512.0);
            var result = alpha * (phi + (beta * Math.Sin(2.0 * phi)) + (gamma * Math.Sin(4.0 * phi)) + (delta * Math.Sin(6.0 * phi))
            + (epsilon * Math.Sin(8.0 * phi)));
            return result;
        }

        public double UTMCentralMeridian(double zone)
        {
            var cmeridian = DegToRad(-183.0 + (zone * 6.0));
            return cmeridian;
        }

        public double FootpointLatitude(double y)
        {

            var n = (sm_a - sm_b) / (sm_a + sm_b);
            var alpha_ = ((sm_a + sm_b) / 2.0) * (1 + (Math.Pow(n, 2.0) / 4) + (Math.Pow(n, 4.0) / 64));
            var y_ = y / alpha_;
            var beta_ = (3.0 * n / 2.0) + (-27.0 * Math.Pow(n, 3.0) / 32.0) + (269.0 * Math.Pow(n, 5.0) / 512.0);
            var gamma_ = (21.0 * Math.Pow(n, 2.0) / 16.0) + (-55.0 * Math.Pow(n, 4.0) / 32.0);
            var delta_ = (151.0 * Math.Pow(n, 3.0) / 96.0) + (-417.0 * Math.Pow(n, 5.0) / 128.0);
            var epsilon_ = (1097.0 * Math.Pow(n, 4.0) / 512.0);
            var result = y_ + (beta_ * Math.Sin(2.0 * y_)) + (gamma_ * Math.Sin(4.0 * y_)) + (delta_ * Math.Sin(6.0 * y_))
             + (epsilon_ * Math.Sin(8.0 * y_));

            return result;
        }

        public void MapLatLonToXY(double phi, double lambda, double lambda0, double[] xy)
        {
            var ep2 = (Math.Pow(sm_a, 2.0) - Math.Pow(sm_b, 2.0)) / Math.Pow(sm_b, 2.0);
            var nu2 = ep2 * Math.Pow(Math.Cos(phi), 2.0);
            var N = Math.Pow(sm_a, 2.0) / (sm_b * Math.Sqrt(1 + nu2));
            var t = Math.Tan(phi);
            var t2 = t * t;
            var tmp = (t2 * t2 * t2) - Math.Pow(t, 6.0);
            var l = lambda - lambda0;
            var l3coef = 1.0 - t2 + nu2;
            var l4coef = 5.0 - t2 + 9 * nu2 + 4.0 * (nu2 * nu2);
            var l5coef = 5.0 - 18.0 * t2 + (t2 * t2) + 14.0 * nu2 - 58.0 * t2 * nu2;
            var l6coef = 61.0 - 58.0 * t2 + (t2 * t2) + 270.0 * nu2 - 330.0 * t2 * nu2;
            var l7coef = 61.0 - 479.0 * t2 + 179.0 * (t2 * t2) - (t2 * t2 * t2);
            var l8coef = 1385.0 - 3111.0 * t2 + 543.0 * (t2 * t2) - (t2 * t2 * t2);

            xy[0] = N * Math.Cos(phi) * l
            + (N / 6.0 * Math.Pow(Math.Cos(phi), 3.0) * l3coef * Math.Pow(l, 3.0))
            + (N / 120.0 * Math.Pow(Math.Cos(phi), 5.0) * l5coef * Math.Pow(l, 5.0))
            + (N / 5040.0 * Math.Pow(Math.Cos(phi), 7.0) * l7coef * Math.Pow(l, 7.0));

            xy[1] = ArcLengthOfMeridian(phi)
            + (t / 2.0 * N * Math.Pow(Math.Cos(phi), 2.0) * Math.Pow(l, 2.0))
            + (t / 24.0 * N * Math.Pow(Math.Cos(phi), 4.0) * l4coef * Math.Pow(l, 4.0))
            + (t / 720.0 * N * Math.Pow(Math.Cos(phi), 6.0) * l6coef * Math.Pow(l, 6.0))
            + (t / 40320.0 * N * Math.Pow(Math.Cos(phi), 8.0) * l8coef * Math.Pow(l, 8.0));

            return;
        }



        //public void MapXYToLatLon (double x,double  y,double lambda0, double [] philambda)
        //{
        //var phif = FootpointLatitude (y);
        //var ep2 = (Math.Pow(sm_a, 2.0) - Math.Pow(sm_b, 2.0))/ Math.Pow (sm_b, 2.0);
        //var cf = Math.Cos(phif);
        //var nuf2 = ep2 * Math.Pow(cf, 2.0);
        //var Nf = Math.Pow(sm_a, 2.0) / (sm_b * Math.Sqrt(1 + nuf2));
        //var Nfpow = Nf;
        //var tf = Math.Tan(phif);
        //var tf2 = tf * tf;
        //var tf4 = tf2 * tf2;
        //var x1frac = 1.0 / (Nfpow * cf);
        //Nfpow *= Nf;  
        //var x2frac = tf / (2.0 * Nfpow);
        //Nfpow *= Nf;  
        //var x3frac = 1.0 / (6.0 * Nfpow * cf);
        //Nfpow *= Nf;   
        //var x4frac = tf / (24.0 * Nfpow);
        //Nfpow *= Nf;   
        //var x5frac = 1.0 / (120.0 * Nfpow * cf);
        //Nfpow *= Nf;  
        //var x6frac = tf / (720.0 * Nfpow);
        //Nfpow *= Nf;   
        //var x7frac = 1.0 / (5040.0 * Nfpow * cf);
        //Nfpow *= Nf;   
        //var x8frac = tf / (40320.0 * Nfpow);
        //var x2poly = -1.0 - nuf2;
        //var x3poly = -1.0 - 2 * tf2 - nuf2;
        //var x4poly = 5.0 + 3.0 * tf2 + 6.0 * nuf2 - 6.0 * tf2 * nuf2
        //    - 3.0 * (nuf2 *nuf2) - 9.0 * tf2 * (nuf2 * nuf2);
        //var x5poly = 5.0 + 28.0 * tf2 + 24.0 * tf4 + 6.0 * nuf2 + 8.0 * tf2 * nuf2;
        //var x6poly = -61.0 - 90.0 * tf2 - 45.0 * tf4 - 107.0 * nuf2
        //    + 162.0 * tf2 * nuf2;
        //var x7poly = -61.0 - 662.0 * tf2 - 1320.0 * tf4 - 720.0 * (tf4 * tf2);
        //var x8poly = 1385.0 + 3633.0 * tf2 + 4095.0 * tf4 + 1575 * (tf4 * tf2);
        	
        ///* Calculate latitude */
        //philambda[0] = phif + x2frac * x2poly * (x * x)
        //    + x4frac * x4poly * Math.Pow (x, 4.0)
        //    + x6frac * x6poly * Math.Pow (x, 6.0)
        //    + x8frac * x8poly * Math.Pow (x, 8.0);
        	
        ///* Calculate longitude */
        //philambda[1] = lambda0 + x1frac * x
        //    + x3frac * x3poly * Math.Pow(x, 3.0)
        //    + x5frac * x5poly * Math.Pow(x, 5.0)
        //    + x7frac * x7poly * Math.Pow(x, 7.0);
        	
        //return;
        //}
        public double LatLonToUTMXY(double lat, double lon, double zone, double[] xy)
        {
            MapLatLonToXY(lat, lon, UTMCentralMeridian(zone), xy);
            xy[0] = xy[0] * UTMScaleFactor + 500000.0;
            xy[1] = xy[1] * UTMScaleFactor;
            if (xy[1] < 0.0)
            {
                xy[1] = xy[1] + 10000000.0;
            }
            return zone;

        }
        //public double UTMXYToLatLon (double x,double  y,double zone,bool southhemi,double[]  latlon)
        //{
        //x -= 500000.0;
        //x /= UTMScaleFactor;
        	
        ///* If in southern hemisphere, adjust y accordingly. */
        //if (southhemi)
        //y -= 10000000.0;
        		
        //y /= UTMScaleFactor;
        
        //var cmeridian = UTMCentralMeridian (zone);
        //MapXYToLatLon (x, y, cmeridian, latlon);
        	
        //return zone;
        //}
    }
}
