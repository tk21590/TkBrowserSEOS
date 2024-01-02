using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SEOShopee.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Threading;
using xNet;

namespace SEOShopee
{
    public class TkHelp
    {
        public static bool ReadKey(string keyname)
        {
            try
            {

         
            HttpRequest http = new HttpRequest();
            string url = "https://bestmoon.com.vn/api/Key/get_name/"+keyname;
            var get = http.Get(url).ToString();

            if (get.Contains("code"))
            {
                dynamic obj = JsonConvert.DeserializeObject(get);
                int code = obj["code"];
                bool success = obj["success"];
                if (success)
                {
                    int dateExpired = obj["data"]["dateExpired"];
                    var date = Date_UnixTimestampToDateTime(dateExpired);
                    if (DateTime.Now < date)
                    {
                       return true ;
                    }
                }
            }
            }
            catch (Exception)
            {

            }
            return false;

        }
        public static bool RegexMatch(string url)
        {
            Regex r;
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            r = new Regex(@"shopee(.*?)-i.(\d{4,15}).(\d{4,15})(.*?)");
            return r.IsMatch(url);

        }
        public static string GetHtmlFragment(string url)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                string originalUrl = uri.GetLeftPart(UriPartial.Path);
                return originalUrl;
            }


            return string.Empty;
        }
        public static string GetStringIP(HttpRequest http)
        {
            try
            {
                http.IgnoreProtocolErrors = true;
                var Get = http.Get("http://lumtest.com/myip.json").ToString();
                if (Get.Contains("ip"))
                {
                    return Get.ToString();

                }
            }
            catch (Exception)
            {

                return string.Empty;
            }



            return string.Empty;
        }
        public static string host = "zproxy.lum-superproxy.io";
        public static string username = "";
        public static string password = "";
        public static string portProxy = "";
        public static int port = 22225;
        public static HttpRequest Connect(HttpRequest http, out string content)
        {
            content = string.Empty;

            for (int i = 0; i < 3; i++)
            {
                string session_id = new Random(Guid.NewGuid().GetHashCode()).Next().ToString();
                var httpProxy = new HttpProxyClient(host, port, username + "-session-" + session_id, password);
                http.Proxy = httpProxy;

                http.IgnoreProtocolErrors = true;

                content = GetStringIP(http);
                if (!string.IsNullOrEmpty(content))
                {

                    return http;

                }
                Thread.Sleep(500);

            }
            return null;
        }
        public static string CreateUrl(long shopid, long itemid)
        {
            return $"https://shopee.vn/n-i.{shopid}.{itemid}?";
        }
        public static string GetRandomFile(string folderPath)
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            if (directory.Exists)
            {
                FileInfo[] files = directory.GetFiles();

                if (files.Length > 0)
                {
                    Random random = new Random();
                    int randomIndex = random.Next(0, files.Length);
                    return files[randomIndex].FullName;
                }
            }

            return null;
        }
        public static string Sha256Hash(string Input)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Input));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static string GetMD5(string chuoi)
        {
            string str_md5 = "";
            byte[] mang = System.Text.Encoding.UTF8.GetBytes(chuoi);

            MD5CryptoServiceProvider my_md5 = new MD5CryptoServiceProvider();
            mang = my_md5.ComputeHash(mang);

            foreach (byte b in mang)
            {
                str_md5 += b.ToString("x2");
            }

            return str_md5;
        }

        public static bool RandomBool(int truePercentage = 50)
        {
            Random r = new Random();
            return r.NextDouble() < truePercentage / 100.0;
        }
        public static void SaveSetting()
        {
            var path = Directory.GetCurrentDirectory() + "\\settings.json";
            string json = JsonConvert.SerializeObject(CEFStatic.list_settings);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            AddList(path, json);
            TkHelp.Comment($"{CEFStatic.list_settings.Count} cài đặt đã lưu thành công");

        }
        public static void UpdateSetting(Setting setting)
        {
            if (CEFStatic.isUpdate)
            {
                try
                {
                    var main_setting = CEFStatic.list_settings.FirstOrDefault(c => c.control_name == setting.control_name);
                    if (main_setting != null)
                    {
                        CEFStatic.list_settings.Remove(main_setting);

                    }
                    CEFStatic.list_settings.Add(setting);

                    SaveSetting();
                }
                catch (Exception err)
                {
                    TkHelp.Comment("Update Setting Failed!");
                }
            }


        }
        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
        public static async Task<string> Get2FACode(string code2FA)
        {

            code2FA = RemoveWhitespace(code2FA);
            string url = "https://2fa.live/tok/" + code2FA;

            try
            {
                HttpClient http = new HttpClient();

                var get = await http.GetStringAsync(url);
                if (get.Contains("token"))
                {
                    var token = Regex.Match(get, @"\d+").Value;

                    return token;
                }
            }
            catch (Exception)
            {

                return string.Empty;
            }

            return string.Empty;

        }
        public static void ReadListSetting()
        {
            try
            {
                var path = Directory.GetCurrentDirectory() + "\\settings.json";
                if (File.Exists(path))
                {
                    var settings_json = File.ReadAllText(path);
                    if (settings_json != null)
                    {
                        CEFStatic.list_settings = JsonConvert.DeserializeObject<List<Setting>>(settings_json);

                    }
                }
            }
            catch (Exception err)
            {
                TkHelp.Comment($"ReadSetting Error: {err.Message}");
            }
        }
        public static object lockForm = new object();
        public static void LoadSetting(Form form)
        {
            try
            {


                for (int i = 0; i < CEFStatic.list_settings.Count; i++)
                {
                    var setting = CEFStatic.list_settings[i];
                    if (setting != null)
                    {

                        var control_list = form.Controls.Find(setting.control_name, true);
                        if (control_list != null && control_list.Count() > 0)
                        {

                            var control = control_list.First();
                            if (control != null)
                            {
                                TkHelp.Comment($"{control.Name}|{setting.value}");

                                if (control is TextBox)
                                {
                                    control.Text = setting.value.ToString();
                                }
                                else
                                if (control is CheckBox)
                                {
                                    var checkBox = (CheckBox)control;
                                    checkBox.Checked = (bool)setting.value;
                                }
                                else
                                if (control is ComboBox)
                                {
                                    var comboBox = (ComboBox)control;

                                    int selected = int.Parse(setting.value.ToString());

                                    comboBox.SelectedIndex = selected;
                                }
                                else
                                if (control is NumericUpDown)
                                {
                                    var numberic = (NumericUpDown)control;

                                    int selected = int.Parse(setting.value.ToString());

                                    numberic.Value = selected;
                                }


                            }
                        }



                    }


                }
                CEFStatic.isUpdate = true;
            }
            catch (Exception err)
            {
                TkHelp.Comment($"LoadSetting Error: {err.Message}");
            }









        }
        public static List<string> GetLinesFromTextBox(TextBox textBox)
        {
            List<string> lines = new List<string>();
            string[] separators = { "\r\n", "\r", "\n" };
            string[] words = textBox.Text.Split(separators, StringSplitOptions.None);
            foreach (string word in words)
            {
                if (word.Length > 4)
                {
                    lines.Add(word);

                }
            }
            return lines;
        }
        public static string RandomUserAgent()
        {
            //"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/7.0.185.1002 Safari/537.36";

            //Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/7.0.185.1002 Safari/537.36
            string browser = "Mozilla/5.0";
            string platform = "(Windows NT 10.0; Win64; x64)";
            string engine = "AppleWebKit/" + RandomNumber(500, 599) + "." + RandomNumber(0, 99) + " (KHTML, like Gecko)";
            string product = "Chrome" + "/" + RandomNumber(80, 109) + "." + RandomNumber(0, 99) + "." + RandomNumber(2000, 3000) + "." + RandomNumber(80, 90);
            string browserSub = "UBrowser" + "/" + RandomNumber(4, 10) + "." + RandomNumber(0, 3) + "." + RandomNumber(150, 190) + "." + RandomNumber(100, 1100);
            string userAgent = browser + " " + platform + " " + engine + " " + product + " " + browserSub + " Safari/537.36";
            return userAgent;
        }
        public static DateTime Date_GetTimeNow()//thời gian hiện tại trả về datetime
        {
            return DateTime.UtcNow.AddHours(7);
        }
        public static DateTime Date_UnixTimestampToDateTime(int unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }

        public static string GetTimeMinify(int time)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;
            var nowDate = Date_GetTimeNow();
            var needDate = Date_UnixTimestampToDateTime(time);

            var ts = new TimeSpan(nowDate.Ticks - needDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "một giây trước" : ts.Seconds + " giây trước";

            if (delta < 2 * MINUTE)
                return "1 phút trước";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " phút trước";

            if (delta < 90 * MINUTE)
                return "1 giờ trước";

            if (delta < 24 * HOUR)
                return ts.Hours + " giờ trước";

            if (delta < 48 * HOUR)
                return ts.Hours + " giờ trước";

            if (delta < 30 * DAY)
                return ts.Days + " ngày trước";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "một tháng trước" : months + " tháng trước";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "một năm trước" : years + " năm trước";
            }
        }
        public static DataGridView dgv;
        public static object lockerdgvv = new object();
        public static void ChangeShopData(Shop shop, string status = "")
        {
            if (dgv == null)
            {
                return;
            }
            var id = shop.id - 1;
            lock (lockerdgvv)
            {
                dgv.BeginInvoke((Action)delegate ()
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        dgv.Rows[id].Cells[1].Value = status;

                    }

                    dgv.Rows[id].Cells[2].Value = shop.name;
                    dgv.Rows[id].Cells[3].Value = shop.ctime;
                    dgv.Rows[id].Cells[4].Value = shop.place;
                    dgv.Rows[id].Cells[5].Value = shop.follower_count.ToString("#,###");
                    dgv.Rows[id].Cells[6].Value = shop.liked_count.ToString("#,###");
                    dgv.Rows[id].Cells[7].Value = (shop.rating_count).ToString("#,###");
                    dgv.FirstDisplayedScrollingRowIndex = id;

                });
            }

        }


        public static DataGridView dgv2;
        public static void AddRow(string name)
        {
            dgv2.BeginInvoke((Action)delegate ()
            {
                dgv2.Rows.Add(name, "", "");
            });
        }
        public static int GetCellIndex(string searchString)
        {
            int result = -1;
            lock (lockerdgv)
            {
                foreach (DataGridViewRow row in dgv2.Rows)
                {
                    DataGridViewCell firstCell = row.Cells[0];
                    if (firstCell.Value != null && firstCell.Value.ToString().Equals(searchString))
                    {
                        result = firstCell.RowIndex;
                        break;
                    }
                }
            }
            return result;
        }
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            return random.Next(min, max);
        }

        public static object lockerdgv = new object();
        public static void ChangeTextDgv(string CellId, string Comment, int indexComment)
        {
            if (dgv == null)
            {
                return;
            }
            var id = GetCellIndex(CellId);
            if (id > -1)
            {
                lock (lockerdgv)
                {
                    dgv2.BeginInvoke((Action)delegate ()
                    {
                        dgv2.Rows[id].Cells[indexComment].Value = Comment;

                    });
                }
            }


        }

        public static int loopComment = 0;
        public static TextBox textBox;
        public static void Comment(string comment)
        {

            if (textBox == null)
            {
                return;
            }

            textBox.BeginInvoke((Action)delegate ()
            {
                textBox.Text += comment + "  ------" + DateTime.Now.ToString("HH:ss") + "   \r\n";
                if (loopComment > 300)
                {
                    textBox.Clear();
                    loopComment = 0;
                }
            });
            loopComment++;


        }
        public static void EndFocus(TextBox textBox)
        {
            textBox.SelectionStart = textBox.Text.Length;
            textBox.SelectionLength = 0;
        }
        public static string RandomToken(int length)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RandomList(List<string> _list_)
        {
            try
            {
                if (_list_.Count > 0 || _list_ != null)
                {
                    return _list_[RandomNumber(0, _list_.Count)];
                }
            }
            catch (Exception)
            {

            }

            return string.Empty;
        }
        public static string RandomCharToken(int length)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RandomNumberString(int length)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void CheckNumberic(KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }
        public static int GetNumberic(string number)
        {
            return int.Parse(number);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static object locker = new object();
        public static void AddList(string path, string NoiDung)
        {


            lock (locker)
            {
                FileStream fs = new FileStream(path, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(NoiDung);
                sw.Flush();
                sw.Close();
            }

        }

        public static List<string> ReadList(string path)
        {
            try
            {
                return File.ReadAllLines(path).ToList();

            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        public static List<string> OpenDialogAndSelectFile(out string path)
        {
            path = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "";
            openFileDialog1.Filter = "TXT files| *.txt";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
                var listFile = ReadList(path);

                return listFile;
            }

            return new List<string>();


        }

    }

}
