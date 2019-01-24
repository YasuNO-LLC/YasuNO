using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LcuApi
{
    public class LcuApiListener : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Task _listeningTask;

        private readonly ConcurrentDictionary<string, Subject<object>> _observables;

        private readonly ClientWebSocket _socket;

        public LcuApiListener(ClientWebSocket socket)
        {
            this._socket = socket ?? throw new ArgumentNullException(nameof(socket));

            this._observables = new ConcurrentDictionary<string, Subject<object>>();

            this._listeningTask = Task.Run(
                async () =>
                {
                    var arr = new byte[1024 * 1024];
                    while (!this._cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            Array.Clear(arr, 0, arr.Length);
                            var res = await socket.ReceiveAsync(
                                          new ArraySegment<byte>(arr),
                                          this._cancellationTokenSource.Token
                                      );

                            if (!res.EndOfMessage)
                            {
                                await Console.Error.WriteLineAsync("Did not read entire buffer of event!");
                                continue;
                            }

                            var payload = Encoding.UTF8.GetString(arr);
                            var objPayload = JsonConvert.DeserializeObject<List<object>>(payload);

                            if (objPayload == null || objPayload.Count != 3 || !(objPayload[1] is string))
                            {
                                continue;
                            }

                            if (this._observables.TryGetValue((string) objPayload[1], out var observable))
                            {
                                observable.OnNext(objPayload[2]);
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            return;
                        }
                        catch (WebSocketException)
                        {
                            this.Disconnected?.Invoke(this, true);
                            this._cancellationTokenSource.Cancel();
                            return;
                        }
                        catch (Exception)
                        {
                            Debugger.Break();
                        }
                    }
                },
                this._cancellationTokenSource.Token
            );
        }

        public void Dispose()
        {
            this._cancellationTokenSource.Cancel();
            foreach (var observable in this._observables)
            {
                observable.Value.Dispose();
            }

            this._observables.Clear();

            this._listeningTask.Wait();
            this._listeningTask.Dispose();
            this._socket.Dispose();
            this._cancellationTokenSource.Dispose();
        }

        public event EventHandler<bool> Disconnected;

        public IObservable<object> GetObservable(string eventName)
        {
            return this._observables.GetOrAdd(
                eventName,
                s =>
                {
                    var subscribeStr = $"[5,\"{eventName}\"]";

                    var subscribeTask = this._socket.SendAsync(
                        new ArraySegment<byte>(
                            Encoding.UTF8.GetBytes(subscribeStr)
                        ),
                        WebSocketMessageType.Text,
                        true,
                        this._cancellationTokenSource.Token
                    );

                    var subject = new Subject<object>();

                    subscribeTask.Wait(this._cancellationTokenSource.Token);

                    return subject;
                }
            );
        }

        public class JsonEventPayload<T>
        {
            public int EventType { get; set; }
            public string EventName { get; set; }
            public T Data { get; set; }
        }
    }
}
