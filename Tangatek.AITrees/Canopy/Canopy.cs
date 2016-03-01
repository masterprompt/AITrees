using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITrees
{
    /// <summary>
    /// Content of our tree
    /// </summary>
    public class Canopy
    {
        #region Fields
        public Dictionary<int, Node> nodes = new Dictionary<int, Node>();
        public int id = 0;
        public Node rootNode = null;

        public AITree tree = null;
        #endregion

        #region Canopy
        public void OnAwake()
        {
            foreach (KeyValuePair<int, Node> pair in nodes)
                if (pair.Value != null)
                    pair.Value.OnAwake();
        }
        public void OnDestroy()
        {
            foreach (KeyValuePair<int, Node> pair in nodes)
                if (pair.Value != null)
                    pair.Value.OnDestroy();
        }
        public State OnUpdate()
        {
            if (rootNode == null) return State.Failure;
            return rootNode.OnUpdate();

        }
        /// <summary>
        /// Called just after the node is deserialized (climb the stack and re-reference any needed child nodes)
        /// </summary>
        public void OnDeserialize() 
        {
            if (rootNode == null) return;
            rootNode.OnDeserialize();
        }
        /// <summary>
        /// Called just before the node is serialized (climb the stack and catelog each node: IE self cleanup)
        /// </summary>
        public void OnSerialize() 
        {
            nodes.Clear();
            if (rootNode == null) return;
            rootNode.OnSerialize();
        }
        #endregion

        #region Generate Node
        public Node CreateNode(System.Type type)
        {
            if (type == null) return null;
            // Create behaviour and assign it it's properties
            var node = (Node)Activator.CreateInstance(type);
            if (node == null) return null;

            node.name = type.Name;
            node.id = id;
            UnityEngine.Debug.Log("CreateNode:" + node.id);
            if (!nodes.ContainsKey(node.id)) nodes.Add(node.id, node);
            UnityEngine.Debug.Log("Nodes:" + nodes.Count);
            nodes[node.id] = node;
            id++;
            //	Give it back
            return node;
        }
        #endregion

        #region Deserialization
        /// <summary>
        /// Deserializes a serialized canopy into a canopy
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public static Canopy Create(SerializedCanopy sc)
        {
            return Create(sc, null);
        }
        /// <summary>
        /// Deserializes a serialized canopy into a canopy
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static Canopy Create(SerializedCanopy sc, AITree tree)
        {
            UnityEngine.Debug.Log("Canopy Create");
            var canopy = new Canopy();
            if (sc == null) return canopy;
            canopy.id = sc.id;
            canopy.tree = tree;
            foreach (var sn in sc.nodes)
            {
                if (sn == null) continue;
                var node = Node.Create(sn);
                if (node == null) continue;
                node.canopy = canopy;
                if (canopy.nodes.ContainsKey(node.id)) continue;
                canopy.nodes.Add(node.id, node);
            }

            //  Assign root
            if (canopy.nodes.ContainsKey(sc.rootId)) canopy.rootNode = canopy.nodes[sc.rootId];
            if (canopy.rootNode == null)
            {
                //  Create a root node by default
                canopy.rootNode = canopy.CreateNode(typeof(DedicatedBranch));
                canopy.rootNode.name = "Root";
            }

            //  Deserialize Each node
            canopy.OnDeserialize();

            return canopy;
        }
        #endregion

        #region Editor
        private Node _selectedNode;
        public Node selectedNode { get { return _selectedNode; } set { _selectedNode = (value != null ? value : _selectedNode); _selectedNode = (_selectedNode != null ? _selectedNode : rootNode); UnityEngine.Debug.Log("Selected:" + (_selectedNode!=null ? _selectedNode.name : "None")); } }
        public Branch selectedBranch { get { return (_selectedNode is Branch ? (Branch)_selectedNode : null); } }
        public Limb selectedLimb { get { return (_selectedNode is Limb ? (Limb)_selectedNode : null); } }
        public Leaf selectedLeaf { get { return (_selectedNode is Leaf ? (Leaf)_selectedNode : null); } }
        #endregion
    }
}
