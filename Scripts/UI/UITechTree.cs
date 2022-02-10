using Godot;
using System;
using System.Collections.Generic;
using Client.Utilities;

namespace Client.UI
{
    public class UITechTree : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathHBoxTechTree;
#pragma warning restore CS0649 // Values are assigned in the editor

        
        private bool Draw;
        public static Control Instance;

        public async override void _Ready()
        {
            Instance = this;

            UITechTreeResearch.Content = this;
            UITechTreeResearch.HBox = GetNode<HBoxContainer>(nodePathHBoxTechTree);
            UITechTreeResearch.Init();

            await ToSignal(GetTree(), "idle_frame");

            foreach (var pair in UITechTreeResearch.Nodes)
                UITechTreeResearch.ResearchData[pair.Key].Position = pair.Value.RectPosition;

            Draw = true;
            Update();
        }

        public override void _Draw()
        {
            if (!Draw)
                return;

            // UITechTree lines between nodes will be drawn here
            var firstNodeInTechCategory = UITechTreeResearch.TechTreeData[0].StartingResearchNodes[0];
            var firstNode = UITechTreeResearch.ResearchData[firstNodeInTechCategory];

            DrawLinesForChildren(firstNodeInTechCategory);
        }

        private float LineThickness = 5.0f;

        private void DrawLinesForChildren(ResearchType type)
        {
            var researchData = UITechTreeResearch.ResearchData;

            var node = researchData[type];

            var children = node.Children;

            if (children == null)
                return;

            var nodeSize = UITechTreeResearch.ResearchNodeSize;

            var columns = UITechTreeResearch.Columns;

            var columnPositionParent = columns[researchData[type].Depth].RectPosition;

            // horizontal line from parent
            DrawLine(columnPositionParent + node.CenterPosition, columnPositionParent + node.CenterPosition + new Vector2(nodeSize.x, 0));

            // draw long vertical line
            DrawLine(columns[researchData[children[0]].Depth].RectPosition + researchData[children[0]].CenterPosition - new Vector2(nodeSize.x, LineThickness / 2), columnPositionParent + node.CenterPosition + new Vector2(nodeSize.x, 0));

            for (int i = 0; i < children.Length; i++)
            {
                var column = columns[researchData[children[i]].Depth];
                var groupPos = ((Control)column.GetChild(0)).RectPosition;

                if (type == ResearchType.C)
                {
                    groupPos = ((Control)column.GetChild(1)).RectPosition;
                }

                var columnPositionChild = column.RectPosition + groupPos;
                // horizontal lines
                var pos = columnPositionChild + researchData[children[i]].CenterPosition - new Vector2(nodeSize.x, 0);

                DrawLine(columnPositionChild + researchData[children[i]].CenterPosition - new Vector2(nodeSize.x, 0), columnPositionChild + researchData[children[i]].CenterPosition);

                // vertical lines
                if (i != children.Length - 1)
                    DrawLine(columnPositionChild + researchData[children[i]].CenterPosition - new Vector2(nodeSize.x, LineThickness / 2), columnPositionChild + researchData[children[i + 1]].CenterPosition - new Vector2(nodeSize.x, -LineThickness / 2));

                DrawLinesForChildren(children[i]);
            }
        }

        private void DrawLine(Vector2 from, Vector2 to) => DrawLine(from, to, Colors.White, LineThickness, false);
    }
}