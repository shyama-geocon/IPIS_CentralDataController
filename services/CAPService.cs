using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.services
{
    public class CAPService : IService
    {
        private bool _isRunning;
        private CancellationTokenSource _cts;

        public void Start()
        {
            _isRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => PollCAPServer(_cts.Token), _cts.Token);
            Console.WriteLine("CAPService started.");
        }

        public void Stop()
        {
            _isRunning = false;
            _cts.Cancel();
            Console.WriteLine("CAPService stopped.");
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        private async Task PollCAPServer(CancellationToken token)
        {
            while (_isRunning && !token.IsCancellationRequested)
            {
                // Poll CAP server logic here
                Console.WriteLine("Polling CAP server...");
                await Task.Delay(3000); // Simulate polling

                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("CAP server polling stopped.");
                    break;
                }
            }
        }
    }

}
