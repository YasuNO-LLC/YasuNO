using System;
using System.Net;
using System.Threading.Tasks;

using LcuApi;

namespace Yasuno
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Program.MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

            while (true)
            {
                try
                {
                    using (var client = await Client.Connect())
                    {
                        var connected = true;

                        void OnClientOnDisconnected(object sender, bool e)
                        {
                            connected = false;
                        }

                        client.Disconnected += OnClientOnDisconnected;

                        Console.WriteLine("Connected to league!");

                        while (await client.Summoner.CurrentSummoner() == null)
                        {
                            // wait until logged in
                            await Task.Delay(5000);
                        }

                        using (new NoPicker(client))
                        {
                            while (connected)
                            {
                                await Task.Delay(1000);
                            }
                        }

                        client.Disconnected -= OnClientOnDisconnected;

                        Console.WriteLine("Disconnected from league");
                    }
                }
                catch (Exception e)
                {
                    await Console.Error.WriteLineAsync(e.Message);
                }
            }
        }
    }
}
