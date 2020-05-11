using System;


namespace R5T.Kefalonia.XElements
{
    public static class PropertyGroupXElementExtensions
    {
        public static ProjectXElement GetProjectXElement(this PropertyGroupXElement propertyGroupXElement)
        {
            var projectXElement = propertyGroupXElement.Value.Parent.AsProject();
            return projectXElement;
        }
    }
}
