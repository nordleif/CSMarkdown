using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Hosting
{
    //Nicholai Axelgaard
    class Reports
    {
        public string Name { get; set; }
        public HashSet<string> Files { get; set; }
        public Dictionary<string, Reports> Folders { get; set; }

        public Reports(string name = "")
        {
            Folders = new Dictionary<string, Reports>();
            Files = new HashSet<string>();
            Name = name;
        }

        public Reports AddToCollection(string filePath)
        {
            int i = filePath.IndexOf('/');
            if (i > -1)
            {
                Reports dir = AddToCollection(filePath.Substring(0, i));
                return dir.AddToCollection(filePath.Substring(i + 1));
            }

            if (filePath == "") return this;

            if (filePath.EndsWith(".smd"))
            {
                Files.Add(filePath);
                return this;
            }

            Reports subFolder;
            if (Folders.ContainsKey(filePath))
            {
                subFolder = Folders[filePath];
            }
            else
            {
                subFolder = new Reports(filePath);
                Folders.Add(filePath, subFolder);
            }
            return subFolder;
        }
    }
}
