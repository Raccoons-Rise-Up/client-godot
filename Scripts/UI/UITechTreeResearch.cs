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

        private const int SPACING_HORIZONTAL = 200;
        private const int SPACING_VERTICAL = 100;
        private static int MaxDepth { get; set; }
        public static Vector2 ResearchNodeSize { get; set; }

        public static void Init() 
        {
            // Create an instance of the prefab to get some information from it
            var researchInstance = Research.Instance<UIResearch>();
            ResearchNodeSize = researchInstance.RectSize;
            researchInstance.QueueFree(); // Free the child from the tree as we no longer have a use for it

            // This is where the first research is placed on the tech tree
            //var researchStartPos = new Vector2(UITechTree.Instance.RectSize.x / 2 - ResearchNodeSize.x / 2, UITechTree.Instance.RectSize.y / 2 - ResearchNodeSize.y / 2);

            // Calculate depth for all nodes and calculate MaxDepth
            SetupNode(ResearchType.A, null, new Vector2(200, 1000));

            for (int i = 0; i < MaxDepth - 1; i++)
                PositionNode(ResearchType.A);

            AddNodes(ResearchType.A);
        }


        private static void AddNodes(ResearchType type, int index = 0, int length = 1)
        {
            CreateNode(type, index, length);

            var unlocks = ResearchData[type].Unlocks;

            if (unlocks == null)
                return;

            for (int i = 0; i < unlocks.Length; i++)
                AddNodes(unlocks[i], i, unlocks.Length);
        }

        private static int verticalCenter = 1000;

        public static void CreateNode(ResearchType type, int i, int length)
        {
            var researchInstance = Research.Instance<UIResearch>();

            var name = Enum.GetName(typeof(ResearchType), type);
            var data = ResearchData[type];
            researchInstance.RectPosition = data.Position;
            researchInstance.Init(name);

            UITechTree.Instance.AddChild(researchInstance);
        }

        private static void PositionNode(ResearchType type) 
        {
            var data = ResearchData[type];

            if (data.Unlocks == null)
                return;

            var prevChildPos = Vector2.Zero;
            var first = true;

            for (int i = 0; i < data.Unlocks.Length; i++)
            {
                var childUnlocks = ResearchData[data.Unlocks[i]].Unlocks;

                if (childUnlocks != null)
                {
                    var yOffsetChild1 = prevChildPos.y - GetFirstChildPos(data.Unlocks[i]).y;
                    //var yOffsetChild2 = prevChildPos.y - GetLastChildPos(data.Unlocks[i]).y;

                    //if (data.Unlocks[i] == ResearchType.G)
                    //{
                    //    GD.Print($"{data.Unlocks[i]} F: {yOffsetChild1} L: {yOffsetChild2}");
                    //}
                    
                    if (!first)
                    {
                        // Apply offset to all remaining nodes in column
                        for (int j = i; j < data.Unlocks.Length; j++)
                        {
                            var pos = new Vector2(0, yOffsetChild1 + ResearchNodeSize.y);
                            ApplyPosition(data.Unlocks[j], pos);
                        }
                    }

                    // the last child nested in the children
                    prevChildPos = ResearchData[childUnlocks[childUnlocks.Length - 1]].Position;
                    first = false;
                }

                PositionNode(data.Unlocks[i]);
            }
        }

        // Gets the first child nested in the children
        private static Vector2 GetFirstChildPos(ResearchType type)
        {
            var data = ResearchData[type];

            if (data.Unlocks != null)
                return GetFirstChildPos(data.Unlocks[0]);

            return ResearchData[type].Position;
        }

        private static Vector2 GetLastChildPos(ResearchType type)
        {
            var data = ResearchData[type];

            if (data.Unlocks != null)
                return GetLastChildPos(data.Unlocks[data.Unlocks.Length - 1]);

            return ResearchData[type].Position;
        }

        private static void ApplyPosition(ResearchType type, Vector2 position)
        {
            var data = ResearchData[type];

            data.Position += position;

            if (data.Unlocks == null)
                return;

            foreach (var unlock in data.Unlocks)
                ApplyPosition(unlock, position);
        }

        private static void SetupNode(ResearchType type, ResearchType? parent, Vector2 position, int depth = 1)
        {
            var data = ResearchData[type];

            data.Position = position;
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
                var xShift = SPACING_HORIZONTAL;
                int yShift;

                if (unlocks.Length % 2 == 0)
                    yShift = ((i - unlocks.Length / 2) * SPACING_VERTICAL + SPACING_VERTICAL / 2);
                else
                    yShift = ((i - unlocks.Length / 2) * SPACING_VERTICAL);

                var shift = new Vector2(xShift, yShift);

                SetupNode(unlocks[i], type, position + shift, depth + 1);
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
