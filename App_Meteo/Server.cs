using System;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.IO;


namespace WeatherApp
{
    public class Server
    {
        private readonly HttpListener _httpListener;
        private readonly WeatherController _weatherController;

        public Server()
        {
            //Ecoute sur le port 8080
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://localhost:8080/");
            _weatherController = new WeatherController();
        }

        //Métode pour démarrer le serveur
        public void StartServer()
        {
            _httpListener.Start();
            Console.WriteLine("Serveur démarré sur http://localhost:8080/");

            //Boucle pour écouter les requêtes (call api)
            while (true)
            {
                //Attend la requête 
                var context = _httpListener.GetContext();
                var request = context.Request;


                string responseString;

                //Traite la requête GET 
                if (request.HttpMethod == "GET")
                {
                    // Gestion des fichiers statiques
                    if (request.RawUrl.StartsWith("/static/"))
                    {
                        Static(context, request.RawUrl);
                    }
                   else if (request.RawUrl == "/")
                    {
                        responseString = _weatherController.ShowHomePage();
                            Reponse(context, responseString, "text/html");
                    }
                    else if (request.RawUrl.StartsWith("/index"))
                    {
                        //Extraire la ville de la query string
                        string city = request.QueryString["q"] ?? "";
                          if (!string.IsNullOrEmpty(city))
                        {
                            responseString = _weatherController.GetWeather(city);
                            Reponse(context, responseString, "text/html");
                        }
                        else
                        {
                            Reponse(context, "<h1> Ville incorrect </h1>", "text/html");
                        }
                    }
                    else
                    {
                        //Erreur 404
                        PageError(context);
                    }
                }
                else
                {
                    
                    Reponse(context, "<h1>Erreur 405 : Méthode non autorisée</h1>", "text/html");
                }
            }
        }

        //Méthode pour les fichier statiques
        private void Static(HttpListenerContext context, string rawUrl)
        {
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), rawUrl.TrimStart('/'));
            if (File.Exists(dirPath))
            {
                 string contentType;

        if (rawUrl.EndsWith(".css"))
        {
            contentType = "text/css";
        }
       else if (rawUrl.EndsWith(".ttf"))
        {
            contentType = "font/ttf";
        }
        else if (rawUrl.EndsWith(".jpg"))
        {
            contentType = "image/jpeg";
        }
        else
        {
            contentType = "application/octet-stream";
        }

        Reponse(context, File.ReadAllBytes(dirPath), contentType);
        }
        else
    {
        Reponse(context, "Erreur : Fichier non trouvé", "text/html");
    }
        }

        //Méthode pour la réponse
        private void Reponse(HttpListenerContext context, string responseString, string contentType)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;

            using (var output = context.Response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }

        //Méthode pour la réponse en binaire (utilisé ici pour la font)
        private void Reponse(HttpListenerContext context, byte[] buffer, string contentType)
        {   
            context.Response.ContentType = contentType;
            context.Response.ContentLength64 = buffer.Length;

            using (var output = context.Response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }

         
    //Méthode pour envoyer la page d'erreur error.html
    private void PageError(HttpListenerContext context)
    {
        string errorPagePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "error.html");
        
        if (File.Exists(errorPagePath))
        {
            string errorHtml = File.ReadAllText(errorPagePath);
            Reponse(context, errorHtml, "text/html");
        }
        else
        {
            //Si error.html n'existe pas
            Reponse(context, "<h1>Erreur 404 - Fichier non trouvé</h1>", "text/html");
        }
    }
    }
}


