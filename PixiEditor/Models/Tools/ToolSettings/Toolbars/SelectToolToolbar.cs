﻿using PixiEditor.Models.Tools.ToolSettings.Settings;

namespace PixiEditor.Models.Tools.ToolSettings.Toolbars
{
    public class SelectToolToolbar : Toolbar
    {
        public SelectToolToolbar()
        {
            Settings.Add(new DropdownSetting("SelectMode", new[] { "New", "Add", "Subtract" }, "Selection type"));
        }
    }
}