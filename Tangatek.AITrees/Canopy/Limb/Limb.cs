using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITrees
{
    public abstract class Limb : Node
    {
        #region Children
        public int childId = -1;
        private Node _child = null;
        public Node child
        {
            get { return _child; }
            set
            {
                _child = value;
                childId = (_child == null ? -1 : _child.id);
            }
        }
        #endregion

        #region Serialization / Deserialization
        public new void OnDeserialize()
        {
            if (canopy == null) return;

            child = null;

            if (!canopy.nodes.ContainsKey(childId)) return;
            child = canopy.nodes[childId];
            child.OnDeserialize();
        }
        public new void OnSerialize()
        {
            if (canopy == null) return;
            if (canopy.nodes.ContainsKey(id)) return;
            canopy.nodes.Add(id, this);
            if (child == null) return;
            child.OnSerialize();
        }
        #endregion

    }
}
