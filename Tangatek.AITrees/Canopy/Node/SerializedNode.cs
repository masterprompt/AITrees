using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Tangatek.Json;


namespace Tangatek.AITrees
{
    [System.Serializable]
    public class SerializedNode
    {
        #region Fields
        public string fqClassName;
        public string json;
        public List<UnityReference> references = new List<UnityReference>();
        #endregion

        #region Serialization
        public static SerializedNode Create(Node node)
        {
            if (node == null) return null;
            var sn = new SerializedNode();
            sn.fqClassName = node.GetType().AssemblyQualifiedName;
            sn.json = JsonMapper.ToJson(node);
            sn.SerializeReferences(node);
            return sn;
        }
        #endregion

        #region Unity References
        private void SerializeReferences(Node node)
        {
            references.Clear();
            if (node == null) return;

            var fields = node.GetType().GetFields();
            foreach (var field in fields)
            {
                if (!typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType)) continue;
                var fieldValue = (UnityEngine.Object)field.GetValue(node);
                if (fieldValue == null) continue;
                references.Add(new UnityReference(field.Name, fieldValue));
            }
        }
        #endregion

        
    }
}
