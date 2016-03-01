using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITrees
{
    /// <summary>
    /// Name of the icon to use in the editor. Icon must be in a resources folder somewhere in project.
    /// </summary>
    public sealed class Icon : Attribute
    {

        private string iconName;
        public string name
        {
            get
            {
                return iconName;
            }
        }
        public Icon(string iconName)
        {
            this.iconName = iconName;
        }
    }
}
