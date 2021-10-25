using System.Collections.Generic;

namespace swfd.core
{
    public class WorkflowPayload
    {
        public string PayloadKey { get; set; }
        public Dictionary<string, List<StepStatus>> StepStatuses { get; set; }
        public dynamic PayloadData { get; set; }

    }
}