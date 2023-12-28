using System.Net;
using System.Text.RegularExpressions;

namespace Common
{
    public class AoCUtil
    {
        private HttpClient Http;
        private HttpClientHandler Handler;
        private CookieContainer Cookies;
        public AoCUtil()
        {
            Cookies = new CookieContainer();
            Handler = new HttpClientHandler() { CookieContainer = Cookies };
            Http = new HttpClient(Handler);
            string sessionCookie = File.ReadAllLines("../../../../.sessioncookie")[0].Trim();
            Cookies.Add(new Uri("https://adventofcode.com"), new Cookie("session", sessionCookie));

        }

        public string[] GetInput(int year, int day)
        {
            string fileName = "input.txt";
            if (!File.Exists(fileName))
            {

                Task<string> download = Http.GetStringAsync($"https://adventofcode.com/{year}/day/{day}/input");
                if (!download.Wait(10 * 1000))
                {
                    throw new Exception("Download didn't succeed within 10 seconds");
                }
                File.WriteAllText(fileName, download.Result);
            }
            return File.ReadAllLines(fileName);
        }

        public string[] GetTestBlock(int year, int day, int block)
        {
            string fileName = $"test-input-{block}.txt";
            if (!File.Exists(fileName))
            {

                Task<string> download = Http.GetStringAsync($"https://adventofcode.com/{year}/day/{day}");
                if (!download.Wait(10 * 1000))
                {
                    throw new Exception("Download didn't succeed within 10 seconds");
                }
                string html = download.Result;
                Regex codeblockRE = new Regex("<pre><code>(.*?)</code></pre>", RegexOptions.Singleline);
                MatchCollection matches = codeblockRE.Matches(html);
                File.WriteAllText(fileName, matches[block].Groups[1].Value);
            }
            return File.ReadAllLines(fileName);
        }


    }
}
