using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using LcuApi;

namespace Yasuno
{
    internal class YasuNo : IDisposable
    {
        private readonly Task _runner;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public YasuNo()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

            this._runner = Task.Run(
                async () =>
                {
                    while (!this._tokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            using (var client = await Client.Connect(this._tokenSource.Token))
                            {
                                var connected = true;

                                void OnClientOnDisconnected(object sender, bool e)
                                {
                                    connected = false;
                                }

                                client.Disconnected += OnClientOnDisconnected;

                                Debugger.Log(0, "", "Connected to league!\n");

                                while (await client.Summoner.CurrentSummoner() == null)
                                {
                                    // wait until logged in
                                    await Task.Delay(5000, this._tokenSource.Token);
                                }

                                using (new NoPicker(client))
                                {
                                    while (connected && !this._tokenSource.IsCancellationRequested)
                                    {
                                        await Task.Delay(1000, this._tokenSource.Token);
                                    }
                                }

                                client.Disconnected -= OnClientOnDisconnected;

                                Debugger.Log(0, "", "Disconnected from league\n");
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            return;
                        }
                        catch (Exception e)
                        {
                            await Console.Error.WriteLineAsync(e.Message);
                        }
                    }
                }
            );
        }

        public void Dispose()
        {
            this._tokenSource.Cancel();
            this._runner.Wait();
            this._runner.Dispose();
            this._tokenSource.Dispose();
        }
    }
}
