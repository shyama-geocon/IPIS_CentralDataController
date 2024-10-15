using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.services
{
    public class BackupService : IService
    {
        private bool _isRunning;
        private CancellationTokenSource _cts;

        public void Start()
        {
            _isRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => PerformBackup(_cts.Token), _cts.Token);
            Console.WriteLine("BackupService started.");
        }

        public void Stop()
        {
            _isRunning = false;
            _cts.Cancel();
            Console.WriteLine("BackupService stopped.");
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        private async Task PerformBackup(CancellationToken token)
        {
            while (_isRunning && !token.IsCancellationRequested)
            {
                // Backup logic here
                Console.WriteLine("Performing backup...");
                await Task.Delay(5000); // Simulate backup work

                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Backup task canceled.");
                    break;
                }
            }
        }
    }

}
