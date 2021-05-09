using System;
using SyntheticInput;
using SyntheticInput.Core;
using System.Windows.Forms;

namespace SyntheticInputTesting
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Type the name of the process to hook into:");
            string procName = Console.ReadLine();
            do
            {
                InputTester(procName);
            } while (true);
        }

        static void InputTester(string procName)
        {
            var input = new SyntheticInputManager(new InputSettings
            {
                Process = ProcessHelper.GetProcessByName(procName),
                DelayBetweenKeyPresses = 1,
                MultiplyDelayOnCapsOrShift = true,
                DelayMultiplier = 2
            });
            Console.WriteLine("String written to the process:");
            string str = Console.ReadLine();
            //Clipboard.SetText(str);
            input.WriteString(str);
        }
    }
}
