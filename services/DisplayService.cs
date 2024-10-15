using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.services
{
    public class DisplayService : IService
    {
        private bool _isRunning;
        private CancellationTokenSource _cts;

        public void Start()
        {
            _isRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => UpdateDisplays(_cts.Token), _cts.Token);
            Console.WriteLine("DisplayService started.");
        }

        public void Stop()
        {
            _isRunning = false;
            _cts.Cancel();
            Console.WriteLine("DisplayService stopped.");
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        private async Task UpdateDisplays(CancellationToken token)
        {
            while (_isRunning && !token.IsCancellationRequested)
            {
                // Logic to update display devices over the network
                Console.WriteLine("Updating displays...");
                await Task.Delay(4000); // Simulate work

                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Display update task canceled.");
                    break;
                }
            }
        }
    }

}
