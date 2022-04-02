using System.Threading.Tasks;

namespace MicroAC.Core.Client
{
    public interface IEndpointResolver
    {
        Task<string> GetServiceEndpoint(MicroACServices service);
    }
}
