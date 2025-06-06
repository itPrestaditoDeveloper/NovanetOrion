using System;
using System.IO;
using System.Net.Http;
using System.IO.Compression;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.Controllers;

namespace OrionCoreCableColor.App_Services.FileKMZService
{
    public class FileKMZService
    {
        public string KmlContent { get; private set; } = "";
        public List<string> KmlContentArray { get; private set; } = new List<string>();
        public List<PlacemarkDataViewModel> Placemarks { get; private set; } = new List<PlacemarkDataViewModel>();

        public List<FileInfo> ArchivosKMZ { get; private set; }

        /// <summary>
        /// Descarga y procesa un archivo KMZ desde una URL.
        /// </summary>
        public async Task LoadKMZFromUrl(string fileUrl)
        {
            KmlContent = await DownloadAndExtractKML(fileUrl);
            KmlContentArray.Add(KmlContent);
            if (KmlContentArray.Any())
            {
                Placemarks = ParseKML(KmlContentArray);
            }
            else
            {
                Console.WriteLine("No se pudo extraer el KML.");
            }
        }

        public async Task LoadKMZFromPath(string filePath)
        {

            if (File.Exists(filePath))
            {
                KmlContent = await ExtractKMLFromKMZ(filePath);

                KmlContentArray.Add(KmlContent);
                if (KmlContentArray.Any())
                {
                    Placemarks = ParseKML(KmlContentArray);
                }
                else
                {
                    Console.WriteLine("No se pudo extraer el KML.");
                }
            }

            if (Directory.Exists(filePath))
            {
                var carpetas = GetCarpetas(filePath);

                foreach(var item in carpetas)
                {
                    KmlContentArray.Add(await ExtractKMLFromKMZ(item));
                }
                    Placemarks = ParseKML(KmlContentArray);
            }
            
        }

        /// <summary>
        /// Descarga el archivo KMZ desde una URL y extrae el KML.
        /// </summary>
        private async Task<string> DownloadAndExtractKML(string fileUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] kmzData = await client.GetByteArrayAsync(fileUrl);

                    using (MemoryStream ms = new MemoryStream(kmzData))
                    using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.FullName.EndsWith(".kml", StringComparison.OrdinalIgnoreCase))
                            {
                                using (StreamReader reader = new StreamReader(entry.Open()))
                                {
                                    return await reader.ReadToEndAsync();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al descargar y procesar el KMZ: " + ex.Message);
                }
            }
            return null;
        }


        private async Task<string> ExtractKMLFromKMZ(string filePath)
        {
           
            var kmzPath = filePath;
            using (FileStream fs = new FileStream(kmzPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".kml", StringComparison.OrdinalIgnoreCase))
                    {
                        using (StreamReader reader = new StreamReader(entry.Open()))
                        {
                            return await reader.ReadToEndAsync(); // Lee el KML de forma asíncrona
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Parsea el KML y extrae los nombres y coordenadas de los puntos.
        /// </summary>
        private List<PlacemarkDataViewModel> ParseKML(List<string> ListkmlContent)
        {
            try
            {
                List<PlacemarkDataViewModel> placemarksList = new List<PlacemarkDataViewModel>();
                foreach (var kmlContent in ListkmlContent)
                {


                    XDocument doc = XDocument.Parse(kmlContent);
                    XNamespace ns = "http://www.opengis.net/kml/2.2";

                    var placemarks = doc.Descendants(ns + "Placemark")
                                        .Where(p => p.Element(ns + "Point") != null); // Filtrar solo los que tienen <Point>

                    foreach (var placemark in placemarks)
                    {
                        // Obtener el nombre del punto
                        var nameElement = placemark.Element(ns + "name");
                        string name = nameElement != null ? nameElement.Value.Trim() : "Sin Nombre";

                        // Obtener las coordenadas
                        var coordsElement = placemark.Descendants(ns + "coordinates").FirstOrDefault();
                        double lat = 0, lon = 0;

                        if (coordsElement != null)
                        {
                            string[] coordPairs = coordsElement.Value.Trim().Split(' ');
                            foreach (var pair in coordPairs)
                            {
                                string[] latLonAlt = pair.Split(',');
                                if (latLonAlt.Length >= 2)
                                {
                                    lon = double.Parse(latLonAlt[0]);
                                    lat = double.Parse(latLonAlt[1]);
                                    break; // Solo tomamos la primera coordenada
                                }
                            }
                        }

                        // Agregar a la lista de puntos con Point
                        placemarksList.Add(new PlacemarkDataViewModel
                        {
                            Name = name,
                            Latitude = lat,
                            Longitude = lon
                        });
                    }
                }
                return placemarksList;
            }
            catch (Exception e)
            {

                throw;
            }
           
        }

       
        public List<string> GetCarpetas(string path)
        {
            if (Directory.Exists(path))
            {
                ArchivosKMZ = Directory.GetFiles(path, "*.kmz").Select(file => new FileInfo(file)).ToList();
                string[] archivos = ArchivosKMZ.OrderBy(x=>x.CreationTime).Select(x=>x.FullName).ToArray();
                
                return archivos.ToList();
            }
            else
            {
                return new List<string>();
            }
        }

        public async Task<string> DescargarKMZ(string file)
        {
            return await ExtractKMLFromKMZ(file);
        }

        public List<InfoArchivo> GetInfoArchivos(string path)
        {
            ArchivosKMZ = Directory.GetFiles(path, "*.kmz").Select(file => new FileInfo(file)).ToList();
            return ArchivosKMZ.OrderBy(x => x.CreationTime).Select(x => new InfoArchivo { fcNombreArchivo = x.FullName, fdFechaCreacion = x.CreationTime }).ToList();
        }

    }

    public class InfoArchivo
    {
        public string fcNombreArchivo { get; set; }
        public DateTime fdFechaCreacion { get; set; }
    }


}
