using CefSharp;
using FontAwesome.Sharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using xNet;
using static System.Net.Mime.MediaTypeNames;

namespace SEOShopee
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        CEFHelp CEFHelp = new CEFHelp();
        SetUp Setup = new SetUp();
        TkHelp TkHelp = new TkHelp();

        #region SHOPEE SETUP

        #endregion


        private async void Form1_Load(object sender, EventArgs e)
        {

            TkHelp.textBox = textBox;
            TkHelp.dgv = dgv;
            TkHelp.dgv2 = dgv2;
            CEFStatic.panel = panel1;
            //toolTip1.SetToolTip(iconButton7, "Khi thực hiện save cookie , phần mềm sẽ tự động save đến thư mục File/Cookie/Browser_X.txt\r\n" +
            //    "Mỗi browser sẽ được lưu trên 1 file txt chứa cookie riêng");

            //toolTip1.SetToolTip(iconButton8, "Load cookie cần thêm địa chỉ web cần load với đầy đủ https đầu, ví dụ https://shopee.vn/ \r\n" +
            //  "Mỗi browser sẽ xóa toàn bộ cookie cũ và dựa theo thư mục File/Cookie/Browser_X.txt mà bạn đã lưu sau đó tiến hành add cookie\r\n" +
            //  "Lưu ý : cookie của browser nào sẽ theo browser đó mà bạn đã đặt trước");

            //toolTip1.SetToolTip(iconButton9, "Phiên bản này là phiên bản đầu tiên và trong giai đoạn thử nghiệm cho nên sẽ không tránh khỏi sai sót \r\n" +
            //          "Mỗi phiên bản sẽ miễn phí hoàn toàn cho nên mong các bạn không mang đi bán với bất kì hình thức nào\r\n" +
            //          "Liên hệ hỗ trợ (Kiệt):\r\n" +
            //          "Sđt,zalo : 0888-055-888\r\n" +
            //          "Facebook : facebook.com/dontcarenothing");

            //CEFStatic.Key= HashMap.CalculateHash(HashMap.GetUniqueIdentifier());

            //CEFStatic.isLive = TkHelp.ReadKey(CEFStatic.Key);
            //if (!CEFStatic.isLive)
            //{
            //    if (MessageBox.Show($"Vui lòng gửi key đến admin để kích hoạt bản quyền!\r\nKey của bạn (click OK to copy ) :\r\n{CEFStatic.Key}", "Copy Key", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            //    {
            //        Clipboard.SetText(CEFStatic.Key);
            //    }
            //    this.Close();
            //    return;
            //}


            TkHelp.ReadListSetting();
            TkHelp.LoadSetting(this);

            textBox.ScrollBars = ScrollBars.Both;

            txt_url.Text = CEFStatic.url;

            dgv.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            CEFStatic.list_agent = TkHelp.ReadList("File\\UA.txt");

            panel1.AutoScroll = true;
            txt_value.Text = "1";
            txt_row_number.Text = "1";
            txt_start_index.Text = "0";
            txt_url.Text = "https://shopee.vn";
            CEFStatic.isAuto = true;//Tự động vào acc khác
                                    //await Setup.ListUser();

            cb_seo_shop.SelectedIndex = 0;
            //n_time_seo.Value = 30;
            //n_percent_seo.Value = 100;
            //n_page.Value = 99;

        }
        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (!CEFStatic.isFirstStart)
            {
                CEFHelp.DefaultSetting(CEFStatic.isImage, true, true, true, RandomUa.RandomUserAgentChrome);
                CEFStatic.isFirstStart = true;

            }
            Thread t = new Thread(() =>
            {
                //Lấy ra số acc chạy


                CEFHelp.AddGroupBoxes();


            });
            t.Start();
        }
        private void btn_Close_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    CEFAction.CloseBrowser(groupBrowser.browser, groupBrowser.thiscontrol);
                });
            }
            CEFStatic.list_cefBrowser.Clear();
            TkHelp.Comment($"Đã xóa toàn bộ browser");

        }
        private void txt_value_TextChanged(object sender, EventArgs e)
        {
            CEFStatic.value = int.Parse(txt_value.Text);
            TkHelp.UpdateSetting(new Setting { control_name = txt_value.Name, value = CEFStatic.value });

        }

        private void txt_row_number_TextChanged(object sender, EventArgs e)
        {
            CEFStatic.row_number = int.Parse(txt_row_number.Text);
            TkHelp.UpdateSetting(new Setting { control_name = txt_row_number.Name, value = CEFStatic.row_number });


        }

        private void txt_start_index_TextChanged(object sender, EventArgs e)
        {
            CEFStatic.start_index = int.Parse(txt_start_index.Text);
            CEFStatic.account_index = int.Parse(txt_start_index.Text);

            TkHelp.UpdateSetting(new Setting { control_name = txt_start_index.Name, value = CEFStatic.start_index });

        }


        private void panel1_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control is GroupBox)
            {
                try
                {
                    Cef.UIThreadTaskFactory.StartNew(delegate
                    {
                        string err = "";
                        var group = e.Control as GroupBox;
                        string proxy = "";
                        var flag = new Flag();
                        CEFBrowser cefBrowser = new CEFBrowser(flag, group);
                        CEFStatic.list_cefBrowser.Add(cefBrowser);

                        if (string.IsNullOrEmpty(CEFStatic.url)) //Nếu không có url nào được cài thì chạy url từ thanh tìm kiếm
                        {
                            CEFStatic.url = txt_url.Text;
                        }
                        cefBrowser.CreateBrowser(true, e.Control, out err);

                    });

                }
                catch (Exception err)
                {
                    TkHelp.Comment(err.Message);
                }

            }
        }

        private void panel1_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (CEFStatic.isAuto)
            {
                if (e.Control is GroupBox)
                {
                    try
                    {
                        var group = e.Control as GroupBox;
                        string Name = e.Control.Name;
                        if (group.Enabled && Name.Contains("Browser_"))
                        {
                            Task t = new Task(() =>
                            {
                                CEFHelp.AddGroupReplace(dgv, group.Size, group.Location);
                            });
                            t.Start();
                            Task.WhenAll(t);
                        }

                    }
                    catch (Exception err)
                    {
                        TkHelp.Comment(err.Message);
                    }
                }
            }

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            CEFHelp.SizeChanged();

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    groupBrowser.browser.LoadUrl(txt_url.Text);
                });
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    groupBrowser.browser.Reload(false);
                });
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    if (groupBrowser.browser.CanGoBack)
                    {
                        groupBrowser.browser.Back();

                    }
                });
            }
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            foreach (var groupBrowser in CEFStatic.list_cefBrowser)
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    if (groupBrowser.browser.CanGoForward)
                    {
                        groupBrowser.browser.Forward();

                    }
                });
            }
        }

        private void cb_isProxy_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.isProxy = cb_isProxy.Checked;
            TkHelp.UpdateSetting(new Setting { control_name = cb_isProxy.Name, value = CEFStatic.isProxy });

        }


        private void txt_link_proxy_TextChanged(object sender, EventArgs e)
        {
            CEFStatic.link_proxy = txt_link_proxy.Text;
            TkHelp.EndFocus(txt_link_proxy);
            TkHelp.Comment($"Url to proxy :{CEFStatic.link_proxy}");

            TkHelp.UpdateSetting(new Setting { control_name = txt_link_proxy.Name, value = CEFStatic.link_proxy });

            var list_string = TkHelp.ReadList(CEFStatic.link_proxy);
            int line = 0;
            foreach (var proxy in list_string)
            {
                line++;
                var splitProxy = proxy.Split(':');
                if (splitProxy.Length > 0)
                {
                    try
                    {
                        Proxy prx = new Proxy();
                        if (splitProxy.Length >= 2)
                        {
                            prx.host = splitProxy[0];
                            prx.port = int.Parse(splitProxy[1]);
                        }

                        if (splitProxy.Length == 4)
                        {
                            prx.username = splitProxy[2];
                            prx.password = splitProxy[3];
                        }

                        CEFStatic.list_proxy.Add(prx);
                    }
                    catch (Exception er)
                    {

                        TkHelp.Comment($"Proxy {line} lỗi : {proxy}");

                    }

                }
            }
            TkHelp.Comment($"Load {CEFStatic.list_proxy.Count()} proxy success");
        }

        private void txt_link_proxy_DoubleClick(object sender, EventArgs e)
        {
            string path = "";
            var list_string = TkHelp.OpenDialogAndSelectFile(out path);
            txt_link_proxy.Text = path;
        }


        private void n_page_ValueChanged(object sender, EventArgs e)
        {
            CEFStatic.max_page = (int)n_page.Value;
            TkHelp.UpdateSetting(new Setting { control_name = n_page.Name, value = CEFStatic.max_page });

        }

        private void cb_is_ads_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.is_ads = cb_is_ads.Checked;
            if (cb_is_ads.Checked)
            {
                cb_is_ads.Text = "Chạy luôn ads";
            }
            else
            {
                cb_is_ads.Text = "Bỏ qua ads";

            }
            TkHelp.UpdateSetting(new Setting { control_name = cb_is_ads.Name, value = CEFStatic.is_ads });

        }

        private void txt_keyword_TextChanged(object sender, EventArgs e)
        {
            var text = txt_keyword.Text.Trim();
            CEFStatic.list_keyword = new List<string>();
            try
            {
                if (text.Contains(","))
                {
                    CEFStatic.list_keyword = text.Split(',').ToList();

                }
                else
                {
                    CEFStatic.list_keyword = new List<string>
                {
                    text,
                };
                }
                TkHelp.Comment($"Có {CEFStatic.list_keyword.Count} keyword :{string.Join(",", CEFStatic.list_keyword.ToArray())}");


            }
            catch (Exception ex)
            {
                TkHelp.Comment($"Lỗi keyword :{ex.Message}");

            }
            TkHelp.UpdateSetting(new Setting { control_name = txt_keyword.Name, value = text });

        }

        private void txt_list_link_seo_TextChanged(object sender, EventArgs e)
        {
            var text = txt_list_link_seo.Text.Trim();
            CEFStatic.list_link_seo = new List<string>();
            CEFStatic.list_SEO = new List<SEO>();
            try
            {
                if (text.Contains(","))
                {
                    CEFStatic.list_link_seo = text.Split(',').ToList();

                }
                else
                {
                    CEFStatic.list_link_seo = new List<string>
                {
                    text,
                };
                }

                foreach (var url in CEFStatic.list_link_seo)
                {
                    var url_fragment = TkHelp.GetHtmlFragment(url);

                    Regex r = new Regex(@"i.(\d{6,12}).(\d{6,12})");
                    Match m = r.Match(url_fragment);
                    if (m.Success)
                    {
                        var seo = new SEO { item_id = m.Groups[2].Value, shop_id = m.Groups[1].Value };
                        CEFStatic.list_SEO.Add(seo);
                    }
                }
                TkHelp.Comment($"Có {CEFStatic.list_SEO.Count} shopid :{string.Join(",", CEFStatic.list_SEO.Select(c => c.shop_id).ToArray())}");

            }
            catch (Exception ex)
            {
                TkHelp.Comment($"Lỗi url seo :{ex.Message}");

            }
            TkHelp.UpdateSetting(new Setting { control_name = txt_list_link_seo.Name, value = text });

        }

        private void cb_seo_shop_SelectedIndexChanged(object sender, EventArgs e)
        {
            CEFStatic.seo_shop = cb_seo_shop.SelectedIndex;
            TkHelp.UpdateSetting(new Setting { control_name = cb_seo_shop.Name, value = CEFStatic.seo_shop });

        }

        private void n_percent_seo_ValueChanged(object sender, EventArgs e)
        {
            CEFStatic.percent_seo = (int)n_percent_seo.Value;
            TkHelp.UpdateSetting(new Setting { control_name = n_percent_seo.Name, value = CEFStatic.percent_seo });

        }

        private void n_time_seo_ValueChanged(object sender, EventArgs e)
        {
            CEFStatic.time_seo = (int)n_time_seo.Value;
            TkHelp.UpdateSetting(new Setting { control_name = n_time_seo.Name, value = CEFStatic.time_seo });

        }

        private void cb_is_like_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.is_like = cb_is_like.Checked;
            TkHelp.UpdateSetting(new Setting { control_name = cb_is_like.Name, value = CEFStatic.is_like });

        }

        private void cb_is_follow_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.is_follow = cb_is_follow.Checked;
            TkHelp.UpdateSetting(new Setting { control_name = cb_is_follow.Name, value = CEFStatic.is_follow });



        }

        private void cb_click_same_store_CheckedChanged(object sender, EventArgs e)
        {
            CEFStatic.click_same_store = cb_click_same_store.Checked;

            TkHelp.UpdateSetting(new Setting { control_name = cb_click_same_store.Name, value = CEFStatic.click_same_store });

        }

        private void n_deletedWindow_ValueChanged(object sender, EventArgs e)
        {
            CEFStatic.deletedWindow = (int)n_deletedWindow.Value;
            TkHelp.UpdateSetting(new Setting { control_name = n_deletedWindow.Name, value = CEFStatic.deletedWindow });

        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {

            textBox.SelectionStart = textBox.TextLength;
            textBox.ScrollToCaret();
        }

        private void txt_link_account_TextChanged(object sender, EventArgs e)
        {

            try
            {
                CEFStatic.list_account = new List<ShopeeAccount>();
                CEFStatic.link_account = txt_link_account.Text;
                TkHelp.EndFocus(txt_link_account);
                TkHelp.UpdateSetting(new Setting { control_name = txt_link_account.Name, value = CEFStatic.link_account });

                var list_string = TkHelp.ReadList(CEFStatic.link_account);
                for (int i = 0; i < list_string.Count; i++)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(list_string[i]))
                        {
                            if (list_string[i].Contains("|"))
                            {
                                var split_account = list_string[i].Split('|');
                             
                                var account = new ShopeeAccount
                                {
                                    Id = i + 1,
                                    Username = split_account[0],
                                    Password = split_account[1],
                                    Token = split_account[2],
                                    Finger = split_account[3],
                                    UserAgent = split_account[4],
                                };

                                CEFStatic.list_account.Add(account);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        TkHelp.Comment($"Lỗi account dòng [{i + 1}] : {list_string[i]}");
                    }



                }

                label10.Text = $"{CEFStatic.list_account.Count()} Account";
            }
            catch (Exception ex)
            {
                TkHelp.Comment($"link_account : {ex.Message}");
            }
        }

        private void txt_link_account_DoubleClick(object sender, EventArgs e)
        {
            string path = "";
            var list_string = TkHelp.OpenDialogAndSelectFile(out path);
            txt_link_account.Text = path;
        }

        private void txt_link_account_TextAlignChanged(object sender, EventArgs e)
        {

        }
    }
}
