using AES;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            AESHelper helper = new AESHelper("45s.7z", "");
          //helper.EncryptFile();
            helper.DecryptFile();
            Console.ReadLine();
        }
    }
}
