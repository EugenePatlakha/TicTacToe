using System.Diagnostics;
using TicTacToe.Service;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
        var pathToContentRoot = Path.GetDirectoryName(pathToExe);

        return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder.UseContentRoot(pathToContentRoot)
                            .UseStartup<Startup>()
                            .UseUrls("http://localhost:61234/");
                });

    }
}
