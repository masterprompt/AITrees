using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangatek.Json;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Tangatek.AITrees
{
    public abstract class Node : NamedObject
    {
        #region Fields
        public int id;
        #endregion

        #region NamedObject
        public string nodeName;
        public string name { get { return nodeName; } set { nodeName = value; } }
        #endregion

        #region Tree
        private Canopy _canopy;
        public Canopy canopy { get { return _canopy; } set { _canopy = value; } }
        #endregion

        #region Node
        /// <summary>
        /// Overridable method that is called when the tree first wakes
        /// </summary>
        public virtual void OnAwake() { }
        /// <summary>
        /// Called when this node of the tree should 'tick' (this is where the logic happens)
        /// </summary>
        /// <returns></returns>
        public abstract State OnUpdate();
        /// <summary>
        /// Overridable method that is called when the tree is destroyed for cleanup
        /// </summary>
        public virtual void OnDestroy() { }
        /// <summary>
        /// Called just after the node is deserialized
        /// </summary>
        public virtual void OnDeserialize() { }
        /// <summary>
        /// Called just before the node is serialized
        /// </summary>
        public virtual void OnSerialize() 
        {
            if (canopy == null) return;
            if (canopy.nodes.ContainsKey(id)) return;
            canopy.nodes.Add(id, this);
        }
        #endregion

        #region Serialization
        public static Node Create(SerializedNode sn)
        {
            var type = GetTypeEx(sn.fqClassName);
            if (type == null) return null;
            var node = (Node)JsonMapper.ToObject(sn.json, type);
            if (node == null) return null;
            node.DeserializeReferences(sn);
            return node;
        }
        #endregion

        #region Utilities
        private static Type GetTypeEx(string fullTypeName)
        {
            fullTypeName = Regex.Replace(fullTypeName, @", Version=\d+.\d+.\d+.\d+", string.Empty);
            fullTypeName = Regex.Replace(fullTypeName, @", Culture=\w+", string.Empty);
            fullTypeName = Regex.Replace(fullTypeName, @", PublicKeyToken=\w+", string.Empty);
            return Type.GetType(fullTypeName);
        }
        #endregion

        #region References
        private void DeserializeReferences( SerializedNode sn)
        {
            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                if (!typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType)) continue;
                foreach (var reference in sn.references)
                {
                    if (reference.reference == null) continue;
                    if (reference.name != field.Name) continue;
                    field.SetValue(this, Convert.ChangeType(reference.reference, field.FieldType));
                }
            }
        }
        #endregion

    }
}
