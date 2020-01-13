using System.Net;

namespace WaterMangoApp.Model.HttpModels
{
    public class ResponseStatusInfoModel
    {
        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}