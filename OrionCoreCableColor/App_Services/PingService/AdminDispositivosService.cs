using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.PingService;
using OrionCoreCableColor.Controllers;
using OrionCoreCableColor.Models.Base;

public class AdminDispositivosService
{
    private readonly string filePath = Path.Combine(MemoryLoadManager.URL, @"Documento\Recursos\Dispositivos.json");
    private static readonly object fileLock = new object();
    private static JArray dispositivosEnMemoria;

    public AdminDispositivosService()
    {
        lock (fileLock)
        {
            if (dispositivosEnMemoria == null)
            {
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "[]");
                }

                dispositivosEnMemoria = LeerDispositivosDesdeArchivo();
            }
        }
    }

    private JArray LeerDispositivosDesdeArchivo()
    {
        var contenido = File.ReadAllText(filePath);
        return JArray.Parse(contenido);
    }

    public JArray LeerDispositivos()
    {
        lock (fileLock)
        {
            return new JArray(dispositivosEnMemoria); // Retorna una copia en memoria
        }
    }


    public List<DispositivosConLogViewModel> LeerDispostivosObjeto()
    {
        return new JArray(dispositivosEnMemoria).ToObject<List<DispositivosConLogViewModel>>();
    }



    public void GuardarDispositivos(JArray dispositivos)
    {
        lock (fileLock)
        {
            dispositivosEnMemoria = dispositivos;
            File.WriteAllText(filePath, dispositivos.ToString(Formatting.Indented));
        }
    }

    public void AgregarOEditarDispositivo(string fcTipoDispositivo, string propiedadUnica, string valor, JObject dispositivoActualizado)
    {
        try
        {
            lock (fileLock)
            {
                bool encontrado = false;

                // Buscar dispositivo por la propiedad única
                foreach (var dispositivo in dispositivosEnMemoria)
                {
                    if (dispositivo[propiedadUnica]?.ToString() == valor && dispositivo["fcTipoDispositivo"]?.ToString() == fcTipoDispositivo)
                    {
                        bool hayCambios = false;

                        if ((bool)dispositivo["fbConectado"] != (bool)dispositivoActualizado["fbConectado"])
                        {
                            var logArray = dispositivo["Log"] as JArray ?? new JArray();
                            logArray.Add(JObject.FromObject(new DispositivoLogViewModel
                            {
                                fbEstado = (bool)dispositivoActualizado["fbConectado"],
                                fdFechaAccion = DateTime.Now
                            }));
                            dispositivo["Log"] = logArray;
                            hayCambios = true;
                        }

                        
                        foreach (var propiedad in dispositivoActualizado)
                        {
                            if (!JToken.DeepEquals(dispositivo[propiedad.Key], propiedad.Value))
                            {
                                dispositivo[propiedad.Key] = propiedad.Value;
                                hayCambios = true;
                            }
                        }
                        
                        // Verificar cambios en las propiedades
                        

                        // Registrar log solo si el estado de conexión cambia
                        

                        // Salir si no hay cambios
                        if (!hayCambios)
                        {
                            Console.WriteLine("No se detectaron cambios en el dispositivo.");
                            return;
                        }

                        encontrado = true;
                        break;
                    }
                }

                if (!encontrado)
                {
                    // Si no se encuentra, agregar nuevo dispositivo
                    dispositivoActualizado["Log"] = new JArray { JObject.FromObject(new DispositivoLogViewModel
                    {
                        fbEstado = (bool)dispositivoActualizado["fbConectado"],
                        fdFechaAccion = DateTime.Now
                    }) };
                    dispositivosEnMemoria.Add(dispositivoActualizado);
                    Console.WriteLine("Dispositivo agregado exitosamente.");
                }
                else
                {
                    Console.WriteLine("Dispositivo actualizado exitosamente.");
                }

                // Guardar cambios
                File.WriteAllText(filePath, dispositivosEnMemoria.ToString(Formatting.Indented));
            }
        }
        catch (Exception e)
        {
            // Loguear el error (puedes personalizar esto según tu sistema de logging)
            Console.WriteLine($"Error: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }


    /*public void AgregarOEditarDispositivo(string fcTipoDispositivo, string propiedadUnica, string valor, JObject dispositivoActualizado)
    {
        try
        {
            lock (fileLock) // Bloqueo para thread-safety
            {
                bool encontrado = false;
                JToken dispositivoAEditar = null;
                int index = -1;

                // Buscar el dispositivo existente
                for (int i = 0; i < dispositivosEnMemoria.Count; i++)
                {
                    var dispositivo = dispositivosEnMemoria[i];
                    if (dispositivo[propiedadUnica]?.ToString() == valor &&
                        dispositivo["fcTipoDispositivo"]?.ToString() == fcTipoDispositivo)
                    {
                        dispositivoAEditar = dispositivo;
                        index = i;
                        encontrado = true;
                        break;
                    }
                }

                if (encontrado)
                {
                    bool hayCambios = false;

                    // Verificar si cambió el estado de conexión
                    if ((bool)dispositivoAEditar["fbConectado"] != (bool)dispositivoActualizado["fbConectado"])
                    {
                        var logArray = dispositivoAEditar["Log"] as JArray ?? new JArray();
                        logArray.Add(JObject.FromObject(new DispositivoLogViewModel
                        {
                            fbEstado = (bool)dispositivoActualizado["fbConectado"],
                            fdFechaAccion = DateTime.Now
                        }));
                        dispositivoAEditar["Log"] = logArray;
                        hayCambios = true;
                    }

                    // Actualizar propiedades solo si son diferentes
                    foreach (var propiedad in dispositivoActualizado.Properties())
                    {
                        if (!JToken.DeepEquals(dispositivoAEditar[propiedad.Name], propiedad.Value))
                        {
                            dispositivoAEditar[propiedad.Name] = propiedad.Value;
                            hayCambios = true;
                        }
                    }

                    if (!hayCambios)
                    {
                        Console.WriteLine("No se detectaron cambios en el dispositivo.");
                        return;
                    }

                    dispositivosEnMemoria[index] = dispositivoAEditar;
                    Console.WriteLine("Dispositivo actualizado exitosamente.");
                }
                else
                {
                    dispositivoActualizado["Log"] = new JArray
                {
                    JObject.FromObject(new DispositivoLogViewModel
                    {
                        fbEstado = (bool)dispositivoActualizado["fbConectado"],
                        fdFechaAccion = DateTime.Now
                    })
                };
                    dispositivosEnMemoria.Add(dispositivoActualizado);
                    Console.WriteLine("Dispositivo agregado exitosamente.");
                }

                // Guardar los cambios de forma segura
                GuardarCambiosSeguro(filePath, dispositivosEnMemoria);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }


    private void GuardarCambiosSeguro(string filePath, JArray datos)
    {
        string tempFilePath = filePath + ".tmp"; // Archivo temporal
        string backupFilePath = filePath + ".bak"; // Respaldo

        try
        {
            // 1. Hacer un respaldo del archivo original si existe
            if (File.Exists(filePath))
            {
                File.Copy(filePath, backupFilePath, true); // Sobrescribe el respaldo anterior
            }

            // 2. Escribir los datos en un archivo temporal
            string jsonContent = datos.ToString(Formatting.Indented);
            File.WriteAllText(tempFilePath, jsonContent);

            // 3. Validar que el archivo temporal sea válido
            if (File.Exists(tempFilePath) && new FileInfo(tempFilePath).Length > 0)
            {
                string tempContent = File.ReadAllText(tempFilePath);
                JArray.Parse(tempContent); // Verifica que sea un JSON válido
            }
            else
            {
                throw new IOException("El archivo temporal está vacío o no se creó correctamente.");
            }

            // 4. Reemplazar el archivo original con el temporal
            File.Move(tempFilePath, filePath); // Sobrescribe el original

            // 5. Eliminar el respaldo si todo salió bien
            if (File.Exists(backupFilePath))
            {
                File.Delete(backupFilePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar el archivo: {ex.Message}");

            // Restaurar el respaldo si algo falla
            if (File.Exists(backupFilePath) && File.Exists(tempFilePath))
            {
                try
                {
                    File.Move(backupFilePath, filePath); // Restaurar el original
                    Console.WriteLine("Archivo restaurado desde el respaldo.");
                }
                catch (Exception restoreEx)
                {
                    Console.WriteLine($"Error al restaurar el respaldo: {restoreEx.Message}");
                }
            }

            // Limpiar el archivo temporal si quedó
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            throw; // Relanzar la excepción para notificar al llamador
        }
    }*/

    public List<IntervaloConexionViewModel> GetIntervaloConexionDispositivo(string fcTipoDispositivo, string propiedadUnica, string valor)
    {
        try
        {
            var listaTiempos = new List<IntervaloConexionViewModel>();
            JToken dispositivoEncontrado = null;
            bool encontrado = false;
            DateTime ahora = DateTime.Now;

            lock (fileLock)
            {
                foreach (var dispositivo in dispositivosEnMemoria)
                {
                    if (dispositivo[propiedadUnica]?.ToString() == valor && dispositivo["fcTipoDispositivo"]?.ToString() == fcTipoDispositivo)
                    {
                        dispositivoEncontrado = dispositivo;
                        encontrado = true;
                        break;
                    }
                }
            }

            if (encontrado)
            {
                JArray logs = (JArray)dispositivoEncontrado["Log"];

                DateTime? inicio = null;
                bool estadoAnterior = false;
                int i = 1;

                foreach (var log in logs)
                {
                    DateTime fechaAccion = DateTime.Parse(log["fdFechaAccion"].ToString());
                    bool estadoActual = log["fbEstado"].ToObject<bool>();

                    if (inicio != null)
                    {
                        TimeSpan intervalo = fechaAccion - inicio.Value;
                        listaTiempos.Add(new IntervaloConexionViewModel
                        {
                            fiIndex = i,
                            fbEstado = estadoAnterior,
                            fdTiempoInicial = inicio.Value,
                            fdTiempoFinal = fechaAccion,
                            fdIntervalo = intervalo
                        });
                        i++;
                    }

                    // Actualizar el estado y la hora inicial
                    estadoAnterior = estadoActual;
                    inicio = fechaAccion;
                }

                // Manejar log incompleto si el último estado es conectado
                if (inicio != null)
                {
                    TimeSpan intervalo = ahora - inicio.Value;
                    listaTiempos.Add(new IntervaloConexionViewModel
                    {
                        fiIndex = i,
                        fbEstado = estadoAnterior,
                        fdTiempoInicial = inicio.Value,
                        fdTiempoFinal = ahora,
                        fdIntervalo = intervalo
                    });
                }
            }

            return listaTiempos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return new List<IntervaloConexionViewModel>();
        }
    }

    public List<IntervaloConexionViewModel> GetIntervaloConexionesDispositivos(int fiCantidad)
    {
        try
        {
            var listaTiempos = new List<IntervaloConexionViewModel>();
            JArray dispositivos = null;

            DateTime ahora = DateTime.Now;

            lock (fileLock)
            {
                dispositivos = (JArray)dispositivosEnMemoria;
            }

            foreach (var dispositivo in dispositivos)
            {
                JArray logs = (JArray)dispositivo["Log"];

                DateTime? inicio = null;
                bool estadoAnterior = false;
                int i = 1;

                foreach (var log in logs)
                {
                    DateTime fechaAccion = DateTime.Parse(log["fdFechaAccion"].ToString());
                    bool estadoActual = log["fbEstado"].ToObject<bool>();

                    if (inicio != null)
                    {
                        TimeSpan intervalo = fechaAccion - inicio.Value;
                        listaTiempos.Add(new IntervaloConexionViewModel
                        {
                            fiIndex = i,
                            fbEstado = estadoAnterior,
                            fdTiempoInicial = inicio.Value,
                            fdTiempoFinal = fechaAccion,
                            fdIntervalo = intervalo,
                            fcCodigoCableColor = dispositivo["fcCodigoCableColor"].ToString(),
                            fcMac = dispositivo["fcMac"].ToString(),
                            fcNombreDispositivo = dispositivo["fcNombreDispositivo"].ToString(),
                            fcTipoDispositivo = dispositivo["fcTipoDispositivo"].ToString(),
                            fiIDOrionSolicitud = Convert.ToInt32(dispositivo["fiIDOrionSolicitud"].ToString())
                        });
                        i++;
                    }

                    // Actualizar el estado y la hora inicial
                    estadoAnterior = estadoActual;
                    inicio = fechaAccion;
                }

                // Manejar log incompleto si el último estado es conectado
                if (inicio != null)
                {
                    TimeSpan intervalo = ahora - inicio.Value;
                    listaTiempos.Add(new IntervaloConexionViewModel
                    {
                        fiIndex = i,
                        fbEstado = estadoAnterior,
                        fdTiempoInicial = inicio.Value,
                        fdTiempoFinal = ahora,
                        fdIntervalo = intervalo,
                        fcCodigoCableColor = dispositivo["fcCodigoCableColor"].ToString(),
                        fcMac = dispositivo["fcMac"].ToString(),
                        fcNombreDispositivo = dispositivo["fcNombreDispositivo"].ToString(),
                        fcTipoDispositivo = dispositivo["fcTipoDispositivo"].ToString(),
                        fiIDOrionSolicitud = Convert.ToInt32(dispositivo["fiIDOrionSolicitud"].ToString())
                    });
                }
            }
            if (fiCantidad > 0)
            {
                return listaTiempos.OrderByDescending(x => x.fdTiempoInicial).Take(fiCantidad).ToList();
            }
            else
            {
                return listaTiempos.OrderByDescending(x => x.fdTiempoInicial).ToList();
            }


        }
        catch (Exception ex)
        {
            return new List<IntervaloConexionViewModel>();
        }
    }

    public async Task<RespuestaApiCepheusViewModel> GetEstadoUsuarioCepheus(string fiCodigoCepheus)
    {
        using (var client = new HttpClient())
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "http://192.168.20.23:8984/API/serviciosActivos"))
            {
                try
                {
                    request.Headers.Add("Authorization", "Basic VVNSX1JFU0xMRVJJTlRFUjowbFhwOThAJQ==");
                    var collection = new List<KeyValuePair<string, string>>();
                    collection.Add(new KeyValuePair<string, string>("cliente", fiCodigoCepheus.Replace(" ", "").Trim()));
                    collection.Add(new KeyValuePair<string, string>("usuario", "us.nova"));
                    collection.Add(new KeyValuePair<string, string>("clave", "24s/GKT29T"));

                    var content = new FormUrlEncodedContent(collection);
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var respuesta = await response.Content.ReadAsStringAsync();
                    var json = JToken.Parse(respuesta);
                    var obj = new RespuestaApiCepheusViewModel
                    {
                        codigo = Convert.ToInt32(json["codigo"].ToString()),
                        mensaje = json["mensaje"].ToString(),
                        estadoServicio = Convert.ToInt32(json["codigo"].ToString()) == 100
                    };
                    return obj;
                }
                catch (Exception ex)
                {
                    return new RespuestaApiCepheusViewModel();
                }

            }
        }
    }

    public DispositivosConLogViewModel ObtenerDispositivoCliente(int fiIDSolicitud)
    {
        var dispositivos = new JArray(dispositivosEnMemoria).ToObject<List<DispositivosConLogViewModel>>();
        return dispositivos.FirstOrDefault(x => x.fiIDOrionSolicitud == fiIDSolicitud);
    }
}
