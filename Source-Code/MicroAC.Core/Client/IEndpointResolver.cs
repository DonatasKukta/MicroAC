using System.Threading.Tasks;

namespace MicroAC.Core.Client
{
    public interface IEndpointResolver
    {
        void InitialiseEndpoints();

        string GetServiceEndpoint(MicroACServices service);

        string GetServiceEndpoint(string fabricService);
    }
}
