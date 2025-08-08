using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models.RMS_Post
{
    public class RMSClient
    {
        private readonly object _apiDataLock = new object();
        public HttpClient HttpClient { get; set; }
        //  public HttpListener HttpListener { get; set; }
        public bool IsRunning { get; set; }
        public string ServerStatus { get; set; }
        public string Log { get; set; }
        public string LastRequestResponse { get; set; }
        public string ApiDataStatus { get; set; }
        public string ApiResponseData { get; set; }



        public RMSClient()
        {
           HttpClient = new HttpClient();
        }



    }

   



}
