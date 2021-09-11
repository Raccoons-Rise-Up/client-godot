using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRU.Game
{
    public class ResourceInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ResourceInfo() 
        {
            Name = "Resource";
            Description = "No description was given for this resource.";
        }
    }
}
