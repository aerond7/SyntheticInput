using System;
using System.Threading.Tasks;
using SyntheticInput.Core;

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
        /// <summary>
        /// Writes an entire string.
        /// </summary>
        /// <param name="content">The string to be written.</param>
        public async Task WriteString(string content)
        {
            // Go through each character in our string and let the KeystrokeProcessor process it
            for (int i = 0; i < content.Length; i++)
            {
                // Send the keystroke
                await _keystrokeProcessor.SendKeystroke(content[i]);
            }
        }

        /// <summary>
        /// Sends a single keystroke.
        /// </summary>
        /// <param name="key">The keystroke to be sent.</param>
        public async Task SendKeystroke(VirtualKeys key)
        {
            // Send the keystroke
            await _keystrokeProcessor.SendKeystroke(key);
        }

        /// <summary>
        /// Sends CTRL + C input to the assigned process.
        /// </summary>
        public async Task CopyToClipboard()
        {
            await _keystrokeProcessor.Copy();
        }

        /// <summary>
        /// Sends CTRL + V input to the assigned process.
        /// </summary>
        public async Task PasteFromClipboard()
        {
            await _keystrokeProcessor.Paste();
        }
    }
}
