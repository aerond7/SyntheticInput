using System.Diagnostics;

namespace SyntheticInput
{
    public class InputSettings
    {
        /// <summary>
        /// Target process to which the input manager should send keystrokes.
        /// </summary>
        public Process Process { get; set; } = null;
        /// <summary>
        /// Delay between individual keystrokes.
        /// </summary>
        public int DelayBetweenKeyPresses { get; set; } = 1;
        /// <summary>
        /// Specifies whether DelayBetweenKeyPresses should multiply by DelayMultiplier if a character is a symbol or is capital, requires DelayMultiplier to be set.
        /// </summary>
        public bool MultiplyDelayOnCapsOrShift { get; set; } = false;
        /// <summary>
        /// Value by which DelayBetweenKeyPresses will be multiplied if MultiplyDelayOnCapsOrShift is set to true.
        /// </summary>
        public int DelayMultiplier { get; set; } = 1;
    }
}
