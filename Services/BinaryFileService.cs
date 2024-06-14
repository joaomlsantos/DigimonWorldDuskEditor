using System;
using System.IO;

public class BinaryFileService
{
    private readonly string filePath;

    public BinaryFileService(string filePath)
    {
        this.filePath = filePath;
    }

    public byte[] ReadBytes(long offset, int length)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            byte[] buffer = new byte[length];
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(buffer, 0, length);
            return buffer;
        }
    }

    public void WriteBytes(long offset, byte[] data)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
        {
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Write(data, 0, data.Length);
        }
    }
}