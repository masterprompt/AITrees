using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Tangatek.AITrees;

namespace Tangatek.AITreesEditor
{
    public class CanopyRenderer
    {
        #region Configuration
        /// <summary>
        /// Padding between the items
        /// </summary>
        public static Vector2 padding = new Vector2(2, 2);
        /// <summary>
        /// Size of the icon
        /// </summary>
        public static Vector2 iconSize = new Vector2(18, 18);
        public static Vector2 iconHalf { get { return iconSize * 0.5f; } }
        /// <summary>
        /// Size of the line (width set by UI)
        /// </summary>
        public static Vector2 lineSize = new Vector2(18, 18);
        public static Vector2 lineHalf { get { return lineSize * 0.5f; } }

        /// <summary>
        /// Color of background box around selected node
        /// </summary>
        public static Color selectedBackgroundColor = new Color(0, 0, 0, 0.2f);
        /// <summary>
        /// Color of the lines between nodes
        /// </summary>
        public static Color connectionColor = new Color(0, 0, 0, 1);

        /// <summary>
        /// Offset to begin map (top left being 0,0)
        /// </summary>
        public static Vector2 startingOffset = new Vector2(iconSize.x, 0);
        #endregion

        #region Fields
        private static CanopyRenderer _Renderer = null;
        private Canopy canopy = null;
        private List<Line> lines = new List<Line>();
        #endregion

        #region Render
        public static void Render(Canopy canopy)
        {
            var renderer = GetRenderer(canopy);
            if (renderer == null) return;
            renderer.Render();

        }
        private static CanopyRenderer GetRenderer(Canopy canopy)
        {
            if (canopy == null) return null;
            if (_Renderer == null) _Renderer = new CanopyRenderer(canopy);
            if (_Renderer.canopy != canopy) _Renderer = new CanopyRenderer(canopy);
            return _Renderer;
        }
        #endregion

        #region Select
        public static Node Select(Canopy canopy)
        {
            var renderer = GetRenderer(canopy);
            if (renderer == null) return null;
            return null;
        }
        #endregion

        #region Constructors
        public CanopyRenderer(Canopy canopy)
        {
            this.canopy = canopy;
            if (canopy == null) return;
            Map(canopy.rootNode, 0);
        }
        #endregion

        #region Mapping
        private void Map(Node node, int tier)
        {
            if (node == null) return;
            Debug.Log("Node:" + node.name);
            lines.Add(new Line(node, tier));
            tier++;

            if (node is Branch)
            {
                var branch = (Branch)node;
                foreach (var child in branch.GetChildren())
                    Map(child, tier);
            }

            if (node is Limb)
            {
                var limb = (Limb)node;
                Map(limb.child, tier);
            }

        }
        #endregion

        #region Line
        public class Line
        {
            public int tier;
            public Node node;

            public Line(Node node, int tier)
            {
                this.node = node;
                this.tier = tier;
            }
        }
        #endregion

        #region Render
        private void Render()
        {
            Vector2 p = Vector2.zero;
            foreach (var line in lines)
            {

                var label = new Rect(0, p.y, 200, lineSize.y);

                GUI.Label(label, line.node.name);

                p.y += lineSize.y;
            }
        }
        #endregion
    }
}
