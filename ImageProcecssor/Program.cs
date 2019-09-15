using System;

namespace ImageProcecssor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***Welcome to the image processor***\n\n" +
                "Type command in the following format: [image, folder, etc.] command1 command2");
            Console.WriteLine("For getting commands info, type Commands");

            UserInput userInput = new UserInput();
            Processor processor = new Processor();

            while(true)
            {
                userInput.ProcessUserInput();
                if (userInput.Input.Equals("Exit"))
                {
                    break;
                }
                processor.ProceedImage(userInput.Commands, userInput.Images);
                userInput.RestartUserInput();
            }
        }
    }
}