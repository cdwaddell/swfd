using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace swfd.core
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(
                DoWork, 
                stoppingToken,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {

            if(state is CancellationToken token && token.IsCancellationRequested)
            {
                _logger.LogInformation("Timer cancellation detected");

                StopTimer();
            } else {
                
                var count = Interlocked.Increment(ref executionCount);

                _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
            }
        }

        private void StopTimer()
        {
            _timer?.Change(Timeout.Infinite, 0);
            
            _logger.LogInformation("Timer stopped.");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            StopTimer();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            StopTimer();

            _timer?.Dispose();
        }
    }
}
