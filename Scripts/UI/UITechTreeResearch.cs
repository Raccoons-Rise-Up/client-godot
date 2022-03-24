using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Client.UI
{
    // The word 'Node' is used a lot here to refer to 'Tech Research Nodes' placed in the tech tree, please do not confuse this with Godots nodes.
    public class UITechTreeResearch
    {
        // Research tech tree node prefab
        private static PackedScene Research = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Research.tscn");

        // Tech tree data
        public static Dictionary<ResearchType, Research> ResearchData = new Dictionary<ResearchType, Research>(){
            { ResearchType.A, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.B,
                    ResearchType.I,
                    ResearchType.J,
                    ResearchType.L,
                    ResearchType.M
                }
            }},
            { ResearchType.B, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.C,
                    ResearchType.W,
                    ResearchType.D,
                    ResearchType.E,
                    ResearchType.F
                }
            }},
            { ResearchType.C, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.U
                }
            }},
            { ResearchType.D, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.G,
                    ResearchType.H,
                    ResearchType.P,
                    ResearchType.Q
                }
            }},
            { ResearchType.E, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.R,
                    ResearchType.S
                }
            }},
            { ResearchType.F, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.T
                }
            }},
            { ResearchType.G, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.AA
                }
            }},
            { ResearchType.H, new Research {
            }},
            { ResearchType.I, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.V
                }
            }},
            { ResearchType.J, new Research {

            }},
            { ResearchType.K, new Research {
            }},
            { ResearchType.L, new Research {
            }},
            { ResearchType.M, new Research {

            }},
            { ResearchType.N, new Research {
            }},
            { ResearchType.O, new Research {
            }},
            { ResearchType.P, new Research {
            }},
            { ResearchType.Q, new Research {
            }},
            { ResearchType.R, new Research {
            }},
            { ResearchType.S, new Research {
            }},
            { ResearchType.T, new Research {
            }},
            { ResearchType.U, new Research {
            }},
            { ResearchType.V, new Research {
            }},
            { ResearchType.W, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.X
                }
            }},
            { ResearchType.X, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.Y,
                    ResearchType.Z
                }
            }},
            { ResearchType.Y, new Research {
            }},
            { ResearchType.Z, new Research {
            }},
            { ResearchType.AA, new Research {
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

        private const int SPACING_H = 200;
        private const int SPACING_V = 100;
        private static int MaxDepth { get; set; }
        public static Vector2 ResearchNodeSize { get; set; }
        private static Dictionary<int, List<ResearchType>> Nodes = new Dictionary<int, List<ResearchType>>();

        public static void Init()
        {
            // Create an instance of the prefab to get some information from it
            var researchInstance = Research.Instance<UIResearch>();
            ResearchNodeSize = researchInstance.RectSize; // Shouldn't static property be used???
            researchInstance.QueueFree(); // Free the child from the tree as we no longer have a use for it

            // Calculate depth for all nodes and calculate MaxDepth
            SetupNode(ResearchType.A, null);

            // Sort nodes by column aka depth (nodes with depth = 0 are not in any column)
            SortNodesByDepth();

            // Position the nodes
            PositionNodes();

            // Enlarge the canvas if needed
            ResizeCanvas();

            // Offset nodes to be centered at start
            OffsetNodes();

            // Create the nodes
            CreateNodes();
        }

        private static void CreateNodes()
        {
            // Create nodes
            foreach (var pair in ResearchData)
                // nodes with depth 0 have no parents or children and are therefore not connected to any nodes in the tech tree
                if (pair.Value.Depth != 0)
                    CreateNode(pair.Key);
        }

        private static void ResizeCanvas() 
        {
            // Get max XY
            float xMax = 0, yMax = 0;
            foreach (var pair in ResearchData)
                if (pair.Value.Depth != 0)
                {
                    var pos = ResearchData[pair.Key].Position;

                    if (pos.x > xMax)
                        xMax = pos.x;
                    if (pos.y > yMax)
                        yMax = pos.y;
                }

            // Set tech tree canvas minsize to size of tech tree
            if (yMax > UITechTree.Instance.RectMinSize.y / 2)
                UITechTree.Instance.RectMinSize = new Vector2(yMax * 2, yMax * 2); // seems to work just fine
        }

        private static void OffsetNodes()
        {
            // Offset all nodes to center starting node at center left
            var startPos = new Vector2(SPACING_H, UITechTree.Instance.RectMinSize.y / 2 - ResearchNodeSize.y / 2);

            var startingNode = Nodes[1][0];
            var offset = startPos - ResearchData[startingNode].Position;

            foreach (var pair in ResearchData)
                if (pair.Value.Depth != 0)
                    ResearchData[pair.Key].Position += offset;
        }

        private static void SortNodesByDepth()
        {
            for (int i = 0; i <= MaxDepth; i++)
            {
                Nodes[i] = new List<ResearchType>();
            }

            foreach (var pair in ResearchData)
            {
                var depth = pair.Value.Depth;
                Nodes[depth].Add(pair.Key);
            }
        }

        private static void PositionNodes()
        {
            // Step 1
            // Place nodes at MaxDepth
            {
                var y = 0;
                foreach (var node in Nodes[MaxDepth])
                {
                    ResearchData[node].Position = new Vector2(MaxDepth * SPACING_H, y * SPACING_V);
                    y++;
                    //CreateNode(node);
                }
            }

            for (int j = 0; j < MaxDepth - 1; j++)
            {
                // Step 2a
                // Determine positions of nodes at MaxDepth - 1 based off positions of nodes at MaxDepth

                // Nodes that were not positioned yet
                var nodesWithNoChildren = new List<ResearchType>();
                var yMax = 0f;

                foreach (var node in Nodes[MaxDepth - 1 - j])
                {
                    var data = ResearchData[node];
                    var yPos = 0f;

                    if (data.Unlocks == null)
                    {
                        // No children
                        nodesWithNoChildren.Add(node);
                    }
                    else
                    {
                        // Find weighted middle y pos based off children
                        for (int i = 0; i < data.Unlocks.Length; i++)
                        {
                            yPos += ResearchData[data.Unlocks[i]].Position.y;
                        }
                        yPos /= data.Unlocks.Length;

                        data.Position = new Vector2((MaxDepth - 1) * SPACING_H - j * SPACING_H, yPos);

                        // Determine largest Y pos
                        if (yPos > yMax)
                            yMax = yPos;
                    }
                }

                // Step 2b
                for (int i = 0; i < nodesWithNoChildren.Count; i++)
                {
                    var data = ResearchData[nodesWithNoChildren[i]];

                    data.Position = new Vector2((MaxDepth - 1) * SPACING_H - j * SPACING_H, yMax + (i + 1) * SPACING_V);
                }
            }
        }

        public static void CreateNode(ResearchType type)
        {
            var researchInstance = Research.Instance<UIResearch>();

            var name = Enum.GetName(typeof(ResearchType), type);
            var data = ResearchData[type];
            researchInstance.RectPosition = data.Position;
            researchInstance.Init(name);

            UITechTree.Instance.AddChild(researchInstance);
        }

        private static void SetupNode(ResearchType type, ResearchType? parent, int depth = 1)
        {
            var data = ResearchData[type];
            data.Depth = depth;

            if (parent != null)
                data.Requirements.Add(parent);

            var unlocks = data.Unlocks;

            if (unlocks == null)
            {
                if (depth > MaxDepth)
                    MaxDepth = depth;
                return;
            }

            for (int i = 0; i < unlocks.Length; i++)
            {
                SetupNode(unlocks[i], type, depth + 1);
            }
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
        public List<ResearchType?> Requirements = new List<ResearchType?>();
        public ResearchType[] Unlocks { get; set; }
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
        Z,
        AA,
        AB,
        AC,
        AD,
        AE,
        AF,
        AG,
        AH
    }
}
