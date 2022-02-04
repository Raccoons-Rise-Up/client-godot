using Godot;
using System;
using System.Collections.Generic;

namespace Client.UI
{
    public class UITechTreeResearch
    {
        private static Vector2 ResearchStartPos { get; set; }

        private static PackedScene Research = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Research.tscn");

        public static Dictionary<ResearchType, Research> ResearchData = new Dictionary<ResearchType, Research>(){
            { ResearchType.A, new Research {
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
            { ResearchType.C, new Research {}},
            { ResearchType.D, new Research {}},
            { ResearchType.E, new Research {}},
            { ResearchType.F, new Research {}}
        };

        public static TechTree[] TechTreeData = new TechTree[] {
            new TechTree {
                Type = TechTreeType.Wood,
                StartingResearchNodes = new ResearchType[] {
                    ResearchType.A
                }
            }
        };

        private static int ChildSpacingHorizontal = 200;
        private static int ChildSpacingVertical = 125;
        public static Control Content;
        public static int ResearchNodeWidth = 100;
        public static int ResearchNodeHeight = 100;

        public static void Init() 
        {
            // This is where the first research is placed on the tech tree
            ResearchStartPos = new Vector2(Content.RectSize.x / 2 - ResearchNodeWidth / 2, Content.RectSize.y / 2 - ResearchNodeHeight / 2);

            var firstNodeInTechCategory = TechTreeData[0].StartingResearchNodes[0];
            var firstNode = ResearchData[firstNodeInTechCategory];

            firstNode.Position = ResearchStartPos;
            AddNode(firstNodeInTechCategory);

            AddChildrenNodes(firstNodeInTechCategory);
        }

        public static void AddChildrenNodes(ResearchType type)
        {
            var children = ResearchData[type].Children; // parent children
            var position = ResearchData[type].Position; // parent position
            
            if (children == null) // if parent has no children do nothing
                return;

            // calculate vertical center offset
            var verticalCenterOffset = ((ChildSpacingVertical * children.Length) / 2) - ChildSpacingVertical / 2;

            // add each child
            for (int i = 0; i < children.Length; i++)
            {
                var childPos = new Vector2(ChildSpacingHorizontal, (ChildSpacingVertical * i) - verticalCenterOffset);

                ResearchData[children[i]].Position = position + childPos;
                AddNode(children[i]);

                AddChildrenNodes(children[i]);
            }
        }

        public static void AddNode(ResearchType researchType)
        {
            var researchInstance = Research.Instance<UIResearch>();
            researchInstance.SetPosition(ResearchData[researchType].Position);
            researchInstance.Init(Enum.GetName(typeof(ResearchType), researchType));

            Content.AddChild(researchInstance);
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
        public ResearchType[] Children { get; set; }
        public Vector2 CenterPosition => Position + new Vector2(UITechTreeResearch.ResearchNodeWidth / 2, UITechTreeResearch.ResearchNodeHeight / 2);
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
