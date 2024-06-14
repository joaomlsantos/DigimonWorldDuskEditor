using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MappingService
{
    private readonly string offsetsFilePath;
    private readonly string valuesFilePath;

    public MappingService(string offsetsFilePath, string valuesFilePath)
    {
        this.offsetsFilePath = offsetsFilePath;
        this.valuesFilePath = valuesFilePath;
    }

    public List<OffsetAddress> GetOffsetAddresses()
    {
        try
        {
            var lines = File.ReadAllLines(offsetsFilePath);
            var offsetAddresses = lines.Select(line =>
            {
                var parts = line.Split('|');
                return new OffsetAddress
                {
                    AreaName = parts[1],
                    Offset = Convert.ToInt64(parts[0], 16)
                };
            }).ToList();

            return offsetAddresses;
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., file not found, parsing error)
            Console.WriteLine("Error reading offset addresses: " + ex.Message);
            return new List<OffsetAddress>();
        }
    }

    public List<ValueMapping> GetValueMappings()
    {
        try
        {
            var lines = File.ReadAllLines(valuesFilePath);
            var valueMappings = lines.Select(line =>
            {
                var parts = line.Split('|');
                return new ValueMapping
                {
                    ValueName = parts[1],
                    HexValue = StringToByteArray(parts[0])
                };
            }).ToList();

            return valueMappings;
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., file not found, parsing error)
            Console.WriteLine("Error reading value mappings: " + ex.Message);
            return new List<ValueMapping>();
        }
    }

    private byte[] StringToByteArray(string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }
}