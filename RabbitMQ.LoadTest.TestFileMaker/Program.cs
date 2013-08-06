using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RabbitMQ.LoadTest.TestFileMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get text to put into files.
            System.IO.StreamReader textFile =
               new System.IO.StreamReader("Text.txt");
            string textString = textFile.ReadToEnd();

            textFile.Close();

            //Create test files directory if it does not exist.
            string testfilesPath = "testfiles";

            bool isExists = System.IO.Directory.Exists(testfilesPath);

            if (!isExists)
                System.IO.Directory.CreateDirectory(testfilesPath);
            

            // Read the lengths file and put textstring section into files.
            int counter = 0;
            string length;

            System.IO.StreamReader lengthsFile =
               new System.IO.StreamReader("MessageLengths.txt");
            while ((length = lengthsFile.ReadLine()) != null)
            {
                System.IO.StreamWriter outFile = new System.IO.StreamWriter(".\\testfiles\\testfile_" + counter.ToString() + ".txt");
                outFile.Write(textString.Substring(0,Convert.ToInt32(length)));
                outFile.Close();
                counter++;
            }

            lengthsFile.Close();
            
        }
    }
}
