using KRU.Networking;
using System.Collections.Generic;

namespace KRU.Game
{
    public class StructureInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<ushort, uint> Cost { get; set; }
        public Dictionary<ushort, uint> Production { get; set; }
        public List<ushort> TechRequired { get; set; }

        public StructureInfo()
        {
            Name = "Structure";
            Description = "No description was given for this structure.";
            Cost = new Dictionary<ushort, uint>();
            Production = new Dictionary<ushort, uint>();
            TechRequired = new List<ushort>();
        }
    }
}