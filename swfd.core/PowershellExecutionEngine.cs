using System;
using System.Management.Automation;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace swfd.core
{
    public class PowershellExecutionEngine
    {
        public ILogger<PowershellExecutionEngine> _logger { get; }

        public PowershellExecutionEngine(ILogger<PowershellExecutionEngine> logger)
        {
            _logger = logger;
        }

        public bool Execute(string command, WorkflowPayload payload, params Func<dynamic, string>[] parameterFactory)
        {
            using (PowerShell powershell= PowerShell.Create())
            {
                powershell.AddScript(command);
                for(int i = 0; i < parameterFactory.Length; i++)
                {
                    var param = parameterFactory[i];
                    powershell.AddParameter($"Param{i}", param(payload.PayloadData));
                }

                var psOutput = powershell.Invoke();

                if (powershell.Streams.Error.Any())
                {
                    return false;
                }
                
                psOutput.Select(x => x.ToString()).ToArray();
                foreach (PSObject output in psOutput)
                {
                    if (output != null)
                    {
                        output.ToString();
                    }
                }
                return true;
            }
        }
    }
}