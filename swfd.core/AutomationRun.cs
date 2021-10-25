using System;
using System.Collections.Generic;

namespace swfd.core
{
    public class AutomationRun
    {
        public Guid Id { get; set; }
        public dynamic Payload { get; set; }
        public Dictionary<string, AutomationStepStatus> StepStatuses {get;set;}
    }

}