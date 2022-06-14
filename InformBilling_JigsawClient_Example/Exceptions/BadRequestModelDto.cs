namespace InformBilling_JigsawClient_Example.Exceptions
{
    /// <summary>
    /// This Dto Mimics what is returned by ASP.NET when a model fails validation based on Atttributes on the Model being passed.
    /// You should return this object when returning your own custom validation errors so the user gets a consistent model with their BadRequest.
    /// type, title, status, traceid are all hard coded values.
    /// You should build up the array of errors to return your self.
    /// </summary>
    public class BadRequestModelDto
    {
        public string type { get { return "https://tools.ietf.org/html/rfc7231#section-6.5.1"; } }
        public string title { get { return "One or more validation errors occurred."; } }
        public int status { get { return 400; } }
        public string traceid { get { return ""; } }
        public Dictionary<string, List<string>> errors { get; set; }

        public BadRequestModelDto()
        {
            errors = new Dictionary<string, List<string>>();
        }
    }

}
