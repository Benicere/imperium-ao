using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ImperiumAO.Server.Network;

public class GameConnection : IGameConnection
{
    private readonly int _connectionId;
    private readonly TcpClient _tcpClient;
    private readonly ILogger<GameConnection> _logger;
    private bool _disposed;

    private PipeReader _pipeReader;
    private PipeWriter _pipeWriter;

    private static readonly Dictionary<byte, int> MinPacketSizes = new()
    {
        { (byte)ClientPacketId.LoginExistingChar, 1 + 20 + 20 }, // ID + username + password
        { (byte)ClientPacketId.Walk, 1 + 1 }, // ID + direction
        { (byte)ClientPacketId.Talk, 1 + 2 }, // ID + message length (strings have int16 prefix)
        { (byte)ClientPacketId.Attack, 1 }, // ID only
        { (byte)ClientPacketId.Ping, 1 }, // ID only
        { (byte)ClientPacketId.Quit, 1 }, // ID only
    };

    public int ConnectionId => _connectionId;
    public bool IsConnected => _tcpClient?.Connected ?? false;

    public GameConnection(int connectionId, TcpClient tcpClient, ILogger<GameConnection> logger)
    {
        _connectionId = connectionId;
        _tcpClient = tcpClient;
        _logger = logger;
        _disposed = false;

        var stream = tcpClient.GetStream();
        _pipeReader = PipeReader.Create(stream);
        _pipeWriter = PipeWriter.Create(stream);
    }

    public async Task<ByteBuffer?> ReadPacketAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _pipeReader.ReadAsync(cancellationToken);
                var buffer = result.Buffer;

                if (buffer.Length == 0)
                {
                    if (result.IsCompleted)
                    {
                        _logger.LogDebug("Connection {ConnectionId}: client disconnected", _connectionId);
                        return null;
                    }
                    continue;
                }

                // Peek first byte to determine packet type
                if (buffer.FirstSpan.Length == 0)
                    continue;

                byte packetId = buffer.FirstSpan[0];
                int minSize = GetMinPacketSize(packetId);

                if (buffer.Length < minSize)
                {
                    _pipeReader.AdvanceTo(buffer.Start, buffer.End);
                    continue;
                }

                // We have enough data for a complete packet
                var packetData = new byte[buffer.Length];
                var i = 0;
                foreach (var memory in buffer)
                {
                    memory.Span.CopyTo(packetData.AsSpan(i));
                    i += memory.Length;
                }

                var packet = new ByteBuffer();
                packet.InitializeReader(packetData);

                _pipeReader.AdvanceTo(buffer.End);
                return packet;
            }

            return null;
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Connection {ConnectionId}: read cancelled", _connectionId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection {ConnectionId}: error reading packet", _connectionId);
            return null;
        }
    }

    public async Task SendAsync(ByteBuffer packet, CancellationToken cancellationToken)
    {
        try
        {
            var data = packet.GetBytes();
            await _pipeWriter.WriteAsync(data, cancellationToken);
            await _pipeWriter.FlushAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection {ConnectionId}: error sending packet", _connectionId);
        }
    }

    public async Task CloseAsync()
    {
        try
        {
            if (_tcpClient?.Connected == true)
            {
                _tcpClient.Close();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection {ConnectionId}: error closing", _connectionId);
        }

        await _pipeWriter.CompleteAsync();
        await _pipeReader.CompleteAsync();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        try { _pipeWriter?.CompleteAsync().AsTask().Wait(1000); } catch { }
        try { _pipeReader?.CompleteAsync().AsTask().Wait(1000); } catch { }
        _tcpClient?.Dispose();
    }

    private static int GetMinPacketSize(byte packetId)
    {
        return MinPacketSizes.TryGetValue(packetId, out var size) ? size : 1;
    }
}

