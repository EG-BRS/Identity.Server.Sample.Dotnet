using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace HybridMVC.Core.Extensions
{
    public static class StringExtensions
    {
        public static StringContent AsJson(this object o)
        {
            try
            {
                return new StringContent(JsonConvert.SerializeObject(o, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }), Encoding.UTF8, "application/json");
            }
            catch (Exception)
            {
                return new StringContent("unable to parse json");
            }
        }
    }
}
