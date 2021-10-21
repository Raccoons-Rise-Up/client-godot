using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Utils;

namespace Common.Game
{
    public abstract class ResourceInfo
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public ResourceInfo()
        {
            var resourceName = GetType().Name.Replace(typeof(ResourceInfo).Name, "");

            Name = SharedUtils.AddSpaceBeforeEachCapital(resourceName);
        }
    }

    public class ResourceInfoWood : ResourceInfo
    {
        public ResourceInfoWood()
        {
            Description = "Some wood";
        }
    }

    public class ResourceInfoStone : ResourceInfo
    {
        public ResourceInfoStone()
        {
            Description = "Some stone";
        }
    }

    public class ResourceInfoWheat : ResourceInfo
    {
        public ResourceInfoWheat()
        {
            Description = "Some wheat";
        }
    }

    public class ResourceInfoGold : ResourceInfo
    {
        public ResourceInfoGold()
        {
            Description = "Some gold";
        }
    }

    public enum ResourceType
    {
        Wood,
        Stone,
        Wheat,
        Gold
    }
}
