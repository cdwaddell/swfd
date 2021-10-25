using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace swfd.core
{
    public class WorkflowStepBuilder: IWorkflowStepBuilder{
        private readonly IWorkflowBuilder _workflow;

        public WorkflowStepBuilder(IWorkflowBuilder workflow)
        {
            _workflow = workflow;
        }

        public IServiceCollection Services => _workflow.Services;

        public string Key { get => _workflow.Key; set => _workflow.Key = value; }
    }

    public interface IWorkflowStepCollection{
        string Key {get;}
        IEnumerable<WorkflowStep> Steps { get; }
    }

    public class WorkflowStepCollection: IWorkflowStepCollection{
        public string Key {get; private set;}
        public IEnumerable<WorkflowStep> Steps { get; private set; }

        public WorkflowStepCollection(string key, IEnumerable<WorkflowStep> steps)
        {
            Key = key;
            Steps = steps;
        }
    }
}
