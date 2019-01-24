using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LcuApi
{
    internal static class Extensions
    {
        public static string GetCommandLine(this Process process)
        {
            using (var searcher = new ManagementObjectSearcher(
                "SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id
            ))
            using (var objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            }
        }
    }

    public class Client : IDisposable
    {
        private const string ClientName = @"LeagueClientUx";
        private static readonly Regex InstallDirectoryRegex = new Regex("\"--install-directory=(?<path>[^\"]*)\"");
        private readonly HttpClient _httpClient;

        private readonly LcuApiListener _listener;
        private Builtin _builtin;
        private CareerStats _careerStats;
        private ChampSelect _champSelect;
        private EndOfGame _endOfGame;
        private Gameflow _gameflow;
        private Login _login;
        private Replays _replays;
        private Summoner _summoner;

        private Client(HttpClient client, ClientWebSocket socket, CancellationToken token)
        {
            this._httpClient = client;
            this._listener = new LcuApiListener(socket, token);

            this._listener.Disconnected += this.OnListenerOnDisconnected;
        }

        public Builtin Builtin => this._builtin ?? (this._builtin = new Builtin(this._httpClient));
        public CareerStats CareerStats => this._careerStats ?? (this._careerStats = new CareerStats(this._httpClient));
        public ChampSelect ChampSelect => this._champSelect ?? (this._champSelect = new ChampSelect(this._httpClient));
        public Gameflow Gameflow => this._gameflow ?? (this._gameflow = new Gameflow(this._httpClient));
        public Login Login => this._login ?? (this._login = new Login(this._httpClient));
        public EndOfGame EndOfGame => this._endOfGame ?? (this._endOfGame = new EndOfGame(this._httpClient));
        public Replays Replays => this._replays ?? (this._replays = new Replays(this._httpClient));
        public Summoner Summoner => this._summoner ?? (this._summoner = new Summoner(this._httpClient));

        public void Dispose()
        {
            this._listener.Disconnected -= this.OnListenerOnDisconnected;
            this._listener?.Dispose();
            this._httpClient?.Dispose();
        }

        public static async Task<Client> Connect(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var clientProcess = await Client.GetLeagueClientProcess(token);

                    var match = Client.InstallDirectoryRegex.Match(clientProcess.GetCommandLine());
                    var installDirectory = match.Groups["path"];

                    var lockfile = $"{installDirectory}lockfile";

                    if (File.Exists(lockfile))
                    {
                        string lockText;
                        using (var lockfileStream = new FileStream(
                            lockfile,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite
                        ))
                        {
                            using (var textStream = new StreamReader(lockfileStream))
                            {
                                lockText = await textStream.ReadToEndAsync();
                            }
                        }

                        var parts = lockText.Split(':');

                        var args = new ClientArgs
                                   {
                                       Process = parts[0],
                                       Pid = int.Parse(parts[1]),
                                       Port = ushort.Parse(parts[2]),
                                       Password = parts[3],
                                       Protocol = parts[4]
                                   };

                        var connectionString = $"127.0.0.1:{args.Port}/";
                        var client = new HttpClient
                                     {
                                         BaseAddress = new Uri($"{args.Protocol}://{connectionString}"),
                                         DefaultRequestHeaders =
                                         {
                                             {
                                                 "Authorization",
                                                 $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{args.Password}"))}"
                                             }
                                         }
                                     };
                        var socket = new ClientWebSocket();

                        var uri = new Uri($"wss://{connectionString}");
                        socket.Options.UseDefaultCredentials = false;
                        socket.Options.SetRequestHeader(
                            "Authorization",
                            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{args.Password}"))}"
                        );
                        await socket.ConnectAsync(uri, token);

                        return new Client(client, socket, token);
                    }

                    await Task.Delay(1000, token);
                }
            }
            catch (TaskCanceledException )
            {
                return null;
            }

            return null;
        }

        private static async Task<Process> GetLeagueClientProcess(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var clientProcess = Process
                                        .GetProcesses()
                                        .FirstOrDefault(process => process.ProcessName == Client.ClientName);

                    if (clientProcess != null)
                    {
                        return clientProcess;
                    }

                    await Task.Delay(1000, token);
                }
            }
            catch (TaskCanceledException)
            {
                return null;
            }

            return null;
        }

        private void OnListenerOnDisconnected(object sender, bool data)
        {
            this.Disconnected?.Invoke(this, data);
        }

        public IObservable<object> GetEventObservable(string eventName)
        {
            return this._listener.GetObservable(eventName);
        }

        public event EventHandler<bool> Disconnected;

        public async Task<bool> GetSwagger()
        {
            var res = await this._httpClient.GetAsync("swagger/v2/swagger.json");

            GC.KeepAlive(res);
            return true;
        }

        private class ClientArgs
        {
            public string Process { get; set; }
            public int Pid { get; set; }
            public ushort Port { get; set; }
            public string Password { get; set; }
            public string Protocol { get; set; }
        }
    }
}
