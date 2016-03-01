using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITrees
{
    [System.Serializable]
    public class SerializedCanopy
    {
        #region Fields
        public List<SerializedNode> nodes = new List<SerializedNode>();
        public int id = 0;
        public int rootId = 0;
        #endregion

        #region Serialization
        /// <summary>
        /// Serializes a canopy into a serialized canopy
        /// </summary>
        /// <param name="canopy"></param>
        /// <returns></returns>
        public static SerializedCanopy Create(Canopy canopy)
        {
            var sc = new SerializedCanopy();
            if (canopy == null) return sc;
            UnityEngine.Debug.Log("canopy.OnSerialize()");
            canopy.OnSerialize();
            sc.id = canopy.id;
            UnityEngine.Debug.Log("Root ID");
            sc.rootId = (canopy.rootNode != null ? canopy.rootNode.id : -1);
            UnityEngine.Debug.Log("canopy.nodes:" + canopy.nodes.Count);
            foreach (KeyValuePair<int, Node> pair in canopy.nodes)
            {
                UnityEngine.Debug.Log("pair.Value Key:" + pair.Key);
                if (pair.Value == null) continue;
                UnityEngine.Debug.Log("SerializedNode.Create");
                var sn = SerializedNode.Create(pair.Value);
                if (sn == null) continue;
                UnityEngine.Debug.Log("sc.nodes.Add");
                sc.nodes.Add(sn);
            }
            UnityEngine.Debug.Log("return sc");
            return sc;
        }
        #endregion
    }
}
