using CefSharp.DevTools.FedCm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SEOShopee.Properties;

namespace SEOShopee
{
    public class CEFStatic
    {
        public static Panel panel { get; set; }
        public static int start_index { get; set; }//Bắt đầu từ
        public static int Page { get; set; }//Số trang
        public static bool isLive { get; set; }//Hoạt động
        public static string Key { get; set; }//Hoạt động
        public static int value { get; set; } //Số acc
        public static int row_number { get; set; } //Số hàng
        public static int index_list_shop { get; set; }
        public static int deletedWindow { get; set; }
        public static string url { get; set; } //url 
        public static string link_proxy { get; set; } //url 
        public static string link_account { get; set; } //url 
        public static bool isRunning { get; set; } //Đang chạy

        public static bool isFirstStart { get; set; } //Lần đầu chạy tool

        public static List<CEFBrowser> list_cefBrowser { get; set; } = new List<CEFBrowser>();

        public static List<Proxy> list_proxy { get; set; } = new List<Proxy>();
        public static List<ShopeeAccount> list_account { get; set; } = new List<ShopeeAccount>();
        public static int index_account { get; set; }
        #region SEO
        public static bool is_like { get; set; }
        public static bool is_follow { get; set; }
        public static int time_seo { get; set; }
        public static int percent_seo { get; set; }
        public static bool is_ads { get; set; }
        public static bool click_same_store { get; set; }
        public static List<string> list_keyword { get; set; }
        public static List<string> list_link_seo { get; set; }
        public static List<SEO> list_SEO { get; set; }
        public static int max_page { get; set; }//Số trang
        public static int seo_shop { get; set; }//Chạy toàn bộ sản phẩm thuộc shop


        #endregion
        public static List<string> list_agent { get; set; } = new List<string>();
        public static bool isUpdate { get; set; } //Cho phép update sau khi đã load xong
        public static bool isProxy { get; set; }
        public static bool isAuto { get; set; } //Chạy auto 
        public static bool isImage { get; set; }
        public static bool isMulti { get; set; } //Đồng bộ thao tác
        public static int MouseX { get; set; }
        public static int MouseY { get; set; }
        public static List<Setting> list_settings { get; set; } = new List<Setting>();
        public static int proxy_index//Xoay vòng khi vượt quá số lượng
        {
            get
            {

                _proxy_index++;
                if (_proxy_index >= list_proxy.Count)
                {
                    _proxy_index = 0;

                }




                return _proxy_index;
            }
        }
        private static int _proxy_index;

        public static int agent_index//Xoay vòng khi vượt quá số lượng
        {
            get
            {
                _agent_index++;
                if (_agent_index >= list_agent.Count)
                {
                    _agent_index = 0;

                }

                return _agent_index;
            }
        }
        private static int _agent_index;
        public static int account_index;//Xoay vòng khi vượt quá số lượng

    }

}
