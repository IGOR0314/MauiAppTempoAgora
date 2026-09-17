using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(
            object sender,
            EventArgs e)
        {
            try
            {
                // Verifica se o usuário digitou uma cidade
                if (string.IsNullOrWhiteSpace(txt_cidade.Text))
                {
                    await DisplayAlert(
                        "Atenção",
                        "Preencha o nome da cidade.",
                        "OK");

                    return;
                }

                // Consulta a previsão
                Tempo? t =
                    await DataService.GetPrevisao(
                        txt_cidade.Text.Trim());

                if (t != null)
                {
                    string visibilidade;

                    if (t.visibility.HasValue)
                    {
                        visibilidade =
                            $"{t.visibility.Value} metros";
                    }
                    else
                    {
                        visibilidade =
                            "Não disponível";
                    }

                    string dados_previsao =
                        $"Latitude: {t.lat}\n" +
                        $"Longitude: {t.lon}\n" +
                        $"Descrição: {t.description}\n" +
                        $"Velocidade do vento: {t.speed} m/s\n" +
                        $"Visibilidade: {visibilidade}\n" +
                        $"Nascer do Sol: {t.sunrise}\n" +
                        $"Pôr do Sol: {t.sunset}\n" +
                        $"Temperatura Máxima: {t.temp_max} °C\n" +
                        $"Temperatura Mínima: {t.temp_min} °C";

                    lbl_res.Text = dados_previsao;
                }
                else
                {
                    lbl_res.Text =
                        "Sem dados de previsão.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "Erro",
                    ex.Message,
                    "OK");
            }
        }
    }
}