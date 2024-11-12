namespace WeatherApp;


class Program
{
    static void Main(string[] args)
    {
        Server server = new Server();

        //Démarre le serveur
        server.StartServer();
    }
}

