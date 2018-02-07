using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Data;
using System.Globalization;

namespace Maclar
{
    class Program
    {
        public static SqlConnection baglanti;
        static void Main(string[] args)
        {
            baglanti = new SqlConnection("Data Source=.; Initial Catalog=iddaa; Integrated Security=true");

            WebClient wc = new WebClient();

            wc.Proxy = WebRequest.DefaultWebProxy;
            wc.Credentials = System.Net.CredentialCache.DefaultCredentials; ;
            wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;


            //wc.Headers.Add("Content-Type: text/html; charset=windows-1254");
            //wc.Headers.Add("Content-Type: text/html; charset=iso-8859-9;");

            //wc.Headers.Add("Content-Type: application/x-www-form-urlencoded");
            //wc.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5");
            //wc.Headers.Add("Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            //wc.Headers.Add("Accept-Encoding: identity");
            //wc.Headers.Add("Accept-Language: tr-TR");
            //wc.Headers.Add("Accept-Language: tr-TR,tr;q=0.8");
            wc.Encoding = Encoding.UTF8;
            long tick = DateTime.Now.Ticks;

            string source = wc.DownloadString("aspx?w=18008&sb=6&sd=1&d=-1&ms=3&i=0&tm=&l=-1&p=0&ps=11&ptype=1&cba=0&spt=1&?_=" + tick.ToString());


            source = vericek(source, "<table", "</table>");
            // source = source.Replace('|',' ').Replace("</tr>", "|").ToString();
            string[] ayir = source.Split(new[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);

            int i = 1;
            StreamWriter sw = new StreamWriter(@"C:\Users\A\Desktop\kapakingen\m1.txt", false);


            foreach (string parca in ayir)
            {
                string son = "";
                try
                {


                    string[] icler = parca.Split(new[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
  string tip="";
                    if (icler[1].ToString().IndexOf("99999.gif") > -1)
                    {
                        tip = "DUELLO";
                    }
                    if (parca.ToString().IndexOf("AU1") < 0 ) { tip = "EKSIK"; }

                    
                    //
                    //<td class="t1bvr"><a href="/teamInfo/32859/"><strong>Oxford United</strong></a></td>
                    string code = vericek(icler[0].ToString(), "<b>", "</b>");
                    string code2 = "code:" + code;
                    string home = "home:" + rep_home(icler[1].ToString());
                    string away = "away:" + rep_away(icler[1].ToString());
                    string iy_skor = "iy_skor:" + rep_skor(icler[2].ToString());
                    string ms_skor = "ms_skor:" + rep_skor(icler[3].ToString());
                    string ms1_oran = "ms1_oran:" + rep_oran("MS1", icler[4].ToString());
                    string msx_oran = "msx_oran:" + rep_oran("MSX", icler[5].ToString());
                    string ms2_oran = "ms2_oran:" + rep_oran("MS2", icler[6].ToString());
                    string AU1_oran = "AU1_oran:" + rep_oran("AU1", icler[7].ToString());
                    string AU2_oran = "AU2_oran:" + rep_oran("AU2", icler[8].ToString());
                    string KG1_oran = "KG1_oran:" + rep_oran("KG1", icler[9].ToString());
                    string KG0_oran = "KG0_oran:" + rep_oran("KG0", icler[10].ToString());
                    string TOPLAM_oran = "TOPLAM_oran:" + rep_SON(icler[11].ToString());
                    string MK_oran = "MK_oran:" + rep_SON(icler[12].ToString());
                    // Match away =  icler[2].ToString() 

                    son = "'" + code.ToString() + "|"
                        + home.ToString() + "|"
                        + away.ToString() + "|"
                        + iy_skor.ToString() + "|"
                        + ms_skor.ToString() + "|"
                        + ms1_oran.ToString() + "|"
                        + msx_oran.ToString() + "|"
                        + ms2_oran.ToString() + "|"
                        + AU1_oran.ToString() + "|"
                        + AU2_oran.ToString() + "|"
                        + KG1_oran.ToString() + "|"
                        + KG0_oran.ToString() + "|"
                        + TOPLAM_oran.ToString() + "|"
                        + MK_oran.ToString() + "'";
                    son = son.Replace("  ", " ");
                    son = son.Replace("  ", "','");
                    son = son.Replace("_ _", "','");


                    if (IsNumeric(code))
                        sw.WriteLine(son);

                }
                catch
                {
                    sw.WriteLine("hata");
                }
                //if (son.Replace(Environment.NewLine, "").Length > 2)
                //    insert(son.Replace(Environment.NewLine, ""));

            }
            sw.Close();



        }


        public static Boolean IsNumeric(String s)
        {
            float f;

            return float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out f);
            //float.TryParse(s, out output);
        }




        public static void insert(string p)
        {
            StreamWriter sw = new StreamWriter(@"C:\Users\A25318\Desktop\kapakingen\m1.txt", false);


            sw.WriteLine(p);
            sw.Close();
        }

        public static string rep_home(string data)
        {
            try
            {
                // iddaa-rows-style' href='javascript:popTeam(

                data = vericek(data, "iddaa-rows-style' href='javascript:popTeam(", "</span");
                data = data.Replace(")'>", "|");
                data = data.Replace("<span class='cc-hand'>", "");

                return data;
            }
            catch
            {
                return "";
            }

        }

        public static string rep_oran(string tip, string data)
        {
            try
            {

                string data1 = vericek(data, tip + "\">", "<");
                data1 = data1.Replace(Environment.NewLine, "");

                string data2 = vericek(data, "%", "<");


                return data1 + "|" + data2;


            }
            catch
            {
                return "";
            }
        }



        public static string rep_SON(string data)
        {
            try
            {

                data = data.Replace(Environment.NewLine, "");

                data = data.Replace("<td style=\"text-align:right;\">", "");
                data = data.Replace("<td style=\"text-align:center;\">", "");
                data = data.Replace(" ", "");
                return data;




            }
            catch
            {
                return "";
            }

        }
        public static string rep_skor(string data)
        {
            try
            {
                // iddaa-rows-style' href='javascript:popTeam(

                data = data.Replace(Environment.NewLine, "");

                data = data.Replace("<td align=\"center\">", "");
                data = data.Replace(" ", "");
                return data;




            }
            catch
            {
                return "";
            }

        }
        public static string rep_away(string data)
        {
            try
            {
                // iddaa-rows-style' href='javascript:popTeam(

                data = vericek(data, "- </a>", "</span");


                data = data.Replace("<a class='iddaa-rows-style' href='javascript:popTeam(", "");
                data = data.Replace(")'>", "|");
                data = data.Replace("<span class='cc-hand'>", "");
                return data;




            }
            catch
            {
                return "";
            }

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
        public static string rep2(string data)
        {
            try
            {
                data = data.Replace(" ", "_");
                data = data.Replace(Environment.NewLine, "");
                data = data.Replace(Environment.NewLine, "");
                data = data.Replace("\n", "");
                data = data.Replace("\t", "");
                char tab = '\u0009';
                data = data.Replace("<span_class=\"  \">", "");
                data = data.Replace("<td_align=\"center\"_class=\"brdright\">", "");
                data = data.Replace("<span_class=\"", "");
                data = data.Replace("</span>", " ");
                data = data.Replace("-", " ");
                data = data.Replace("bBold", "");
                data = data.Replace("dx", "");
                data = data.Replace("rx", "");
                data = data.Replace("sx", "");
                data = data.Replace(">", " ");
                data = data.Replace("\"", "");

                data = data.Replace(tab.ToString(), "");

                data = data.Replace("__", " ");


                return data;
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
                data = data.Replace(" ", "_");
                data = data.Replace("%", " ");
                data = data.Replace("%", " ");
                data = data.Replace("\n", "");
                data = data.Replace("\t", "");
                char tab = '\u0009';
                data = data.Replace(tab.ToString(), "");

                data = data.Replace(Environment.NewLine, "");
                data = data.Replace("<a_href=\"/teamInfo/", " ");
                data = data.Replace("<td_class=\"t1bvr\">", "");
                data = data.Replace("<td_class=\"t2bvr\">", "");
                data = data.Replace("</a>", "");
                data = data.Replace("<strong>", "");
                data = data.Replace("</strong>", " ");
                data = data.Replace("/\"", "','");
                data = data.Replace(">", " ");

                data = data.Replace("__", " ");
                // .Replace("<td class=\"t2bvr\">", "").Replace("</a>", "").Replace("<strong>", "").Replace("</strong>", "").Replace("/\">", "->");

                return data;
            }
            catch
            {
                return "";
            }

        }
        public void CreateTXTFile(DataTable dt, string strFilePath)
        {
            try
            {
                // Create the CSV file to which grid data will be exported.
                StreamWriter sw = new StreamWriter(strFilePath, false);
                // First we will write the headers.
                //DataTable dt = m_dsProducts.Tables[0];
                int iColCount = dt.Columns.Count;
                for (int i = 0; i < iColCount; i++)
                {
                    sw.Write(dt.Columns[i]);
                    if (i < iColCount - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);

                // Now write all the rows.

                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < iColCount; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            sw.Write(dr[i].ToString());
                        }
                        if (i < iColCount - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
    
