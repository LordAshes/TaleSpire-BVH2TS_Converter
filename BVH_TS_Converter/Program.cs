using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVH_TS_Converter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Variables to hold the current file source and the mode
            string sourceFile = "";
            string mode = "";

            // Check for command line input
            if(args.Count()==1)
            {
                // Single input is assumed to be teh source file using the default skeleton
                sourceFile = args[0];
                if(System.IO.File.Exists("Minimal.txt")) { mode = "Minimal"; } else { Console.WriteLine("Skeleton 'Minimal' does not exist (requires file 'Minimal.txt')"); return; }
            }
            else if (args.Count() == 2)
            {
                // Two inputs are assumed to be the source file followed by the skeleton name from the Bones.Json list
                sourceFile = args[0];
                if (System.IO.File.Exists(args[1] + ".txt")) { mode = args[1]; } else { Console.WriteLine("Skeleton '" + args[1] + "' does not exist (requires file '" + args[1] + ".txt')"); return; }
            }
            else
            {
                // Provied proper syntax if the parameter count as not one or two
                Console.WriteLine();
                Console.WriteLine("Error: Invalid Number Of Parameters");
                Console.WriteLine();
                Console.WriteLine("Syntax:");
                Console.WriteLine();
                Console.WriteLine("BVH_TS_Converter \"D:/Data/Animation.bvh\"");
                Console.WriteLine("BVH_TS_Converter \"D:/Data/Animation.bvh\" Full");
                Console.WriteLine();
                Console.WriteLine("Where the first parameter is the path and file name of the BVH file to be converterd");
                Console.WriteLine("Where the second optional parameter is the name of a skeleton (defined in the Name.txt file) to use. Default is Minimal if it exists.");
                Console.WriteLine();
            }

            // Summarize request
            Console.WriteLine("Processing '" + sourceFile + "' using '" + mode + "' Skeleton structure");

            string[] inclusionsList = System.IO.File.ReadAllLines(mode + ".txt");

            // Read all the available bones in the BVH file. This is necessary, even if the skeleton used has less bones, becasuse that BVH data is based on the bones in the BVH skeleton
            String[] contents = System.IO.File.ReadAllLines(sourceFile);
            List<string> bones = new List<string>();
            foreach(string content in contents)
            {
                string entry = content.Trim();
                if(entry.StartsWith("ROOT") || entry.StartsWith("JOINT"))
                {
                    bones.Add(entry.Substring(entry.IndexOf(" ") + 1).Trim());
                }
                if (entry.StartsWith("MOTION")) { break; }
            }
            // Reread the data as a single string to easily extract the data section
            String data = System.IO.File.ReadAllText(sourceFile).Replace("\r\n","\n");
            // Cut to the motion section
            data = data.Substring(data.IndexOf("MOTION")+"MOTION".Length);
            // Cut past the frames
            data = data.Substring(data.IndexOf("Frames: ") + "Frames: ".Length);
            int frames = int.Parse(data.Substring(0, data.IndexOf("\n")));
            // Cut past the frame time
            data = data.Substring(data.IndexOf("Frame"));
            data = data.Substring(data.IndexOf("\n")+1);
            // Determine output file - Same as input file but with a JSON extension
            string fileName = ((System.IO.Path.GetDirectoryName(sourceFile)!="") ? System.IO.Path.GetDirectoryName(sourceFile)  : Environment.CurrentDirectory)+ "/" + System.IO.Path.GetFileNameWithoutExtension(sourceFile) + ".json";
            // Split out the change data
            string[] changes = data.Replace("\n","").Split(' ');
            // Start the JSON structure
            System.IO.File.WriteAllText(fileName, "{\r\n");
            // Variables for holding the trailing ending (in most cases a comma except for the last lines in a list)
            string suffix = "";
            string subsuffix = "";
            // Pointer into the changes array
            int pnt = 3;
            // Process each frame
            for (int frame = 0; frame < frames; frame++)
            {
                Console.WriteLine("Processing Frame " + frame+" of "+frames+"...");
                if (frame != (frames - 1)) { suffix = ","; } else { suffix = ""; }
                // Append the JSON frame structure
                System.IO.File.AppendAllText(fileName, "  \"" + frame + "\":\r\n");
                System.IO.File.AppendAllText(fileName, "  {\r\n");
                // Process each bone
                for(int b=0; b<bones.Count; b++)
                {
                    if (b != (bones.Count - 1)) { subsuffix = ","; } else { subsuffix = ""; }
                    // Write out each bone that is on the including list
                    if (inclusionsList.Contains(bones[b]))
                    {
                        System.IO.File.AppendAllText(fileName, "    \"" + (bones[b] + "\":                         ").Substring(0, 25) + " {\"character\": \"{General}\", \"bone\": \"" + bones[b] + "\", \"target\": {\"ax\": " + changes[pnt + 3] + ", \"ay\": " + (float.Parse(changes[pnt + 4]) * -1) + ", \"az\": " + (float.Parse(changes[pnt + 5]) * -1) + ", \"px\": \"NaN\", \"py\": \"NaN\", \"pz\": \"NaN\"}}" + subsuffix + "\r\n");
                    }
                    // Advance the pointer into the change array by 6 (each bone uses 6 values: px, py, pz, ax, ay, and az) regardless if the bone was written
                    pnt = pnt + 3;
                }
                // Append the JSON frame end structure
                System.IO.File.AppendAllText(fileName, "  }"+suffix+"\r\n");
            }
            // Append the JSON end structure
            System.IO.File.AppendAllText(fileName, "}" + suffix + "\r\n");
        }
    }
}
