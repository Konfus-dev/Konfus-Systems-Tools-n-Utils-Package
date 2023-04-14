using System;

namespace Konfus.Tools.Graph_Editor.Editor.Attributes
{
    /// <summary>
    /// Attribute that can be attached to a class that inherits from EdgeDropMenu to change & customize the edge drop context menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomEdgeDropMenuAttribute : Attribute
    {
        public CustomEdgeDropMenuAttribute()
        {
        }
    }
}