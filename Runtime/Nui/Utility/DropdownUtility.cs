using System;
using System.Collections.Generic;
using System.Linq;

namespace Noo.Nui
{
    public static class DropdownUtility
    {
        public static DropdownSelect<string> CreateSimpleMenu(IReadOnlyList<string> buttons, Action<string> onItemClick)
        {
            return new DropdownSelect<string>(buttons.ToDictionary(x => x), null, onItemClick);
        }
    }
}
