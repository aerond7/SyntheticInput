using System;
using System.Threading.Tasks;
using SyntheticInput;
using SyntheticInput.Core;

namespace SyntheticInputTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            InputTester();
            Console.ReadLine();
        }

        static async void InputTester()
        {
            var input = new SyntheticInputManager(new InputSettings
            {
                Process = ProcessHelper.GetProcessByName("TheIsleClient-Win64-Shipping")
            });

            Console.WriteLine("Input string:");
            string str = Console.ReadLine();
            await input.WriteString(str);

            await Task.Delay(1000);

            await input.SendKeystroke(VirtualKeys.Return);
        }
    }
}
