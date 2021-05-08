using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SyntheticInput.Core
{
    public class KeystrokeProcessor
    {
        // Enums
        private enum CapsLockState
        {
            OFF = 0,
            ON = 1
        }

        private enum ShiftState
        {
            Released = 0,
            Pressed = 1
        }

        // Windows API
        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEYEVENTF_KEYUP = 0x2;
        const UInt32 WM_KEYDOWN = 0x0100;
        const UInt32 WM_KEYUP = 0x0101;
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);
        [DllImport("user32.dll")]
        static extern int MapVirtualKey(int uCode, uint uMapType);

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
            bool currentlyEnabled = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;

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
            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);

            return Task.CompletedTask;
        }

        private Task SetShiftState(ShiftState state)
        {
            switch (state)
            {
                case ShiftState.Pressed:
                    keybd_event((byte)VirtualKeys.LeftShift, (byte)MapVirtualKey((int)VirtualKeys.LeftShift, 0), KEYEVENTF_EXTENDEDKEY | 0, (UIntPtr)0);
                    break;
                case ShiftState.Released:
                    keybd_event((byte)VirtualKeys.LeftShift, (byte)MapVirtualKey((int)VirtualKeys.LeftShift, 0), KEYEVENTF_KEYUP, (UIntPtr)0);
                    break;
            }
            return Task.CompletedTask;
        }

        private Task<bool> IsSymbol(char c)
        {
            return Task.FromResult(
                c == '!' ||
                c == ':' ||
                c == '|');
        }

        // Public methods
        public async Task ProcessKeystroke(char c)
        {
            // Check if the character is capital and if it's a symbol
            bool isUpper = char.IsUpper(c);
            bool isSymbol = await IsSymbol(c);
            // If the character is capital, request to turn on caps lock, otherwise turn it off
            if (isUpper)
                await SetCapslockState(CapsLockState.ON);
            else
                await SetCapslockState(CapsLockState.OFF);
            // If the character is symbol, press left shift to print the symbol
            if (isSymbol)
                await SetShiftState(ShiftState.Pressed);
            // Send the character keystroke to the process
            PostMessage(_settings.Process.MainWindowHandle, WM_KEYDOWN, VkKeyScan(c), 0);
            // Release left shift
            if (isSymbol)
                await SetShiftState(ShiftState.Released);
            // Turn off caps lock
            await SetCapslockState(CapsLockState.OFF);
            // Delay between keystrokes, ensures that the string is written out correctly as it was requested
            await Task.Delay(_settings.DelayBetweenKeyPresses);
        }
    }
}
