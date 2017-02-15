using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebClientEx
{
    class WebClientProgram
    {
        static void Main(string[] args)
        {
            // Create web client.
            using (WebClient client = new WebClient())
            {
                // Download string.
                //string value = client.DownloadString("http://en.wikipedia.org/");
                string value = client.DownloadString("http://www.vecka.nu/");
                // Write values.
                Console.WriteLine("--- WebClient result ---");
                Console.WriteLine(value.Length);
                Console.WriteLine(value);
            }
            Console.ReadKey();

        }
    }
}
