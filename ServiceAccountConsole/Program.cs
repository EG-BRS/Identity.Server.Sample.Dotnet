using System;
using System.Threading.Tasks;

namespace ClientCredentialsConsole
{
    public class Credentials
    {
        public string ClientId { get; private set; }
        public string Secret { get; private set; }
        public string Apis { get; private set; }
        public string IdentityEndPoint { get; private set; }
        public static Credentials Build()
        {
            return new Credentials()
            {
                ClientId = "ef2e55b8-dab8-4a7b-9f7e-5ce4351803ca.xena.biz",
                Secret = "ab304272-417c-4709-b963-31b1708b8d51",
                Apis = "testapi",
                IdentityEndPoint = "http://localhost:5000/"
            };
        }
    }

    public class Program
    {
        static async Task Main(string[] args)
        {
            var tokenConnection = await ClientCredentialsTokenService.CreateAsync(Credentials.Build());

            if (tokenConnection == null)
            {
                Console.WriteLine("failed to get token service.");
            }
            else
            {
                var token = await tokenConnection.RequestToken();
                Console.WriteLine(token.ToString());
            }
            Console.WriteLine("Press esc to exit.");
            Console.ReadLine();
        }
    }
}