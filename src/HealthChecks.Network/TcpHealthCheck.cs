using System.Net.Sockets;
using HealthChecks.Network.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Network
{
    public class TcpHealthCheck : IHealthCheck
    {
        private readonly TcpHealthCheckOptions _options;

        public TcpHealthCheck(TcpHealthCheckOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var (host, port) in _options.ConfiguredHosts)
                {
                    using (var tcpClient = new TcpClient(_options.AddressFamily))
                    {
                        await tcpClient.ConnectAsync(host, port).WithCancellationTokenAsync(cancellationToken);

                        if (!tcpClient.Connected)
                        {
                            return new HealthCheckResult(context.Registration.FailureStatus, description: $"Connection to host {host}:{port} failed");
                        }
                    }
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
