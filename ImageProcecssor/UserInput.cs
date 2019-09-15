using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImageProcecssor
{
    public class UserInput
    {
        public string Input { get; private set; } = String.Empty;
        public string[] Commands { get; private set; }
        public List<string> Images { get; private set; } = new List<string>();
            
        public UserInput() { }

        public void ProcessUserInput()
        {
            while (!IsInputValid())
            {
                Input = GetUsersInput();
            }
        }

        public void RestartUserInput()
        {
            Input = String.Empty;
        }

        private void WriteCommands()
        {
            Console.WriteLine("\nCommands: ");
            foreach(string s in CommandsList.readCommands)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine();
        }

        private string GetUsersInput()
        {
            Console.WriteLine("Enter your command");
            return Console.ReadLine();
        }

        private bool AreCommandsValid()
        {
            string[] cmds = ParseCommands();
            foreach(string command in cmds)
            {
                if (!CommandsList.readCommands.Contains(Input.Split().Last().ToLower(), StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"There is no \"{Input.Split().Last()}\" command option\n" +
                        $"Type Commands to check commands options");
                    return false;
                }
            }
            Commands = cmds;
            return true;
        }

        private void GetAllImagesFromFolder(string path)
        {
            string[] files = Directory.GetFiles(path);
            int numberOfImagesInDirectory = 0;
            foreach(string file in files)
            {
                if (file.EndsWith(".jpg") || file.EndsWith(".png"))
                {
                    Images.Add(file);
                    numberOfImagesInDirectory++;
                }
            }
            if (numberOfImagesInDirectory == 0)
            {
                Console.WriteLine($"There are no image files in {path}");
            }
        }

        private bool IsFolder(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    GetAllImagesFromFolder(path);
                }
                return attr.HasFlag(FileAttributes.Directory);
            }
            catch(FileNotFoundException)
            {
                return false;
            }
        }

        private bool IsImage(string path)
        {
            if(!File.Exists(path))
            {
                return false;
            }
            if(!(path.EndsWith(".jpg") || path.EndsWith(".png")))
            {
                return false;
            }
            Images.Add(path);
            return true;
        }

        private string[] ParseCommands()
        {
            string commands = Input.Split(']')[1].Trim().ToLower();
            return commands.Split(' ');
        }

        private string[] ParsePaths()
        {
            Regex reg = new Regex(@"\[.*\]");
            string fileOrFolderPaths = reg.Match(Input).Value.Replace("[","").Replace("]","");
            return fileOrFolderPaths.Split(',');
        }

        private bool IsSyntaxCorrect()
        {
            Regex inputSyntax = new Regex(@"\[.*\][\s*\w*]*");
            return inputSyntax.IsMatch(Input);
        }

        private bool AreBracketsEmpty()
        {
            string parsed = Input.Split(']')[0].Replace("[", "");
            return parsed.Equals("");
        }

        private bool UserWantsCompareDurations()
        {
            return Commands.Contains("comparedurations");
        }

        private bool UserWantsExit()
        {
            return Input.Trim().ToLower().Equals("exit");
        }

        private bool UserWantsCommands()
        {
            return Input.Trim().ToLower().Equals("commands");
        }

        private bool IsPathOrImageFile()
        {
            foreach (string path in ParsePaths())
            {
                string trimmedPath = path.Trim();
                if (!(IsFolder(trimmedPath) || IsImage(trimmedPath)))
                {
                    Console.WriteLine($"{trimmedPath} is not folder nor image file");
                    return false;
                }
            }
            return true;
        }

        private bool IsInputValid()
        {
            if (String.IsNullOrEmpty(Input))
            {
                return false;
            }

            if (UserWantsExit())
            {
                Input = "Exit";
                return true;
            }

            if (UserWantsCommands())
            {
                WriteCommands();
                return false;
            }

            if (!IsSyntaxCorrect())
            {
                Console.WriteLine("Input does not match required syntax");
                return false;
            }

            if (AreBracketsEmpty())
            {
                Console.WriteLine("There are no files or folders to proceed");
                return false;
            }

            if (!AreCommandsValid() || !IsPathOrImageFile())
            {
                return false;
            }

            if(UserWantsCompareDurations() && Commands.Length != 1)
            {
                Console.WriteLine("If you want to compare durations of LockBit and Get/SetPixel" +
                    " methods via CompareDurations command, other commands are forbidden!");
                return false;
            }

            if (Images.Count == 0)
            {
                Console.WriteLine("There are no image files to proceed!");
                return false;
            }
            return true;
        }
    }
}