using System;
using System.IO;

namespace DigimonWorldDuskEditor.Services
{
    public class BinaryFileService
    {
        private readonly string filePath;
    
        public BinaryFileService(string filePath)
        {
            this.filePath = filePath;
        }
    
    /*
        public byte[] ReadBytes(long offset, int count)
        {
            byte[] buffer = new byte[count];
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(offset, SeekOrigin.Begin);
                fs.Read(buffer, 0, count);
            }
            return buffer;
        }
        */
    
        public void WriteBytes(long offset, byte[] data)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
            {
                fs.Seek(offset, SeekOrigin.Begin);
                fs.Write(data, 0, data.Length);
            }
        }
    
    
        public List<(byte[], long)> ReadValuesAtIntervals(long baseOffset, int interval, int termination1, int termination2)
        {
            var values = new List<(byte[], long)>();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                long currentOffset = baseOffset;
                byte[] buffer = new byte[2];
    
                while (true)
                {
                    fs.Seek(currentOffset, SeekOrigin.Begin);
                    fs.Read(buffer, 0, buffer.Length);
                    
                    // Check for termination conditions
                    if ((buffer[0] == termination1 && buffer[1] == termination1) ||
                        (buffer[0] == termination2 && buffer[1] == termination2))
                    {
                        break;
                    }
    
                    values.Add(((byte[])buffer.Clone(), currentOffset)); // Add a copy of the buffer to the list
                    currentOffset += interval; // Move to the next interval
                }
            }
            return values;
        }
    }
}