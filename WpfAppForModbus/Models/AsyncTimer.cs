using System;
using System.Threading;
using System.Threading.Tasks;

namespace WpfAppForModbus.Models {
    public class AsyncTimer : IDisposable {
        private readonly int interval;
        private readonly Func<Task> callback;
        private CancellationTokenSource cancellationTokenSource = null!;
        private Task timerTask = null!;

        public AsyncTimer(int interval, Func<Task> callback) {
            this.interval = interval;
            this.callback = callback;
        }

        public void Start() {
            cancellationTokenSource = new CancellationTokenSource();
            timerTask = RunTimerAsync(cancellationTokenSource.Token);
        }

        public async Task? StopAsync() {
            cancellationTokenSource?.Cancel();

            await timerTask;
        }

        private async Task RunTimerAsync(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                await Task.Delay(interval, cancellationToken);
                if (!cancellationToken.IsCancellationRequested) {
                    await callback();
                }
            }
        }

        public void Dispose() => throw new NotImplementedException();
    }
}