using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.CQP.DolanyAI
{
    public class HttpRequester
    {
        private WebClient Client = new WebClient();

        public HttpRequester()
        {
            Client.Credentials = CredentialCache.DefaultCredentials;
        }

        public string Request(string url)
        {
            Byte[] pageData = Client.DownloadData(url);
            string pageHtml = Encoding.UTF8.GetString(pageData);

            return pageHtml;
        }
    }
}