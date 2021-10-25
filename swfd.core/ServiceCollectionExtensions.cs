using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace swfd.core
{
    public static class ServiceCollectionExtensions{
        public static IWorkflowBuilder AddSwfd(this IServiceCollection services)
        {
            return new WorkflowBuilder(services);
        }
        
        public static IWorkflowStepBuilder AddWorkflow(this IWorkflowBuilder workflow, string key, string scriptfolder, string inputfolder)
        {
            workflow.Key = key;

            Func<IServiceProvider, IHostedService> factory = x => {
                var runner = x.GetService<ScriptRunningService>();

                runner.Key = key;
                runner.Steps = x.GetServices<IWorkflowStepCollection>().Where(x => x.Key == key).ToList();
                runner.ScriptFolder = scriptfolder;
                runner.InputFolder = inputfolder;

                return runner;
            };

            workflow.Services.AddSingleton<IHostedService>(factory);
            return new WorkflowStepBuilder(workflow);
        }

        public static IWorkflowStepBuilder AddSteps(this IWorkflowStepBuilder workflow, params WorkflowStep[] steps)
        {
            workflow.Services.AddSingleton<IWorkflowStepCollection>(x => new WorkflowStepCollection(workflow.Key, steps));
            return workflow;
        }
    }
}
