using System;

namespace swfd.core
{
    public class WorkflowStep{
        public WorkflowStep(string key, params Func<dynamic, string>[] ParameterFactories)
        {
            
        }
        public WorkflowStep(string key, Action<dynamic, string[]> OutputExtractor, params Func<dynamic, string>[] ParameterFactories)
        {
            
        }
    }
}
