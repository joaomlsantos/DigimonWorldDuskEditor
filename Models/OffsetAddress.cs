
namespace DigimonWorldDuskEditor.Models
{
    public class OffsetAddress
    {
        public string AreaName { get; set; }
        public long Offset { get; set; } // Use long to handle large offset values

        public OffsetAddress()
        {
            this.AreaName = "";
        }
    }
}