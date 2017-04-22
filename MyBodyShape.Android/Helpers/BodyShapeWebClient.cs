/**********************************************************/
/*************** The custom bodyshape web client.
/**********************************************************/

using System;
using System.Net;

namespace MyBodyShape.Android.Helpers
{
    /// <summary>
    /// The bodyshape web client.
    /// </summary>
    public class BodyShapeWebClient : WebClient
    {
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = Timeout;
            return request;
        }
    }
}