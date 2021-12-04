using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Utils;

namespace Common.Game
{
    public abstract class StructureInfo
    {
        public virtual StructureType Type { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual Dictionary<ResourceType, uint> Cost { get; set; }
        public virtual Dictionary<ResourceType, uint> Production { get; set; }
        public virtual List<TechType> TechRequired { get; set; }

        public StructureInfo()
        {
            var rawName = GetType().Name.Replace(typeof(StructureInfo).Name, "");

            Name = SharedUtils.AddSpaceBeforeEachCapital(rawName);
            Type = (StructureType)Enum.Parse(typeof(StructureType), rawName);
            Description = "No description was given for this structure.";
            Cost = new Dictionary<ResourceType, uint>();
            Production = new Dictionary<ResourceType, uint>();
            TechRequired = new List<TechType>();
        }
    }

    public class StructureInfoHut : StructureInfo
    {
        public StructureInfoHut()
        {
            Description = "Housing for cats";
            Cost = new Dictionary<ResourceType, uint>()
            {
                { ResourceType.Wood, 100 },
                { ResourceType.Wheat, 23 }
            };
        }
    }

    public class StructureInfoWheatFarm : StructureInfo
    {
        public StructureInfoWheatFarm()
        {
            Description = "A source of food for the cats.";
            Cost = new Dictionary<ResourceType, uint>()
            {
                { ResourceType.Wood, 100 }
            };

            Production = new Dictionary<ResourceType, uint>() 
            {
                { ResourceType.Wheat, 1 }
            };
        }
    }

    public class StructureInfoStoneMine : StructureInfo 
    {
        public StructureInfoStoneMine()
        {
            Cost = new Dictionary<ResourceType, uint>
            {
                { ResourceType.Wood, 100 }
            };

            Production = new Dictionary<ResourceType, uint> 
            {
                { ResourceType.Stone, 1 }
            };
        }
    }

    public class StructureInfoGoldMine : StructureInfo
    {
        public StructureInfoGoldMine()
        {
            Cost = new Dictionary<ResourceType, uint>
            {
                { ResourceType.Wood, 100 }
            };

            Production = new Dictionary<ResourceType, uint>
            {
                { ResourceType.Gold, 1 }
            };
        }
    }

    public class StructureInfoLumberYard : StructureInfo 
    {
        public StructureInfoLumberYard() 
        {
            Description = "For chopping trees.";
            Cost = new Dictionary<ResourceType, uint> 
            {
                { ResourceType.Wood, 100 }
            };

            Production = new Dictionary<ResourceType, uint> 
            {
                { ResourceType.Wood, 1 }
            };
        }
    }

    public enum StructureType
    {
        Hut,
        WheatFarm,
        LumberYard,
        StoneMine,
        GoldMine
    }

    public enum TechType
    {

    }
}
