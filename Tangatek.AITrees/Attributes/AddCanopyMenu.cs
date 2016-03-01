using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITrees
{
    /// <summary>
    /// Description that is displayed to user in the inspector
    /// </summary>
    public class AddCanopyMenu : Attribute
    {
        private string _text;
        public string canopyMenu
        {
            get
            {
                return _text;
            }
        }
        public AddCanopyMenu(string text)
        {
            this._text = text;
        }
    }
}
