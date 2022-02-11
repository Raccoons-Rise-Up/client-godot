using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Client.UI
{
    public class UITechTreeResearch
    {
        // Research tech tree node prefab
        private static PackedScene Research = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Research.tscn");

        // Tech tree data
        public static Dictionary<ResearchType, Research> ResearchData = new Dictionary<ResearchType, Research>(){
            { ResearchType.M, new Research {
                Requirements = new ResearchType[] {
                    ResearchType.O
                }
            }},
            { ResearchType.N, new Research {
            }},
            { ResearchType.O, new Research {
            }},
            { ResearchType.P, new Research {
                Requirements = new ResearchType[] {
                    ResearchType.N,
                    ResearchType.O
                }
            }},
            { ResearchType.Q, new Research {
                Requirements = new ResearchType[] {
                    ResearchType.P
                }
            }}
        };

        // There will be multiple tech trees
        public static TechTree[] TechTreeData = new TechTree[] {
            new TechTree {
                Type = TechTreeType.Wood,
                StartingResearchNodes = new ResearchType[] {
                    ResearchType.A
                }
            }
        };

        private static List<ResearchType> AddedNodes = new List<ResearchType>();

        private const int SPACING_HORIZONTAL = 200;
        private const int SPACING_VERTICAL = 150;
        public static Vector2 ResearchNodeSize;

        public static void Init() 
        {
            // Create an instance of the prefab to get some information from it
            var researchInstance = Research.Instance<UIResearch>();
            ResearchNodeSize = researchInstance.RectSize;
            researchInstance.QueueFree(); // Free the child from the tree as we no longer have a use for it

            // This is where the first research is placed on the tech tree
            var researchStartPos = new Vector2(UITechTree.Instance.RectSize.x / 2 - ResearchNodeSize.x / 2, UITechTree.Instance.RectSize.y / 2 - ResearchNodeSize.y / 2);

            //var firstNodeInTechCategory = TechTreeData[0].StartingResearchNodes[0];
            //var firstNode = ResearchData[firstNodeInTechCategory];

            //firstNode.Position = researchStartPos;

            // Calculate depth for all nodes and calculate MaxDepth
            foreach (var node in ResearchData)
                ResearchData[node.Key].Depth = RecursivelyCalculateDepth(node.Key, null);

            foreach (var node in ResearchData)
                GD.Print($"{node.Key} {node.Value.Depth} Children: {string.Join(" ", node.Value.Children.Select(x => Enum.GetName(typeof(ResearchType), x)))}");

            foreach (var node in ResearchData)
                RecursivelyAddRequirements(node.Key);

            //RecursivelyAddRequirements(ResearchType.M);
        }


        private static void RecursivelyAddRequirements(ResearchType type, int index = 0, int length = 1)
        {
            CreateNode(type, index, length);

            var requirements = ResearchData[type].Requirements;

            if (requirements == null)
                return;

            for (int i = 0; i < requirements.Length; i++)
            {
                RecursivelyAddRequirements(requirements[i], i, requirements.Length);
            }
        }

        public static void CreateNode(ResearchType researchType, int i, int length)
        {
            if (AddedNodes.Contains(researchType))
                return;

            var researchInstance = Research.Instance<UIResearch>();

            var name = Enum.GetName(typeof(ResearchType), researchType);
            researchInstance.Init(name);

            var data = ResearchData[researchType];

            // Positioning
            var verticalCenter = 1000;

            var x = data.Depth * SPACING_HORIZONTAL;
            var y = verticalCenter + i * SPACING_VERTICAL - ((length * SPACING_VERTICAL) / 2) + (data.Children.Count() * SPACING_VERTICAL);

            researchInstance.RectPosition = new Vector2(x, y);
            UITechTree.Instance.AddChild(researchInstance);
            AddedNodes.Add(researchType);
        }

        private static int RecursivelyCalculateDepth(ResearchType type, ResearchType? prev, int depth = 1)
        {

            var requirements = ResearchData[type].Requirements;

            if (prev != null)
                if (!ResearchData[type].Children.Contains(prev))
                    ResearchData[type].Children.Add(prev);
            
            // Return depth if there are no requirements
            if (requirements == null)
                return depth;

            // There is at least one requirement, increase depth by 1
            depth++;

            // Attempt to find any more requirements
            foreach (var requirement in requirements)
            {
                RecursivelyCalculateDepth(requirement, type, depth);

                if (ResearchData[requirement].Requirements == null)
                    continue;

                // There is at least one requirement, increase depth by 1
                depth++;
            }
                
            return depth;
        }
    }

    public struct TechTree
    {
        public TechTreeType Type { get; set; }
        public ResearchType[] StartingResearchNodes { get; set; }
    }

    public class Research
    {
        public Vector2 Position { get; set; }
        public Vector2 CenterPosition => Position + new Vector2(UITechTreeResearch.ResearchNodeSize.x / 2, UITechTreeResearch.ResearchNodeSize.y / 2);
        public ResearchType[] Requirements { get; set; }
        public List<ResearchType?> Children = new List<ResearchType?>();
        public int Depth { get; set; }
    }

    public enum TechTreeType 
    {
        Wood
    }

    public enum ResearchType 
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z
    }
}
