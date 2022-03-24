using Godot;
using System;
using System.Collections.Generic;
using Client.Utilities;

namespace Client.UI
{
    public class UITechTree : Control
    {
        private bool Draw;
        public static Control Instance;

        public async override void _Ready()
        {
            Instance = this;

            UITechTreeResearch.Init();

            await ToSignal(GetTree(), "idle_frame");

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
            var children = node.Unlocks;

            if (children == null || children.Length == 0)
                return;

            var nodeSize = UITechTreeResearch.ResearchNodeSize;

            // horizontal line from parent
            DrawLine(node.CenterPosition, node.CenterPosition + new Vector2(nodeSize.x, 0));

            // draw long vertical line
            DrawLine(researchData[children[0]].CenterPosition - new Vector2(nodeSize.x, LineThickness / 2), node.CenterPosition + new Vector2(nodeSize.x, LineThickness / 2));

            // very un-optimal temporary solution
            DrawLine(researchData[children[0]].CenterPosition - new Vector2(nodeSize.x, LineThickness / 2), node.CenterPosition + new Vector2(nodeSize.x, -LineThickness / 2));

            for (int i = 0; i < children.Length; i++)
            {
                var childCenterPos = researchData[children[i]].CenterPosition;

                // horizontal lines
                var pos = childCenterPos - new Vector2(nodeSize.x, 0);

                DrawLine(childCenterPos - new Vector2(nodeSize.x, 0), childCenterPos);

                // vertical lines
                if (i != children.Length - 1)
                    DrawLine(childCenterPos - new Vector2(nodeSize.x, LineThickness / 2), researchData[children[i + 1]].CenterPosition - new Vector2(nodeSize.x, -LineThickness / 2));

                DrawLinesForChildren(children[i]);
            }
        }

        private void DrawLine(Vector2 from, Vector2 to) => DrawLine(from, to, Colors.White, LineThickness, false);
    }
}