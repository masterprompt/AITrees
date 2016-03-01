using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITreesEditor
{
    /// <summary>
    /// Canopy inspector attribute.  Class must be subclass of CanopyInspector
    /// </summary>
    public sealed class CanopyEditor : Attribute
    {

        private Type inspectedType;
        public Type type
        {
            get
            {
                return inspectedType;
            }
        }
        public CanopyEditor(Type inspectedType)
        {
            this.inspectedType = inspectedType;
        }
    }
}
