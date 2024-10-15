using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.services
{
    public class AnnouncementService : IService
    {
        private bool _isRunning;
        private CancellationTokenSource _cts;

        public void Start()
        {
            _isRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => ProcessAnnouncements(_cts.Token), _cts.Token);
            Console.WriteLine("AnnouncementService started.");
        }

        public void Stop()
        {
            _isRunning = false;
            _cts.Cancel();
            Console.WriteLine("AnnouncementService stopped.");
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        private async Task ProcessAnnouncements(CancellationToken token)
        {
            while (_isRunning && !token.IsCancellationRequested)
            {
                // Logic to process and send announcements
                Console.WriteLine("Processing announcements...");
                await Task.Delay(3000); // Simulate work

                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Announcement processing canceled.");
                    break;
                }
            }
        }
    }

}
