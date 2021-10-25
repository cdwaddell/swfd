using System;
using Xunit;
using swfd.core;

namespace swfd.test
{
    public class PowershellExecutionEngine_Tests
    {
        [Fact]
        public void TestPowershellExecute()
        {
            var logger = new TestLogger<PowershellExecutionEngine>();
            var engine = new PowershellExecutionEngine(logger);
            var output = engine.Execute("param($param1) $d = get-date; $s = 'test string value'; $d; $s; $param1;", new WorkflowPayload(), x => "Some Value");
        }
    }
}