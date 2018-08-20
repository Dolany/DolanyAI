﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
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
            var pageData = Client.DownloadData(url);
            var pageHtml = Encoding.UTF8.GetString(pageData);

            return pageHtml;
        }
    }
}