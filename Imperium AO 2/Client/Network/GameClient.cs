using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;

namespace ImperiumAO.Client.Network;

public class GameClient : IDisposable
{
    private TcpClient? _tcpClient;
    private bool _isConnected;
    private CancellationTokenSource? _cancellationTokenSource;

    public bool IsConnected => _isConnected;
    public event EventHandler<ByteBuffer>? PacketReceived;

    public async Task ConnectAsync(string host, int port)
    {
        try
        {
            _tcpClient = new TcpClient();
            _cancellationTokenSource = new CancellationTokenSource();

            await _tcpClient.ConnectAsync(host, port);
            _isConnected = true;

            _ = ReadPacketsAsync();
        }
        catch (Exception)
        {
            _isConnected = false;
            throw;
        }
    }

    public async Task SendPacketAsync(ByteBuffer packet)
    {
        if (!_isConnected || _tcpClient == null)
            throw new InvalidOperationException("Not connected");

        var stream = _tcpClient.GetStream();
        var data = packet.GetBytes();
        await stream.WriteAsync(data, 0, data.Length);
    }

    public void Disconnect()
    {
        _isConnected = false;
        _cancellationTokenSource?.Cancel();
        _tcpClient?.Close();
    }

    private async Task ReadPacketsAsync()
    {
        try
        {
            var stream = _tcpClient.GetStream();
            var buffer = new byte[1024];

            while (_isConnected && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token);
                if (bytesRead == 0)
                {
                    _isConnected = false;
                    break;
                }

                var packetBuffer = new ByteBuffer();
                packetBuffer.InitializeReader(buffer[..bytesRead]);
                PacketReceived?.Invoke(this, packetBuffer);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when disconnecting
        }
        catch (Exception)
        {
            _isConnected = false;
        }
    }

    public void Dispose()
    {
        Disconnect();
        _tcpClient?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}
