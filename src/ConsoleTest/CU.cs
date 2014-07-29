using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleTest
{
    class CU
    {
        public static void Write2File(string fileFullPath, string content)
        {
            if (File.Exists(fileFullPath))
                using (File.Create(fileFullPath)) { }

            using (FileStream fs = new FileStream(fileFullPath, FileMode.Append))
            using (StreamWriter sw = new StreamWriter(fs))
                sw.WriteLine(content);
        }
    }
}
