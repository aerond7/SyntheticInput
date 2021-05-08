using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SyntheticInput.Core;
using SyntheticInput.Exceptions;

namespace SyntheticInput
{
    public class SyntheticInputManager
    {
        // Class variables
        private InputSettings _settings;
        private KeystrokeProcessor _keystrokeProcessor;

        // Properties
        public InputSettings Settings { get { return _settings; } }

        // Constructors
        public SyntheticInputManager(InputSettings settings)
        {
            if (settings.Process == null)
                throw new ArgumentException("Process property in InputSettings is null.");
            _settings = settings;
            _keystrokeProcessor = new KeystrokeProcessor(_settings);
        }

        // Public methods
        public async Task WriteString(string content)
        {
            // Go through each character in our string and let the KeystrokeProcessor process it
            foreach (char c in content)
            {
                // Send the keystroke
                await _keystrokeProcessor.SendKeystroke(c);
            }
        }

        public async Task SendKeystroke(VirtualKeys key)
        {
            // Send the keystroke
            await _keystrokeProcessor.SendKeystroke(key);
        }
    }
}
