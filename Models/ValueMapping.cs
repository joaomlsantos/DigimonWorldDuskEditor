public class ValueMapping
{
    public string ValueName { get; set; }
    public byte[] HexValue { get; set; }

    public ValueMapping()
    {
        this.ValueName = "";
        this.HexValue = new byte[0];
    }
}