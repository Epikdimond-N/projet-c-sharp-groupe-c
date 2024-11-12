using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

public class WeatherController
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "aeac6aac060d85a9da89026837c77528";

    public WeatherController()
    {
        _httpClient = new HttpClient();
    }

    public string ShowHomePage()
    {       
        //Au lancement j'ai la météo de Marseille au niveau de la div météo de l'input utilisateur et j'ai mes 4 météos fixes
         string htmlContent = GetWeather("Marseille");
         string tokyoweather = GetStaticWeather("Tokyo");
         string newyorkweather = GetStaticWeather("New York");
         string parisweather = GetStaticWeather("Paris");
         string delhiweather = GetStaticWeather("New Delhi");

        //Combine les dans le même fichier HTML
        htmlContent = htmlContent.Replace("<!-- WeatherTokyo -->", tokyoweather);
        htmlContent = htmlContent.Replace("<!-- WeatherNewYork -->", newyorkweather);
        htmlContent = htmlContent.Replace("<!-- WeatherParis -->", parisweather);
        htmlContent = htmlContent.Replace("<!-- WeatherNewDelhi -->", delhiweather);

        return htmlContent;
    }

    //Call api pour avoir la météo de la ville choisie par l'utilisateur
    public string GetWeather(string city)
    {
        string callapi = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
        HttpResponseMessage response = _httpClient.GetAsync(callapi).Result;

        if (response.IsSuccessStatusCode)
        {
            //Je récupère les données 
            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            dynamic weatherData = JObject.Parse(jsonResponse);
            string temperature = weatherData.main.temp;
            string icon = weatherData.weather[0].icon;

            int timezone = weatherData.timezone;
            TimeSpan time = TimeSpan.FromSeconds(timezone);

            DateTime utcNow = DateTime.UtcNow;
            DateTime localtime = utcNow.Add(time);

        //Je change le background en fonction de l'heure 
        string background;
        if (localtime.Hour >= 21 || localtime.Hour < 6)
        {
            background = "night";
        }
        else
        {
            background = "day";
        }
            
            //Je charge mon index.html
           string dirpath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "index.html");

            //Remplacement des données dans mon index.html
            string htmlContent = File.ReadAllText(dirpath);

            //1ère lettre en majuscule et le reste en minuscule 
            string cityupper = char.ToUpper(city[0]) + city.Substring(1).ToLower();

                 htmlContent = htmlContent.Replace("<!-- City -->", cityupper)
                                .Replace("<!-- Temperature -->", temperature+ " °C")    
                                .Replace("<!-- Icon -->", GetIcon(icon))
                                .Replace("<!-- Heure -->", localtime.ToString("HH:mm"))
                                .Replace("<!-- Background -->", background);

            //Refresh de mes météos fixes    
            string weatherTokyo = GetStaticWeather("Tokyo");
            string weatherNewYork = GetStaticWeather("New York");
            string weatherParis = GetStaticWeather("Paris");
            string weatherNewDelhi = GetStaticWeather("New Delhi");
            htmlContent = htmlContent.Replace("<!-- WeatherTokyo -->", weatherTokyo);
            htmlContent = htmlContent.Replace("<!-- WeatherNewYork -->", weatherNewYork);
            htmlContent = htmlContent.Replace("<!-- WeatherParis -->", weatherParis);
            htmlContent = htmlContent.Replace("<!-- WeatherNewDelhi -->", weatherNewDelhi);

            return htmlContent;
        }
        else
        {   
            //Erreur si la ville n'existe pas je refresh quand meme mes 4 météos fixes et j'affiche une erreur
            string dirpath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "index.html");
            string htmlContent = File.ReadAllText(dirpath);
            htmlContent = htmlContent.Replace("<!-- Error -->", "Ville incorrecte");
            string weatherTokyo = GetStaticWeather("Tokyo");
            string weatherNewYork = GetStaticWeather("New York");
            string weatherParis = GetStaticWeather("Paris");
            string weatherNewDelhi = GetStaticWeather("New Delhi");
            htmlContent = htmlContent.Replace("<!-- WeatherTokyo -->", weatherTokyo);
            htmlContent = htmlContent.Replace("<!-- WeatherNewYork -->", weatherNewYork);
            htmlContent = htmlContent.Replace("<!-- WeatherParis -->", weatherParis);
            htmlContent = htmlContent.Replace("<!-- WeatherNewDelhi -->", weatherNewDelhi);

            return htmlContent;
        }
}

  //Récupération de l'icone météo de l'api 
  public string GetIcon(string icon)
    {
        
        string iconmeteo = $"http://openweathermap.org/img/wn/{icon}@2x.png";
        return $"<img src='{iconmeteo}' alt='Icone' />";
    }


    //Call api pour avoir la météo fixe à partir d'un string ( pour mes 4 villes )
    public string GetStaticWeather(string city)
    {
        string callapi = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
        HttpResponseMessage response = _httpClient.GetAsync(callapi).Result;

        if (response.IsSuccessStatusCode)
        {
            //Je récupère les données 
            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            dynamic weatherData = JObject.Parse(jsonResponse);
            string temperature = weatherData.main.temp;
            string icon = weatherData.weather[0].icon;

            int timezone = weatherData.timezone;
            TimeSpan time = TimeSpan.FromSeconds(timezone);

            DateTime utcNow = DateTime.UtcNow;
            DateTime localtime = utcNow.Add(time);

        //Je change le background en fonction de l'heure
        string background;
        if (localtime.Hour >= 21 || localtime.Hour < 6)
        {
            background = "night";
        }
        else
        {
            background = "day";
        }
            
            //Je charge mon index.html
           string dirpath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "index.html");
            //Remplacement des données dans mon index.html
            string staticweather = $"<div class='tempinfo-rectanglefixe {background}'>"
                            + $"<h2>{city}</h2>"
                            + $"<p>{localtime.ToString("HH:mm")}</p>"
                            + $"<p>{GetIcon(icon)}</p>"
                            + $"<h3>{temperature} °C</h3>"
                            + "</div>";
            //Je return la div avec les données
            return staticweather;
    }
    else
    {
        return "<div class='tempinfo-rectanglefixe'>Erreur météo.</div>";
    }
}
}






   