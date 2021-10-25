using Microsoft.Extensions.DependencyInjection;

namespace swfd.core
{
    public interface IWorkflowBuilder
    {
        public IServiceCollection Services {get;}
        public string Key {get; set;}
    }
}
