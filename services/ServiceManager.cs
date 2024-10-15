using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.services
{
    public class ServiceManager
    {
        private readonly List<IService> _services;

        public ServiceManager(IEnumerable<IService> services)
        {
            _services = services.ToList();
        }

        // Start all services
        public void StartAllServices()
        {
            foreach (var service in _services)
            {
                try
                {
                    service.Start();
                    Console.WriteLine($"{service.GetType().Name} started successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start {service.GetType().Name}: {ex.Message}");
                }
            }
        }

        // Stop all services
        public void StopAllServices()
        {
            foreach (var service in _services)
            {
                try
                {
                    service.Stop();
                    Console.WriteLine($"{service.GetType().Name} stopped successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to stop {service.GetType().Name}: {ex.Message}");
                }
            }
        }

        // Check the health of all services
        public void CheckServiceHealth()
        {
            foreach (var service in _services)
            {
                if (!service.IsRunning())
                {
                    Console.WriteLine($"{service.GetType().Name} is not running. Attempting to restart...");
                    RestartService(service.GetType());
                }
                else
                {
                    Console.WriteLine($"{service.GetType().Name} is running fine.");
                }
            }
        }

        // Restart a specific service
        public void RestartService(Type serviceType)
        {
            var service = _services.FirstOrDefault(s => s.GetType() == serviceType);
            if (service != null)
            {
                service.Stop();
                service.Start();
                Console.WriteLine($"{serviceType.Name} restarted successfully.");
            }
        }
    }
}
