using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITrees
{
    /// <summary>
    /// Description that is displayed to user in the inspector
    /// </summary>
    public class Description : Attribute
    {
        private string _text;
        public string text
        {
            get
            {
                return _text;
            }
        }
        public Description(string text)
        {
            this._text = text;
        }
    }
}
