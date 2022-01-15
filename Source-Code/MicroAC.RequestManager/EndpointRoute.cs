namespace MicroAC.RequestManager
{
    public class EndpointRoute
    {
        public string ExternalRoute { get; set; }

        public string InternalRoute { get; set; }

        public bool RequiresAuhtentication { get; set; }

        public bool RequiresAuthorization { get; set; }
    }
}
