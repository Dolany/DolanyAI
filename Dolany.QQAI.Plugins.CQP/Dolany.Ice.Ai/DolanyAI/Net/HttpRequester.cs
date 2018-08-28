﻿using System;
using System.Text;
using System.Net;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class HttpRequester : IDisposable
    {
        private WebClient Client { get; set; }

        public HttpRequester()
        {
            Client = new WebClient
            {
                Credentials = CredentialCache.DefaultCredentials
            };
        }

        public string Request(string url)
        {
            var pageData = Client.DownloadData(url);
            var pageHtml = Encoding.UTF8.GetString(pageData);

            return pageHtml;
        }

        public void Dispose()
        {
            ((IDisposable)Client).Dispose();
            GC.SuppressFinalize(this);
        }
    }
}