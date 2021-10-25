using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace swfd.core
{
    public class WorkflowBuilder: IWorkflowBuilder{
        public WorkflowBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services {get; private set;}

        public string Key {get; set;}
    }
}
