using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITrees
{
    /// <summary>
    /// Indicates that this node is only for a particualar type of tree
    /// </summary>
    public sealed class OnlyTree : Attribute
    {
        private Type treeType;
        public Type type
        {
            get
            {
                return treeType;
            }
        }
        public OnlyTree(Type treeType)
        {
            this.treeType = treeType;
        }
    }
}
