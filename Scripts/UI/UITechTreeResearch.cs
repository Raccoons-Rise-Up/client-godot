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
        public static Dictionary<ResearchType?, Research> ResearchData = new Dictionary<ResearchType?, Research>(){
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
                    ResearchType.D,
                    ResearchType.E,
                    ResearchType.F
                }
            }},
            { ResearchType.C, new Research {
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
                    ResearchType.R
                }
            }},
            { ResearchType.F, new Research {
                
            }},
            { ResearchType.G, new Research {
            }},
            { ResearchType.H, new Research {
            }},
            { ResearchType.I, new Research {
            }},
            { ResearchType.J, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.K
                }
            }},
            { ResearchType.K, new Research {
            }},
            { ResearchType.L, new Research {
            }},
            { ResearchType.M, new Research {
                Unlocks = new ResearchType[] {
                    ResearchType.N,
                    ResearchType.O
                }
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
        public static Vector2 ResearchNodeSize;

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
            researchInstance.Init(name);

            var data = ResearchData[type];

            researchInstance.RectPosition = data.Position;
            UITechTree.Instance.AddChild(researchInstance);
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
                return;

            int leftSide = 0;
            int rightSide = 0;
            int branchOffset = 0;
            bool prevNodeHasNoUnlocks = false;
            int previousChildUnlockGroups = 0;

            for (int i = 0; i < unlocks.Length; i++)
            {
                var childUnlocks = ResearchData[unlocks[i]].Unlocks;

                // Do not add offset if only 1 child group is present
                for (int j = 0; j < i; j++)
                    if (ResearchData[unlocks[j]].Unlocks != null)
                        previousChildUnlockGroups++;

                leftSide += 1;
                if (childUnlocks != null)
                    rightSide += childUnlocks.Length;

                if (unlocks[i] == ResearchType.E)
                    GD.Print("yes");

                if (rightSide > leftSide && i != 0 && prevNodeHasNoUnlocks && previousChildUnlockGroups >= 1) 
                {
                    prevNodeHasNoUnlocks = false;
                    GD.Print($"{unlocks[i]} {leftSide} {rightSide}");
                    branchOffset += ((rightSide - leftSide) * SPACING_VERTICAL);
                }

                if (childUnlocks == null)
                    prevNodeHasNoUnlocks = true;

                var xShift = SPACING_HORIZONTAL;
                int yShift;

                if (unlocks.Length % 2 == 0)
                    yShift = branchOffset + ((i - unlocks.Length / 2) * SPACING_VERTICAL + SPACING_VERTICAL / 2);
                else
                    yShift = branchOffset + ((i - unlocks.Length / 2) * SPACING_VERTICAL);

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
        Z
    }
}
