using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITrees
{
    [System.Serializable]
    public class UnityReference
    {
        public string name;
        public UnityEngine.Object reference;

        public UnityReference(string name, UnityEngine.Object reference)
        {
            this.name = name;
            this.reference = reference;
        }
    }
}
