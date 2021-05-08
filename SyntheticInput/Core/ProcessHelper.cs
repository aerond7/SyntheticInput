using SyntheticInput.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SyntheticInput.Core
{
    public static class ProcessHelper
    {
        // Static methods
        public static Process GetProcessByName(string processName)
        {
            string actualName = processName.Replace(".exe", "");
            var p = Process.GetProcessesByName(actualName);
            if (p.Length < 1)
                throw new NoProcessException($"No process with name '{actualName}' was found.");
            if (p.Length > 1)
                throw new MultipleProcessesException($"There are multiple processes that match the process name '{actualName}', this is not supported.");
            return p[0];
        }
    }
}
