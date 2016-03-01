using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tangatek.AITrees
{
    [AddCanopyMenu("Branches/")]
    public abstract class Branch : Node
    {
        #region Fields
        public List<int> childIds = new List<int>();
        #endregion

        #region Serialization / Deserialization
        public new virtual void OnDeserialize()
        {
            if (canopy == null) return;
            
            children.Clear();

            foreach (var childId in childIds)
                if (canopy.nodes.ContainsKey(childId))
                    children.Add(canopy.nodes[childId]);

            foreach (var child in children)
                child.OnDeserialize();
        }
        public new virtual void OnSerialize()
        {
            if (canopy == null) return;
            if (canopy.nodes.ContainsKey(id)) return;
            canopy.nodes.Add(id, this);

            childIds.Clear();

            foreach (var child in children)
                if (child != null)
                {
                    childIds.Add(child.id);
                    child.OnSerialize();
                }

        }
        #endregion

        #region Children
        protected List<Node> children = new List<Node>();
        public void AddChild(Node child)
        {
            if (child == null) return;
            if (children.Contains(child)) return;
            children.Add(child);
        }
        public void RemoveChild(Node child)
        {
            if (child == null) return;
            children.Remove(child);
        }
        public void MoveChild(Node child, int direction)
        {
            //	Get index
            var index = children.IndexOf(child);
            if (index < 0) return;

            //	remove child
            children.Remove(child);

            //	modify index
            index += direction;// - (direction > 0 ? 1 : 0);

            //	clamp index
            index = Mathf.Clamp(index, 0, children.Count);

            if (index >= children.Count) children.Add(child);
            else children.Insert(index, child);
        }
        public List<Node> GetChildren()
        {
            return children;
        }
        #endregion
    }
}
