using System;
using System.Text;

namespace ImperiumAO.Common.Network;

/// <summary>
/// Manejo de buffers de bytes para serialización de protocolo de red.
/// Convertido desde VB6 clsByteBuffer.cls
/// </summary>
public class ByteBuffer : IDisposable
{
    private byte[] _data;
    private long _currentPos;
    private long _lastPos;

    private const long DefaultMaxSizeFile = 65535;
    private const byte ByteSize = 1;
    private const byte BoolSize = 2;
    private const byte IntegerSize = 2;
    private const byte LongSize = 4;
    private const byte SingleSize = 4;
    private const byte DoubleSize = 8;
    private const byte StringLengthSize = 2;

    public ByteBuffer()
    {
        _data = new byte[DefaultMaxSizeFile * 20];
        _currentPos = 0;
        _lastPos = -1;
    }

    public void InitializeReader(byte[] arrayByte)
    {
        _lastPos = arrayByte.Length - 1;
        _data = new byte[_lastPos + 1];
        Array.Copy(arrayByte, _data, arrayByte.Length);
        _currentPos = 0;
    }

    public void InitializeWriter()
    {
        _data = new byte[DefaultMaxSizeFile * 20];
        _currentPos = 0;
        _lastPos = -1;
    }

    public byte[] GetBytes(long length = -1)
    {
        if (length >= 0)
        {
            var result = new byte[length];
            Array.Copy(_data, _currentPos, result, 0, length);
            return result;
        }

        var fullBuffer = new byte[_lastPos + 1];
        Array.Copy(_data, fullBuffer, _lastPos + 1);
        return fullBuffer;
    }

    public byte GetByte()
    {
        byte value = _data[_currentPos];
        _currentPos += ByteSize;
        return value;
    }

    public bool GetBoolean()
    {
        bool value = BitConverter.ToBoolean(_data, (int)_currentPos);
        _currentPos += BoolSize;
        return value;
    }

    public short GetInteger()
    {
        short value = BitConverter.ToInt16(_data, (int)_currentPos);
        _currentPos += IntegerSize;
        return value;
    }

    public int GetLong()
    {
        int value = BitConverter.ToInt32(_data, (int)_currentPos);
        _currentPos += LongSize;
        return value;
    }

    public float GetSingle()
    {
        float value = BitConverter.ToSingle(_data, (int)_currentPos);
        _currentPos += SingleSize;
        return value;
    }

    public double GetDouble()
    {
        double value = BitConverter.ToDouble(_data, (int)_currentPos);
        _currentPos += DoubleSize;
        return value;
    }

    public string GetString(int length = -1)
    {
        if (length < 0)
        {
            length = GetInteger();
            return GetString(length);
        }

        if (length <= 0)
            return string.Empty;

        string value = Encoding.UTF8.GetString(_data, (int)_currentPos, length);
        _currentPos += length;
        return value;
    }

    public void PutByte(byte value)
    {
        _data[_lastPos + 1] = value;
        _lastPos += ByteSize;
    }

    public void PutBoolean(bool value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, _data, _lastPos + 1, BoolSize);
        _lastPos += BoolSize;
    }

    public void PutInteger(short value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, _data, _lastPos + 1, IntegerSize);
        _lastPos += IntegerSize;
    }

    public void PutLong(int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, _data, _lastPos + 1, LongSize);
        _lastPos += LongSize;
    }

    public void PutSingle(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, _data, _lastPos + 1, SingleSize);
        _lastPos += SingleSize;
    }

    public void PutDouble(double value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, _data, _lastPos + 1, DoubleSize);
        _lastPos += DoubleSize;
    }

    public void PutString(string str, bool withLength = true)
    {
        if (withLength)
        {
            PutInteger((short)str.Length);
            PutString(str, false);
        }
        else if (!string.IsNullOrEmpty(str))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            Array.Copy(bytes, 0, _data, _lastPos + 1, bytes.Length);
            _lastPos += bytes.Length;
        }
    }

    public void GetVoid(int length)
    {
        _currentPos += length;
    }

    public void PutVoid(int length)
    {
        _lastPos += length;
    }

    public void ClearData()
    {
        _data = new byte[DefaultMaxSizeFile];
        _currentPos = 0;
        _lastPos = -1;
    }

    public long GetLastPos() => _lastPos;

    public long GetCurrentPos() => _currentPos;

    public bool IsEof() => _currentPos > _lastPos;

    public void Dispose()
    {
        Array.Clear(_data, 0, _data.Length);
    }
}
