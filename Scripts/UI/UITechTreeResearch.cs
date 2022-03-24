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

        /*public static Dictionary<ResearchType, Research> ResearchData = new Dictionary<ResearchType, Research>() {
            {
                ResearchType.A, new Research {
                    Unlocks = new ResearchType[] {
                        ResearchType.B,
                        ResearchType.C
                    }
                }
            }, 
            {   ResearchType.B, new Research {
                    Unlocks = new ResearchType[] {
                        ResearchType.D,
                        ResearchType.E
                    }
            } },
            {   ResearchType.C, new Research {} },
            {   ResearchType.D, new Research {} },
            {   ResearchType.E, new Research {} }
        };*/

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

        public static void Init()
        {
            // Create an instance of the prefab to get some information from it
            var researchInstance = Research.Instance<UIResearch>();
            ResearchNodeSize = researchInstance.RectSize; // Shouldn't static property be used???
            researchInstance.QueueFree(); // Free the child from the tree as we no longer have a use for it

            // This is where the first research is placed on the tech tree
            //var researchStartPos = new Vector2(UITechTree.Instance.RectSize.x / 2 - ResearchNodeSize.x / 2, UITechTree.Instance.RectSize.y / 2 - ResearchNodeSize.y / 2);

            // Calculate depth for all nodes and calculate MaxDepth
            SetupNode(ResearchType.A, null);

            SortNodesByDepth();

            // --- DEBUG ---
            foreach (var node in Nodes)
            {
                var depth = node.Key;
                var children = node.Value.Select(x => x.ToString()).ToArray();

                // Print nodes sorted by depth
                GD.Print($"{depth} {string.Join(" ", children)}");
            }
            // --- DEBUG ---

            PositionNodes();
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
                    ResearchData[node].Position = new Vector2(600, y * SPACING_V);
                    y++;
                    CreateNode(node);
                }
            }

            // Step 2a
            // Determine positions of nodes at MaxDepth - 1 based off positions of nodes at MaxDepth

            // Nodes that were not positioned yet
            var nodesWithNoChildren = new List<ResearchType>();
            var largestYPos = 0f;

            foreach (var node in Nodes[MaxDepth - 1]) 
            {
                var data = ResearchData[node];
                var pos = Vector2.Zero;

                if (data.Unlocks == null)
                {
                    // No children
                    nodesWithNoChildren.Add(node);
                }
                else
                {
                    // Find middle y pos based off children
                    for (int i = 0; i < data.Unlocks.Length; i++)
                    {
                        pos += ResearchData[data.Unlocks[i]].Position;
                    }
                    pos /= data.Unlocks.Length;

                    data.Position = new Vector2(400, pos.y);

                    // Determine largest Y pos
                    if (pos.y > largestYPos)
                        largestYPos = pos.y;

                    CreateNode(node);
                }
            }

            // Step 2b
            for (int i = 0; i < nodesWithNoChildren.Count; i++)
            {
                var data = ResearchData[nodesWithNoChildren[i]];

                data.Position = new Vector2(400, largestYPos + (i + 1) * SPACING_V);

                CreateNode(nodesWithNoChildren[i]);
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

        private static Dictionary<int, List<ResearchType>> Nodes = new Dictionary<int, List<ResearchType>>();


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

    public struct Column 
    {
        public Group[] Groups;
    }

    public struct Group 
    {
        public ResearchType[] Nodes;
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
