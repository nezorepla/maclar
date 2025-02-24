using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace maclar_consol
{
    class Program
    {
       
         string tt = "MACID,HOME,HOMEID,AWAY,AWAYID,C1,TM,DT,MS_HA,MS_HY,KOD,IY_HA,IY_HY,MBS,HANDIKAP_H,HANDIKAP_A,MS1,MSX,MS2,CS1X,CS12,CS2X,ALT,UST,C2,C3,LIG,LIGID,LIGID2,TG01,TG23,TG46,TG7P,IY1,IYX,IY2,H1,HX,H2,KGVAR,KGYOK,C4,IY15ALT,IY15UST,MS15ALT,MS15UST,MS35ALT,MS35UST,C5,C6";
            string h = getCurrentWeek();
            string g = DateTime.Now.ToString("dd.MM.yyyy");


            int hafta = int.Parse(h);


            string u = "http://www.mackolik.com/AjaxHandlers/ProgramComboHandler.ashx?sport=1&type=6&sortValue=DATE&week=" + hafta.ToString() + "&day=" + g.ToString() + "&sortDir=-1&groupId=-1&np=1";


            string data = dwnload(u);
            data = rep(data);
            string[] w = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string week in w)
            {
                string p = Regex.Replace(week, @"/\{(.*?)\}/", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (p.Length == 38 && p.Replace("'", "").Substring(0, 1) == "1")
                {
                    string nwk = p.Replace("'", "").Substring(0, 5);

                    tt += haftaAl(nwk);

                }


            }


            StreamWriter sw = new StreamWriter(@"..\" + h.ToString() + ".txt", false);

            sw.Write(tt);
            sw.Close();

        }
        public static string haftaAl(string h)
        {
            string t = "";

            string u = "http://www.mackolik.com/AjaxHandlers/ProgramDataHandler.ashx?type=6&sortValue=DATE&week=" + h.ToString() + "&day=-1&sort=-1&sortDir=1&groupId=-1&np=0&sport=1";


            string data = dwnload(u);
            data = rep(data);
            data = data.Replace("{}", "{içi}");
            string[] d = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string p in d)
            {
                if (p.Length > 50)
                {
                    //                string pl = Regex.Replace(p, @"/\{(.*?)\}/", " ");

                    string degis = vericek(p, "{", "}");
                    string pl = p.Replace(degis, "-");

                    t += pl + Environment.NewLine;
                }
            }

            return t;
        }

        public static bool IsNumeric(string s)
        {
            float output;
            return float.TryParse(s, out output);
        }
        public static string getCurrentWeek()
        {
            string data = dwnload("http://www.mackolik.com/Genis-Iddaa-Programi");
            string currentWeek = vericek(data, "currentWeek = \"", "\"");
            System.Threading.Thread.Sleep(1000);

            return currentWeek;
        }

        public static string dwnload(string url)
        {
            WebClient wc = new WebClient();
            wc.Proxy = WebRequest.DefaultWebProxy;
            wc.Credentials = System.Net.CredentialCache.DefaultCredentials; ;
            wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;


            string source = "";

            wc.Headers.Add("Content-Type: text/html; charset=windows-1254");
            wc.Headers.Add("Content-Type: text/html; charset=iso-8859-9;");

            wc.Headers.Add("Content-Type: application/x-www-form-urlencoded");
            wc.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5");
            wc.Headers.Add("Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            wc.Headers.Add("Accept-Encoding: identity");
            wc.Headers.Add("Accept-Language: tr-TR");
            wc.Headers.Add("Accept-Language: tr-TR,tr;q=0.8");
            wc.Headers.Add("Referer", "http://www.mackolik.com/");
            wc.Encoding = Encoding.UTF8;
            try
            {
                source = wc.DownloadString(url);
                //   StreamWriter sw = new StreamWriter(@"C:\SourceCodes\" + hafta.ToString() + ".txt", false);
                // source = rep(source);

                //  sw.Write(source);
                //   sw.Close();

                Console.WriteLine(url + "Complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
                Console.ReadLine();
            }

            return source;

        }
        public static string vericek(string StrData, string StrBas, string StrSon)
        {
            try
            {
                int IntBas = StrData.IndexOf(StrBas) + StrBas.Length;
                int IntSon = StrData.IndexOf(StrSon, IntBas + 1);
                return StrData.Substring(IntBas, IntSon - IntBas);
            }
            catch
            {
                return "";
            }

        }

        public static string rep(string data)
        {
            try
            {
                // iddaa-rows-style' href='javascript:popTeam(

                data = data.Replace(Environment.NewLine, "");

                data = data.Replace("[{", "");
                data = data.Replace("[[", "|" + Environment.NewLine);
                data = data.Replace("]]", "|" + Environment.NewLine);
                data = data.Replace("],[", "|" + Environment.NewLine);
                data = Regex.Replace(data, @"/\{(.*?)\}/", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                // regex = ;
                return data;




            }
            catch
            {
                return "";
            }

        }

    
    }
}
