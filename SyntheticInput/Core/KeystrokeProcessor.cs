using System;
using System.Threading.Tasks;

namespace SyntheticInput.Core
{
    public class KeystrokeProcessor
    {
        // List of symbol characters --- MOVE THIS TO A JSON INSTEAD
        private char[] symbolCharacters = { '!', ':', '|', '(', ')', '{', '}', '"', '<', '>', '?', '_', '+', '@', '#', '$', '%', '^', '&', '*' };

        // Enums
        private enum CapsLockState
        {
            OFF = 0,
            ON = 1
        }

        private enum ToggleKeyState
        {
            Released = 0,
            Pressed = 1
        }

        // Variables
        private InputSettings _settings;

        public KeystrokeProcessor(InputSettings settings)
        {
            _settings = settings;
        }

        // Private methods
        private Task SetCapslockState(CapsLockState state)
        {
            // Get the current state of caps lock
            bool currentlyEnabled = (((ushort)Win32.GetKeyState(0x14)) & 0xffff) != 0;

            // Compare the requested state and the current state of caps lock
            // If the current state is the same as the requested state, return the function, otherwise continue
            switch (state)
            {
                case CapsLockState.ON:
                    if (currentlyEnabled)
                        return Task.CompletedTask;
                    break;
                case CapsLockState.OFF:
                    if (!currentlyEnabled)
                        return Task.CompletedTask;
                    break;
            }

            // Toggle the caps lock key
            Win32.keybd_event((int)VirtualKeys.CapsLock, (byte)Win32.MapVirtualKey((int)VirtualKeys.CapsLock, 0), Win32.KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            Win32.keybd_event((int)VirtualKeys.CapsLock, (byte)Win32.MapVirtualKey((int)VirtualKeys.CapsLock, 0), Win32.KEYEVENTF_EXTENDEDKEY | Win32.KEYEVENTF_KEYUP, (UIntPtr)0);

            return Task.CompletedTask;
        }

        private Task SetShiftState(ToggleKeyState state)
        {
            switch (state)
            {
                case ToggleKeyState.Pressed:
                    Win32.keybd_event((byte)VirtualKeys.LeftShift, (byte)Win32.MapVirtualKey((int)VirtualKeys.LeftShift, 0), Win32.KEYEVENTF_EXTENDEDKEY | 0, (UIntPtr)0);
                    break;
                case ToggleKeyState.Released:
                    Win32.keybd_event((byte)VirtualKeys.LeftShift, (byte)Win32.MapVirtualKey((int)VirtualKeys.LeftShift, 0), Win32.KEYEVENTF_KEYUP, (UIntPtr)0);
                    break;
            }
            return Task.CompletedTask;
        }

        private Task SetCtrlState(ToggleKeyState state)
        {
            switch (state)
            {
                case ToggleKeyState.Pressed:
                    Win32.keybd_event((byte)VirtualKeys.LeftControl, (byte)Win32.MapVirtualKey((int)VirtualKeys.LeftControl, 0), Win32.KEYEVENTF_EXTENDEDKEY | 0, (UIntPtr)0);
                    break;
                case ToggleKeyState.Released:
                    Win32.keybd_event((byte)VirtualKeys.LeftControl, (byte)Win32.MapVirtualKey((int)VirtualKeys.LeftControl, 0), Win32.KEYEVENTF_KEYUP, (UIntPtr)0);
                    break;
            }
            return Task.CompletedTask;
        }

        private Task<bool> IsSymbol(char c)
        {
            // Check if the character is a specific symbol
            // NOTE: In the future this should be checked via a list that's loaded from a json config file rather than hard-coding the characters
            bool isSymbol = false;
            foreach (var symbol in symbolCharacters)
            {
                if (c == symbol)
                {
                    isSymbol = true;
                    break;
                }
            }

            return Task.FromResult(isSymbol);
        }

        // Public methods
        /// <summary>
        /// Sends a WM_KEYDOWN command to the assigned process.
        /// </summary>
        /// <param name="key">VirtualKey which will be the keystroke.</param>
        public async Task SendKeystroke(VirtualKeys key)
        {
            // Send the character keystroke to the process
            Win32.PostMessage(_settings.Process.MainWindowHandle, Win32.WM_KEYDOWN, (int)key, 0);
            // Delay between keystrokes, ensures that any next input is after this one and not before (in case this method is called in a loop)
            await Task.Delay(_settings.DelayBetweenKeyPresses);
        }

        /// <summary>
        /// Sends a WM_KEYDOWN command to the assigned process.
        /// </summary>
        /// <param name="c">Character which will be turned into the keystore.</param>
        /// <param name="next">The next character in a loop (if available).</param>
        public async Task SendKeystroke(char c)
        {
            // Delay between keystrokes, ensures that any next input is after this one and not before (in case this method is called multiple times in a short timeframe)
            await Task.Delay(_settings.DelayBetweenKeyPresses);
            // Check if the character is capital and if it's a symbol
            bool isUpper = char.IsUpper(c);
            bool isSymbol = await IsSymbol(c);
            // Check if settings requires us to apply delay on use of control keys (caps lock, shift)
            bool delayControlKeys = _settings.MultiplyDelayOnCapsOrShift && (isSymbol || isUpper);
            // Assign our control key delay value and calculate using the multiplier if required
            int delayControlPress = _settings.DelayBetweenKeyPresses;
            if (delayControlKeys)
                delayControlPress = _settings.DelayBetweenKeyPresses * _settings.DelayMultiplier;
            // If the character is capital, request to turn on caps lock, otherwise turn it off
            if (isUpper)
                await SetCapslockState(CapsLockState.ON); else await SetCapslockState(CapsLockState.OFF);
            // If the character is symbol, press left shift to print the symbol
            if (isSymbol)
                await SetShiftState(ToggleKeyState.Pressed);
            // Delay for control keys (caps lock, shift) to take effect properly
            if (delayControlKeys)
                await Task.Delay(delayControlPress);
            // Send the character keystroke to the process
            Win32.PostMessage(_settings.Process.MainWindowHandle, Win32.WM_KEYDOWN, Win32.VkKeyScan(c), 0);
            // Delay for control keys (caps lock, shift) to take effect properly
            if (delayControlKeys)
                await Task.Delay(delayControlPress);
            // Release left shift
            if (isSymbol)
                await SetShiftState(ToggleKeyState.Released);
            // Turn off caps lock
            await SetCapslockState(CapsLockState.OFF);
            // Delay between keystrokes, ensures that any next input is after this one and not before (in case this method is called multiple times in a short timeframe)
            await Task.Delay(_settings.DelayBetweenKeyPresses);
        }

        /// <summary>
        /// Sends a WM_COPY command to the assigned process.
        /// </summary>
        public async Task Copy()
        {
            // Send the command
            Win32.PostMessage(_settings.Process.MainWindowHandle, Win32.WM_COPY, 0, 0);
            // Delay between keystrokes, ensures that any next input is after this one and not before (in case this method is called in a loop)
            await Task.Delay(_settings.DelayBetweenKeyPresses);
        }

        /// <summary>
        /// Sends a WM_PASTE command to the assigned process.
        /// </summary>
        public async Task Paste()
        {
            // Send the command
            Win32.PostMessage(_settings.Process.MainWindowHandle, Win32.WM_PASTE, 0, 0);
            // Delay between keystrokes, ensures that any next input is after this one and not before (in case this method is called in a loop)
            await Task.Delay(_settings.DelayBetweenKeyPresses);
        }
    }
}
