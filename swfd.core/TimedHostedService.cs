using System;
using System.Threading;
using System.Threading.Tasks;
using t = System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Timers;

namespace swfd.core
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private readonly object _locker = new object();
        private t.Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new t.Timer();
            _timer.Elapsed += OnTimerElapsed;

            return Task.CompletedTask;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DoWork(object state)
        {

            if(state is CancellationToken token && token.IsCancellationRequested)
            {
                _logger.LogInformation("Timer cancellation detected");

                StopTimer();
            } else if (Monitor.TryEnter(_locker))
            {
                _logger.LogInformation("Timed Hosted Service started");

                //do work here

            } else {
                _logger.LogError("Enterlock error on timer.");
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
