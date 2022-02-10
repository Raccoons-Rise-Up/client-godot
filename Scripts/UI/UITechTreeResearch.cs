using Godot;
using System;
using System.Collections.Generic;

namespace Client.UI
{
    public class UITechTreeResearch
    {
        // Research tech tree node prefab
        private static PackedScene Research = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Research.tscn");

        // Tech tree data
        public static Dictionary<ResearchType, Research> ResearchData = new Dictionary<ResearchType, Research>(){
            { ResearchType.A, new Research {
                Depth = 1,
                Children = new ResearchType[] {
                    ResearchType.B,
                    ResearchType.C,
                    ResearchType.D
                }
            }},
            { ResearchType.B, new Research {
                Children = new ResearchType[] {
                    ResearchType.E,
                    ResearchType.F
                }
            }},
            { ResearchType.C, new Research {
                Children = new ResearchType[] {
                    ResearchType.G,
                    ResearchType.H
                }
            }},
            { ResearchType.D, new Research {
            }},
            { ResearchType.E, new Research {
            }},
            { ResearchType.F, new Research {
            }},
            { ResearchType.G, new Research {
            }},
            { ResearchType.H, new Research {
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

        public static Dictionary<int, VBoxContainer> Columns = new Dictionary<int, VBoxContainer>();
        public static Dictionary<ResearchType, UIResearch> Nodes = new Dictionary<ResearchType, UIResearch>();

        private const int COLUMN_SPACING = 100;
        public static Vector2 ResearchNodeSize;
        private static int MaxDepth;

        // UITechTree.cs Variables
        public static Control Content;
        public static HBoxContainer HBox;

        public static void Init() 
        {
            // Create an instance of the prefab to get some information from it
            var researchInstance = Research.Instance<UIResearch>();
            ResearchNodeSize = researchInstance.RectSize;
            researchInstance.QueueFree(); // Free the child from the tree as we no longer have a use for it

            // This is where the first research is placed on the tech tree
            var researchStartPos = new Vector2(Content.RectSize.x / 2 - ResearchNodeSize.x / 2, Content.RectSize.y / 2 - ResearchNodeSize.y / 2);

            var firstNodeInTechCategory = TechTreeData[0].StartingResearchNodes[0];
            var firstNode = ResearchData[firstNodeInTechCategory];

            firstNode.Position = researchStartPos;

            // Calculate depth for all nodes and calculate MaxDepth
            RecursivelyCalculateDepth(ResearchType.A); 

            // Create columns based on MaxDepth
            CreateColumns(); 

            // Create the first group for column 1
            var group = CreateGroup();
            group.AddChild(CreateNode(ResearchType.A));
            Columns[1].AddChild(group);

            // Add children for each node starting from first node
            RecursivelyAddChildren(ResearchType.A); 
        }

        private static void CreateColumns()
        {
            for (int i = 1; i <= MaxDepth; i++)
                CreateColumn(i);
        }

        private static void CreateColumn(int index)
        {
            var column = new VBoxContainer();
            column.Name = "Column " + index;

            var horizontalPadding = new Control();
            horizontalPadding.RectMinSize = new Vector2(COLUMN_SPACING, 0);
            horizontalPadding.MouseFilter = Control.MouseFilterEnum.Ignore;

            HBox.AddChild(column);
            HBox.AddChild(horizontalPadding);

            Columns.Add(index, column); // keep track of columns
        }

        private static void RecursivelyAddChildren(ResearchType type)
        {
            var children = ResearchData[type].Children; // parent children
            
            if (children == null) // if parent has no children do nothing
                return;

            var group = CreateGroup(); // create a group to be added to the column

            foreach (var child in children) // add children to this group
            {
                group.AddChild(CreateNode(child));

                RecursivelyAddChildren(child);
            }

            Columns[ResearchData[children[0]].Depth].AddChild(group);
        }

        private static VBoxContainer CreateGroup()
        {
            var group = new VBoxContainer();
            group.Alignment = BoxContainer.AlignMode.Center;
            group.SizeFlagsVertical = (int)Control.SizeFlags.ExpandFill;
            return group;
        }

        public static UIResearch CreateNode(ResearchType researchType)
        {
            var researchInstance = Research.Instance<UIResearch>();
            researchInstance.Init(Enum.GetName(typeof(ResearchType), researchType));
            Nodes[researchType] = researchInstance;
            return researchInstance;
        }

        private static void RecursivelyCalculateDepth(ResearchType type)
        {
            var children = ResearchData[type].Children; // parent children
            
            if (children == null) // if parent has no children do nothing
                return;

            foreach (var child in children)
            {
                ResearchData[child].Depth = ResearchData[type].Depth + 1;

                if (ResearchData[child].Depth > MaxDepth)
                    MaxDepth = ResearchData[child].Depth;
                
                RecursivelyCalculateDepth(child);
            }
        }
    }

    public struct Column 
    {
        public VBoxContainer VBoxColumn { get; set; }
        public VBoxContainer[] Groups { get; set; }
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
        public ResearchType[] Children { get; set; }
        public int Group { get; set; }
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
        I
    }
}
