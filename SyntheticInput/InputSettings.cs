using SyntheticInput.Exceptions;
using System;
using System.Diagnostics;

namespace SyntheticInput
{
    public class InputSettings
    {
        public Process Process { get; set; } = null;
        public int DelayBetweenKeyPresses { get; set; } = 1;
    }
}
