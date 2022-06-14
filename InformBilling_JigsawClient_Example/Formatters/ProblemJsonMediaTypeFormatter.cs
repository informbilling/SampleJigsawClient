using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace InformBilling_JigsawClient_Example.Formatters
{
    public class ProblemJsonMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public ProblemJsonMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/problem+json"));
        }
    }
}
