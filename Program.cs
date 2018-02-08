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

            //    

            int hafta = 17103;

            //     sw.WriteLine("hafta|macId|tip|code|homeId|home|awayId|away|IY|MS|MS1|MS1_Y|MSX|MSX_Y|MS2|MS2_Y|AU1|AU1_Y|AU2|AU2_Y|KG1|KG1_Y|KG0|KG0_Y|TKS|TKS_Y");

            for (int x = 0; x <=103; x++)
            {
                hafta = hafta - x;
                string source = wc.DownloadString("https://user.mackolik.com/userpages/CouponStats/CouponStatisticsHandler.aspx?w=" + hafta.ToString() + "&sb=6&sd=1&d=-1&ms=3&i=0&tm=&l=-1&p=0&ps=20000&ptype=1&cba=0&spt=1");
                System.Threading.Thread.Sleep(30000);
                StreamWriter sw = new StreamWriter(@"C:\SourceCodes\" + hafta.ToString() + ".txt", false);
                sw.Write(source);
                sw.Close();
                source = vericek(source, "<table", "</table>");
                // source = source.Replace('|',' ').Replace("</tr>", "|").ToString();
                string[] ayir = source.Split(new[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string parca in ayir)
                {
                    string son = "";
                    try
                    {


                        string[] icler = parca.Split(new[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        string tip = "";
                        if (icler[1].ToString().IndexOf("99999.gif") > -1)
                        {
                            tip = "DUELLO";
                        }
                        else { tip = "NORMAL"; }
                        //      if (parca.ToString().IndexOf("AU1") < 0 ) { tip = "EKSIK"; }



                        //hafta|macId|tip|code|homeId|home|awayId|away|IY|MS|MS1|MS1_Y|MSX|MSX_Y|MS2|MS2_Y|AU1|AU1_Y|AU2|AU2_Y|KG1|KG1_Y|KG0|KG0_Y|TKS|TKS_Y
                        string code = vericek(icler[0].ToString(), "<b>", "</b>");
                        string home = rep_home(icler[1].ToString());
                        string away = rep_away(tip, icler[1].ToString());
                        string macId = rep_mid(tip, icler[1].ToString());

                        string iy_skor = rep_skor(icler[2].ToString());
                        string ms_skor = rep_skor(icler[3].ToString());
                        string ms1_oran = rep_oran("MS1", icler[4].ToString());
                        string msx_oran = rep_oran("MSX", icler[5].ToString());
                        string ms2_oran = rep_oran("MS2", icler[6].ToString());
                        string AU1_oran = rep_oran("AU1", icler[7].ToString());
                        string AU2_oran = rep_oran("AU2", icler[8].ToString());
                        string KG1_oran = rep_oran("KG1", icler[9].ToString());
                        string KG0_oran = rep_oran("KG0", icler[10].ToString());
                        string TOPLAM_oran = rep_SON(icler[11].ToString());
                        string MK_oran = rep_SON(icler[12].ToString());
                        // Match away =  icler[2].ToString() 

                        son = hafta.ToString() + "," + macId.ToString() + ",'" + tip + "'," + code.ToString() + ","
                            + home.ToString() + "',"
                            + away.ToString() + "','"
                            + iy_skor.ToString() + "','"
                            + ms_skor.ToString() + "','"
                            + ms1_oran.ToString() + "','"
                            + msx_oran.ToString() + "','"
                            + ms2_oran.ToString() + "','"
                            + AU1_oran.ToString() + "','"
                            + AU2_oran.ToString() + "','"
                            + KG1_oran.ToString() + "','"
                            + KG0_oran.ToString() + "','"
                            + TOPLAM_oran.ToString().Replace(".", "").Replace(",", ".") + "','"
                            + MK_oran.ToString().Replace(",", ".") + "'";
                        son = son.Replace(" ", " ");
                        son = son.Replace("  ", " ");
                        son = son.Replace(" ,", ",");
                        son = son.Replace("' ", "'");
                        son = son.Replace(Environment.NewLine, "'");




                        if (IsNumeric(code))
                            insert(son); //sw.WriteLine(son);

                    }
                    catch
                    {
                        //  sw.WriteLine("hata");
                    }
                    //if (son.Replace(Environment.NewLine, "").Length > 2)
                    //    insert(son.Replace(Environment.NewLine, ""));

                }



            }
            //  sw.Close();
        }

        public static string rep_mid(string tip, string data)
        {
            try
            {
                if (tip == "DUELLO")
                {
                    data = vericek(data, "javascript:popDuelloDialog(", ",");
                }
                else
                {
                    data = vericek(data, "javascript:popMatch(", ",");
                }

                return data;




            }
            catch
            {
                return "";
            }

        }
        public static Boolean IsNumeric(String s)
        {
            float f;

            return float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out f);
            //float.TryParse(s, out output);
        }





        public static void insert(string q)
        {
            string query = "INSERT INTO tbl_mackolik_CouponStats_1 (hafta,macId,tip,code,homeId,home,awayId,away,IY,MS,MS1,MS1_Y,MSX,MSX_Y,MS2,MS2_Y,AU1,AU1_Y,AU2,AU2_Y,KG1,KG1_Y,KG0,KG0_Y,TKS,TKS_Y) VALUES (" + q + ")";
            SqlCommand cmd = new SqlCommand(query, baglanti);
            /* SqlCommand cmd = new SqlCommand("INSERT INTO PD_Sites_Tmp_Betvirus (dt,hid,home,aid,away,f1,fx,f2,alt,ust) VALUES (getdate(),@a,@b,@c,@d,@o1,@o2,@o3,@o4,@o5)", baglanti);
          cmd.Parameters.AddWithValue("@a", a);
            cmd.Parameters.AddWithValue("@b", b);
            cmd.Parameters.AddWithValue("@c", c);
            cmd.Parameters.AddWithValue("@d", d);
            cmd.Parameters.AddWithValue("@o1", o1);
            cmd.Parameters.AddWithValue("@o2", o2);
            cmd.Parameters.AddWithValue("@o3", o3);
            cmd.Parameters.AddWithValue("@o4", o4);
            cmd.Parameters.AddWithValue("@o5", o5);*/
            if (baglanti.State == ConnectionState.Closed)
            {
                baglanti.Open();
            }

            cmd.ExecuteNonQuery();
            baglanti.Close();
        }
        public static string rep_home(string data)
        {
            try
            {
                // iddaa-rows-style' href='javascript:popTeam(

                data = vericek(data, "iddaa-rows-style' href='javascript:popTeam(", "</span");
                data = data.Replace(")'>", ",'");
                data = data.Replace("<span class='cc-hand'>", "");

                return data;
            }
            catch
            {
                return "";
            }

        }
        public static Boolean IsNull(String t)
        {
            bool rv;

            if (t == null)
            { rv = true; }
            else { rv = false; }

            return rv;
        }
        public static string rep_oran(string tip, string data)
        {
            try
            {

                string data1 = vericek(data, tip + "\">", "<");
                data1 = data1.Replace(Environment.NewLine, "");


                data1 = data1.Replace(" ", "");


                if (IsNull(data1) || data1.Length < 2) { data1 = "0"; }



                string data2 = vericek(data, "%", "<");


                return data1.Replace(",", ".") + "','" + data2.Replace(",", ".");


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


        public static string rep_away(string tip, string data)
        {
            try
            {
                // iddaa-rows-style' href='javascript:popTeam(


                if (tip == "DUELLO")
                {
                    data = vericek(data, "gif\"></a>", "<span");
                }
                else
                {
                    data = vericek(data, "- </a>", "</span");
                }

                data = data.Replace("<a class='iddaa-rows-style' href='javascript:popTeam(", "");
                data = data.Replace(")'>", ",'");
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
