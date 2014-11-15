using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Stapp.Resources;

using Windows.Phone.Speech.Synthesis;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.VoiceCommands;

using System.Xml.Linq;

using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using Microsoft.Phone.Tasks;
using System.Device;
using Microsoft.Phone.Net.NetworkInformation;
using System.IO.IsolatedStorage;


namespace Stapp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public SpeechSynthesizer speechSynth = new SpeechSynthesizer();
        GeoCoordinateWatcher ubicacion; // variable para el mapa
        MapOverlay mipos;
        double lon;
        double lat;
        string microbuscada;
        Conversor obj = new Conversor();
        numeros obj2 = new numeros();
        public double[] xy = new double[2];
        string temp;
        public Persona[] people = new Persona[0];
        double y1, y2, x1, x2;
        bool IsConnected = false;
        IsolatedStorageSettings isolated = IsolatedStorageSettings.ApplicationSettings;


        public void isolated2()
        {
            isolated.Add("miposx", 0);
            isolated.Add("miposy", 0);
            isolated.Add("coorxpa", 0);
            isolated.Add("coorypa", 0);
            isolated.Add("pos", 0);
        }
        //public void guardar()
        //{
        //    try
        //    {

        //        //if (people != null)
        //        //{ 
        //            int pos = Convert.ToInt32(isolated["pos"]);
        //            string[] yab = people[pos].Cory.Split('.');
        //            string[] xab = people[pos].Corx.Split('.');
        //            int ya = Convert.ToInt32(xy[1]); int yb = Convert.ToInt32(yab[0]);
        //            int xa = Convert.ToInt32(xy[0]); int xb = Convert.ToInt32(xab[0]);
        //            isolated["miposx"] = xa;
        //            isolated["miposy"] = ya;
        //            isolated["coorxpa"] = xb;
        //            isolated["coorypa"] = yb;
        //        //}
        //        //else
        //        //{
        //           // isolated["miposx"] = 0;
        //          //  isolated["miposy"] = 0;
        //            //isolated["coorxpa"] = 0;
        //           // isolated["coorypa"] = 0;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //       // MessageBox.Show("error isolated: " + ex.Message+ex.StackTrace);
        //        error2();
        //    }
           

        //    //isolated["coodenada"] = "nuevo valor";

        //    //var coor = isolated["coordenada"];
        //}

        double zone;
        public void conversor()
        {
            zone = Math.Floor((lon + 180.0) / 6) + 1;
            double zone2 = obj.LatLonToUTMXY(obj.DegToRad(lat), obj.DegToRad(lon), zone, xy);
        }

        public class Persona
        {
            public string Servicio { get; set; }
            public string NumParadero { get; set; }
            public string Comuna { get; set; }
            public string Eje { get; set; }
            public string Desde { get; set; }
            public string Hacia { get; set; }
            public string Corx { get; set; }
            public string Cory { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
           
           
            try
            {
               

                checkNetworkConnection();

                if (IsConnected == false)
                {
                    error3();
                }


                leer();
                


                ubicacion = new GeoCoordinateWatcher(GeoPositionAccuracy.High);// Precicion
                ubicacion.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(ubicacion_lugar);//Pocicion actual
                ubicacion.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(ubicacion_estado);//Verificacion de permisos
                ubicacion.MovementThreshold = 1; //distancia minima para la actualizacion ddee ubicacion
                ubicacion.Start();

                conversor();
                if(isolated.Count == 0)
                {
                    isolated2();
                }
                
            }
            catch (Exception ex)
            {
               // MessageBox.Show("error buscar paraderos: " + ex.Message + ex.StackTrace);
                

                error2();
            }

            

        }
        public async void leer()
        {
            try
            {
                var synth = InstalledVoices.All.Where(iv => iv.Language == "es-ES").First();
                if (synth == InstalledVoices.Default)
                {
                    await speechSynth.SpeakTextAsync("Error: you don't have spanish lenguaje, please install... ");
                }
                else
                {
                    speechSynth.SetVoice(synth);
                }
                //await speechSynth.SpeakTextAsync("Para iniciar la aplicación presione en el centro de su pantalla.  Si desea saber su posición presione en la esquina superior derecha.");
            }
            catch
            {
                error2();
            }
        }
        public async void error4()
        {
            await speechSynth.SpeakTextAsync("Error: you don't have spanish lenguaje, please install... ");
        }
        public async void error3()
        {
            await speechSynth.SpeakTextAsync("Conexion a internet no disponible o desconectada. La funcionalidad completa de la aplicación no está disponible");
        }
        //-------------------------metodos del mapa
        private void myMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "584ed013-6dee-4e46-8f3e-67fcc659b637";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "DnLWAXThnskI55OLWZdb1A";
        }
        public void ubicacion_lugar(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            
            mipos = new MapOverlay();
            mipos.GeoCoordinate = e.Position.Location;  //Se le inserta la Longitud y Latitud
            //  mipos.Content = "Aqui estoy yo";       //contenido del pushping
            //  MapaInacap.Children.Add(mipos);        //Insercion del Pushping al mapa
            //  MapaInacap.SetView(mipos.Location, 17);//Cargar mapa con esta pocicion
            lon = mipos.GeoCoordinate.Longitude;
            lat = mipos.GeoCoordinate.Latitude;
            //MessageBox.Show(lon+","+lat);
           
            
        }
        public async void query_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            try
            {
                if (e.Error == null)
                {
                    var a = e.Result;
                    string comuna = a[0].Information.Address.City;
                    string calle = a[0].Information.Address.Street;
                    string numero = a[0].Information.Address.HouseNumber;
                    await speechSynth.SpeakTextAsync("Usted se encuentra en la comuna de " + comuna + ". En la calle: " + calle + ", Número aproximado : " + numero);

                }
                else
                {
                    await speechSynth.SpeakTextAsync("No se ha encontrado su ubicación actual");

                }
            }
            catch(System.Threading.Tasks.TaskCanceledException)
            {

            }
            catch 
            {
                error3();
            }
           

        } 
      
        public async void ubicacion_estado(object sender, GeoPositionStatusChangedEventArgs e)
        {
            try
            {
                switch (e.Status)
                {
                    case GeoPositionStatus.Disabled:
                        if (ubicacion.Permission == GeoPositionPermission.Denied)
                        {
                            await speechSynth.SpeakTextAsync("Debes activar tu ubicacion en : Configuración, ubicación, activar ubicación. Error De Localizacion");

                        }
                        else
                        {
                            await speechSynth.SpeakTextAsync("Servicio de localizacion sin funcionamiento");

                        }
                        break;
                    case GeoPositionStatus.Initializing:
                        break;
                    case GeoPositionStatus.NoData:
                        await speechSynth.SpeakTextAsync("Datos no disponibles.");
                        break;
                    case GeoPositionStatus.Ready:
                        break;
                }
            }
            catch
            {
                error2();
            }
            
        }
        //-------------------------- metodos del sonido
       
        public async void error2()
        {
            await speechSynth.SpeakTextAsync("Ha ocurrido un error...");
        }



        public async void buscar_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                speechSynth.CancelAll();
                SpeechRecognizerUI speechRecognizer = new SpeechRecognizerUI();

                var spanishRecognizer = InstalledSpeechRecognizers.All.Where(sr => sr.Language == "es-ES").First();
                if (speechRecognizer != null)
                    speechRecognizer.Recognizer.SetRecognizer(spanishRecognizer);
                else
                    MessageBox.Show("Recognizer with the language not found");


                speechRecognizer.Recognizer.Settings.InitialSilenceTimeout = TimeSpan.FromSeconds(4.0);
                speechRecognizer.Recognizer.Settings.BabbleTimeout = TimeSpan.FromSeconds(3.0);
                speechRecognizer.Settings.ExampleText = "308,201e,120... etc";
                speechRecognizer.Settings.ListenText = "Diga el bus que desea buscar";

                await speechSynth.SpeakTextAsync("Después del tono, indique el recorrido que desea buscar");


                var result = await speechRecognizer.RecognizeWithUIAsync();


                if (result.RecognitionResult != null)
                {
                    string result2 = Convert.ToString(result.RecognitionResult.Text);
                    temp = result2.Replace(".", string.Empty);
                    string num2 = obj2.conversornum(temp);
                    //microbuscada = result.RecognitionResult.Text;
                    microbuscada = Convert.ToString(num2);

                    temp = microbuscada.Replace(" ", string.Empty).Replace(".", string.Empty);
                    conversor();

                    y1 = xy[1] - 500;
                    y2 = xy[1] + 500;
                    x1 = xy[0] - 500;
                    x2 = xy[0] + 500;

                    await speechSynth.SpeakTextAsync("Por favor espere mientras se envía la solicitud");


                    XDocument objXML = XDocument.Load("Data.xml");
                    this.people = (
                    from obj in objXML.Descendants("Paradero")
                    where obj.Element("Servicio").Value == temp && (float)obj.Element("Cory") >= y1 && (float)obj.Element("Cory") <= y2 && (float)obj.Element("Corx") >= x1 && (float)obj.Element("Corx") <= x2
                    select new Persona
                    {
                        Servicio = obj.Element("Servicio").Value,
                        NumParadero = obj.Element("NumParadero").Value,
                        Comuna = obj.Element("Comuna").Value,
                        Eje = obj.Element("Eje").Value,
                        Desde = obj.Element("Desde").Value,
                        Hacia = obj.Element("Hacia").Value,
                        Corx = obj.Element("Corx").Value,
                        Cory = obj.Element("Cory").Value

                    }).ToArray();

                    if (people.Length == 0)
                    {
                        await speechSynth.SpeakTextAsync("No se han encontrado paraderos cercanos a su posición. si desea volver a buscar, presione de nuevo en su pantalla");
                        isolated["coorypa"] = 0;
                        isolated["coorxpa"] = 0;
                        if (people == null)
                            MessageBox.Show("hola");
                       
                    }
                    else
                    {
                        await speechSynth.SpeakTextAsync("Se han encontrado " + people.Length + " Paraderos cercanos a su posición. si desea buscar el más cercano presione en la esquina inferior izquierda de su pantalla.");

                    }
                }
                else
                {
                    await speechSynth.SpeakTextAsync(" Ahora puede volver a buscar apretando en su pantalla ");

                }
            }
            catch (System.Threading.Tasks.TaskCanceledException) { }
            catch
            {
                error2();
            }
           


        }

        private void mipos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                speechSynth.CancelAll();
                ReverseGeocodeQuery query = new ReverseGeocodeQuery()

                {

                    GeoCoordinate = new GeoCoordinate(lat, lon)

                };


                query.QueryCompleted += query_QueryCompleted;

                query.QueryAsync();
            }
            catch
            {
                error2();
            }
           
        }

        public async void ayuda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                speechSynth.CancelAll();

                await speechSynth.SpeakTextAsync("Si necesita asistencia de Transantiago, por favor llame al 800 730 073, opción 1, luego opción 0");
            }
            catch (System.Threading.Tasks.TaskCanceledException) { }
            catch
            {
                error2();
            }
        }
        public int pos = 0;
        public async void cambiar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                speechSynth.CancelAll();

                //int [][]x=new int[][];
               
                if (people.Length != 0)
                {
                    int cont = 0;
                    int cant = people.Length;
                    double[] auxy = new double[cant];
                    double[] auxx = new double[cant];
                    int[,] auxsum = new int[2, cant];
                    //float[] xy2 = new float[1];
                    //xy2[0] = Convert.ToSingle(xy[0]);
                    //xy2[1] = Convert.ToSingle(xy[1]);
                    double distance =0;
                    for (int i = 0; i < people.Length; i++)
                    {
                        string[] xy2 = people[i].Cory.Split('.');
                        string[] xy3 = people[i].Corx.Split('.');
                        int x = Convert.ToInt32(xy3[0]);
                        int y = Convert.ToInt32(xy2[0]);
                        int y2 = Convert.ToInt32(xy[1]);
                        int x2 = Convert.ToInt32(xy[0]);

                        if (x2 < x)
                        {
                            auxx[i] = x - x2;
                        }
                        else
                        {
                            auxx[i] = x2 - x;
                        }

                        if (y2 < y)
                        {
                            auxy[i] = y - y2;
                        }
                        else
                        {
                            auxy[i] = y2 - y;
                        }
                        cont++;
                        auxsum[0, i] = Convert.ToInt32(auxy[i] + auxx[i]);
                        auxsum[1, i] = cont;

                        //MessageBox.Show("valor: " + auxsum[0, i] + " pos: " + auxsum[1, i]);
                        //MessageBox.Show("corx: " + people[i].Corx + "cory :" + people[i].Cory +"x: "+ xy[0]+"y: "+ xy[1] );

                      
                    }
                    int aux;
                    for (int i = 0; i < auxsum.GetLength(1); i++)
                    {
                        for (int j = i + 1; j < auxsum.GetLength(1); j++)
                        {
                            if (auxsum[0, i] > auxsum[0, j])
                            {
                                aux = auxsum[0, i];
                                auxsum[0, i] = auxsum[0, j];
                                auxsum[0, j] = aux;

                                aux = auxsum[1, i];
                                auxsum[1, i] = auxsum[1, j];
                                auxsum[1, j] = aux;
                            }
                        }
                    }
                    //for (int i = 0; i < auxsum.GetLength(1); i++)
                    //{
                    //    MessageBox.Show("valor: " + auxsum[0, i] + " pos: " + auxsum[1, i]);
                    //}
                    pos = auxsum[1, 0] - 1;
                    conversor();
                    isolated["pos"] = pos;
                    //MessageBox.Show("cor x: " + people[pos].Corx+ "cor y: " + 
                    //people[pos].Cory + "\ncomuna: " + people[pos].Comuna + "\ndesde: " + 
                    //people[pos].Desde + "hacia: " + people[pos].Hacia);
                    string[] yab = people[pos].Cory.Split('.');
                    string[] xab = people[pos].Corx.Split('.');
                    int ya = Convert.ToInt32(xy[1]);int yb =Convert.ToInt32(yab[0]);
                    int xa = Convert.ToInt32(xy[0]); int xb = Convert.ToInt32(xab[0]);
                    int a = xa - xb;
                    int b = ya - yb;
                    distance = Math.Sqrt(a * a + b * b);
                  

                   // await speechSynth.SpeakTextAsync("ud se encuentra a :" +  + "metros del paradero");
                    await speechSynth.SpeakTextAsync("El paradero más cercano a su ubicación es el: " +
                     people[pos].NumParadero + ". se encuentra en la calle " + people[pos].Eje + ". entre las calles" + people[pos].Desde + ", y  " + people[pos].Hacia
                     + ". Esta, a:" + Convert.ToInt32(distance) + " metros aproximadamente . Recuerde que este paradero puede ir de ida o regreso. Si necesita asistencia, presione en la esquina inferior derecha");

                    isolated["coorypa"] = yb;
                    isolated["coorxpa"] = xb;
                }
                else
                {
                    await speechSynth.SpeakTextAsync("No se han buscado paraderos...");
                    isolated["coorypa"] = 0;
                    isolated["corrxpa"] = 0;
                   
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException) { }
            catch(Exception ex)
            {
              // MessageBox.Show("error buscar paraderos: "+ex.Message+ex.StackTrace);
                error2();
            }

        }


        public bool checkNetworkConnection()
        {
            var ni = NetworkInterface.NetworkInterfaceType;


            if ((ni == NetworkInterfaceType.Wireless80211) || (ni == NetworkInterfaceType.MobileBroadbandCdma) || (ni == NetworkInterfaceType.MobileBroadbandGsm))
            {
                IsConnected = true;
            }
            else if (ni == NetworkInterfaceType.None)
            {
                IsConnected = false;
            }
            return IsConnected;
        }

        private async void distancias_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                speechSynth.CancelAll();
                if (Convert.ToInt32(isolated["coorypa"]) != 0)
                {
                    //ubicacion.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(ubicacion_lugar);
                    conversor();
                    isolated["miposy"] = xy[1];
                    isolated["miposx"] = xy[0];
                    int yb = Convert.ToInt32(isolated["coorypa"]); 
                    int ya = Convert.ToInt32(isolated["miposy"]); 
                    int xa = Convert.ToInt32(isolated["miposx"]); 
                    int xb = Convert.ToInt32(isolated["coorxpa"]);
                    int a = xa - xb;
                    int b = ya - yb;
                    double distance = Math.Sqrt(a * a + b * b);
                    //MessageBox.Show("distancia: " + distance);
                   // MessageBox.Show("mi pos: " + isolated["miposy"] + " " + isolated["miposx"]);
                   // MessageBox.Show("paradero: " + isolated["coorypa"] + " " + isolated["coorxpa"]);
                    await speechSynth.SpeakTextAsync("Para llegar al paradero faltan" + Convert.ToInt32(distance) + " metros aproximadamente");
                }
                else
                {
                    await speechSynth.SpeakTextAsync("No se puede calcular la distancia...");
                }

            }
            catch (System.Threading.Tasks.TaskCanceledException) { }
            catch(Exception ex)
            {
               //MessageBox.Show("error:"+ex.Message+ex.Source+ex.StackTrace);
                error2();

            }
        }
    
        // Código de ejemplo para compilar una ApplicationBar traducida
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Establecer ApplicationBar de la página en una nueva instancia de ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Crear un nuevo botón y establecer el valor de texto en la cadena traducida de AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Crear un nuevo elemento de menú con la cadena traducida de AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}