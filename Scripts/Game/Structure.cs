using KRU.Networking;
using System.Collections.Generic;

namespace KRU.Game
{
    public class Structure
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<ResourceType, uint> Cost { get; set; }
        public Dictionary<ResourceType, uint> Production { get; set; }
        public List<TechType> TechRequired { get; set; }

        public Structure()
        {
            Name = "Structure";
            Description = "No description was given for this structure.";
            Cost = new Dictionary<ResourceType, uint>();
            Production = new Dictionary<ResourceType, uint>();
            TechRequired = new List<TechType>();
        }
    }
}