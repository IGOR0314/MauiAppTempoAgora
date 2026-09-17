using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            string chave = "6135072afe7f6cec1537d5cb08a5a1a2";

            string url =
                $"https://api.openweathermap.org/data/2.5/weather?" +
                $"q={cidade}&units=metric&lang=pt_br&appid={chave}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp =
                        await client.GetAsync(url);

                    // Cidade não encontrada
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new Exception(
                            "A cidade informada não foi encontrada.");
                    }

                    // Outros erros da API
                    if (!resp.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            $"Erro ao consultar a previsão. " +
                            $"Código HTTP: {(int)resp.StatusCode}.");
                    }

                    string json =
                        await resp.Content.ReadAsStringAsync();

                    JObject rascunho =
                        JObject.Parse(json);

                    Tempo t = new Tempo();

                    // Latitude
                    t.lat =
                        rascunho["coord"]?["lat"]?.Value<double>();

                    // Longitude
                    t.lon =
                        rascunho["coord"]?["lon"]?.Value<double>();

                    // Descrição do clima
                    t.description =
                        rascunho["weather"]?[0]?["description"]?
                        .Value<string>();

                    // Clima principal
                    t.main =
                        rascunho["weather"]?[0]?["main"]?
                        .Value<string>();

                    // Temperatura mínima
                    t.temp_min =
                        rascunho["main"]?["temp_min"]?
                        .Value<double>();

                    // Temperatura máxima
                    t.temp_max =
                        rascunho["main"]?["temp_max"]?
                        .Value<double>();

                    // Velocidade do vento
                    t.speed =
                        rascunho["wind"]?["speed"]?
                        .Value<double>();

                    // Visibilidade
                    if (rascunho["visibility"] != null)
                    {
                        t.visibility =
                            rascunho["visibility"]!.Value<int>();
                    }
                    else
                    {
                        t.visibility = null;
                    }

                    // Nascer do Sol
                    if (rascunho["sys"]?["sunrise"] != null)
                    {
                        long sunriseUnix =
                            rascunho["sys"]!["sunrise"]!
                            .Value<long>();

                        t.sunrise =
                            DateTimeOffset
                                .FromUnixTimeSeconds(sunriseUnix)
                                .ToLocalTime()
                                .ToString("dd/MM/yyyy HH:mm");
                    }

                    // Pôr do Sol
                    if (rascunho["sys"]?["sunset"] != null)
                    {
                        long sunsetUnix =
                            rascunho["sys"]!["sunset"]!
                            .Value<long>();

                        t.sunset =
                            DateTimeOffset
                                .FromUnixTimeSeconds(sunsetUnix)
                                .ToLocalTime()
                                .ToString("dd/MM/yyyy HH:mm");
                    }

                    return t;
                }
            }
            catch (HttpRequestException)
            {
                throw new Exception(
                    "Não foi possível conectar à internet. " +
                    "Verifique sua conexão e tente novamente.");
            }
            catch (TaskCanceledException)
            {
                throw new Exception(
                    "A consulta demorou muito para responder. " +
                    "Verifique sua conexão com a internet.");
            }
        }
    }
}