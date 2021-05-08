using System;
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

            await input.Write("/announce Welcome to DINO WORLD! | Join our Discord: https://discord.gg/dinoworld");
        }
    }
}
