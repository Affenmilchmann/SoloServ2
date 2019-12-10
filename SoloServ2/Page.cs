using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloServ2
{
    class Page
    {
        public string data;

        public Page() { }
        public void LoadFromFile(string fileName)
        {
            if (File.Exists(fileName)) data = File.ReadAllText(fileName);
            else data = "PAGE_DOES_NOT_EXISTS";
        }
    }
}
