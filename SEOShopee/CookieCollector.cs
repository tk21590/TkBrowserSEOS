using CefSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEOShopee
{
    public class CookieCollector : ICookieVisitor
    {
        private readonly TaskCompletionSource<List<Cookie>> _source = new TaskCompletionSource<List<Cookie>>();

        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            _cookies.Add(cookie);

            if (count == (total - 1))
            {
                _source.SetResult(_cookies);
            }
            return true;
        }


        public Task<List<Cookie>> Task => _source.Task;

        public static string GetCookieJson(List<Cookie> cookies)
        {

            StringBuilder cookieString = new StringBuilder();
            string delimiter = string.Empty;
            return JsonConvert.SerializeObject(cookies);
        }
        public static string GetCookieString(List<Cookie> cookies)
        {

            StringBuilder cookieString = new StringBuilder();
            string delimiter = string.Empty;
            foreach (var cookie in cookies)
            {

                //if (cookie.Name == "session-key-www")
                //{
                cookieString.Append(delimiter);
                cookieString.Append(cookie.Name);
                cookieString.Append('=');
                cookieString.Append(cookie.Value);
                delimiter = "; ";
                //}
                cookieString.Append(cookieString);
            }
            return cookieString.ToString();
        }
        private readonly List<Cookie> _cookies = new List<Cookie>();
        public void Dispose()
        {
        }
    }

}
