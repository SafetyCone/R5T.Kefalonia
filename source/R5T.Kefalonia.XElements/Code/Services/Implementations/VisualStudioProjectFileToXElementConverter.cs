using System;
using System.Linq;
using System.Xml.Linq;

using R5T.Gloucester.Types;
using R5T.VisualStudioProjectFileStuff;


namespace R5T.Kefalonia.XElements
{
    public class VisualStudioProjectFileToXElementConverter : IVisualStudioProjectFileToXElementConverter
    {
        public ProjectFile ToProjectFile(ProjectXElement projectXElement)
        {
            var hasPropertyGroup = projectXElement.Value.Elements()
                .Where(x => x.Name == ProjectFileNodeName.PropertyGroup)
                .Any();


            throw new NotImplementedException();
        }

        public ProjectXElement ToProjectXElement(ProjectFile projectFile)
        {
            throw new NotImplementedException();
        }
    }
}
