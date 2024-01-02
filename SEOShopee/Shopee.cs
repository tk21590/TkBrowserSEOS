using CefSharp.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using xNet;

namespace SEOShopee
{

    public class Shopee
    {

        public static readonly string IOSAgent = $"iOS app iPhone Shopee";
        public static readonly string AndroidAgent = $"Android app Shopee";

        //public static readonly string shopee_app_version = "28531";
        //public static readonly string shopee_rn_version = "1649949384";
        public static readonly string shopee_app_version = "29224";
        public static readonly string shopee_rn_version = "1664570692";
        public static readonly string AndroidAgentVer = $"Android app Shopee appver=" + shopee_app_version + " app_type=1";
        public static readonly string IOSAgentVer = $"iOS app iPhone Shopee appver=" + shopee_app_version + " app_type=1";
        //_http.Cookies.Add("shopee_app_version", "27010");
        //    _http.Cookies.Add("shopee_rn_version", "1613746323");

        public HttpRequest _http { get; set; }
        public int val { get; set; }
        public DataGridView dgv { get; set; }
        public ShopeeAccount account { get; set; }
        public int err_code { get; set; } = 99;
        public string err_msg { get; set; }
        public long userid { get; set; }
        public int increase_coins { get; set; }
        public string transaction_id { get; set; }

        public bool is_claim { get; set; }
        public bool is_send { get; set; }
        public bool is_get_coin { get; set; }
        public long userid_send { get; set; }
        public int subid_send { get; set; }
        public string authoziration { get; set; }
        public string riskToken { get; set; }
        #region LOGIN
        public bool Login()
        {
            _http = new HttpRequest
            {
                Cookies = new CookieDictionary(),
                //Proxy = new _httpProxyClient(host, port),
                IgnoreProtocolErrors = true,
                ConnectTimeout = 10000,
                KeepAliveTimeout = 10000,
                ReadWriteTimeout = 10000,
            };

            string SDT = account.Username;
            string Password = account.Password;


            if (string.IsNullOrEmpty(account.Finger) || string.IsNullOrEmpty(account.Token))
            {
                account.Finger = //Tkiet.RandomToken(32);
                account.Token = //Tkiet.RandomToken(32);
                //account.UserAgent = _http.ChromeUserAgent()+ " Shopee Beeshop locale/vi version=376 appver=26008 rnver=1599037766 app_type=1";
                account.UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; SM-G610F Build/MMB29K; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/99.0.4844.88 Mobile Safari/537.36 Shopee Beeshop locale/vi version=599 appver=28531 rnver=1648821680 app_type=1";
                //Android app Shopee appver=28609 app_type=1
            }
            string Finger = account.Finger;
            string Token = account.Token;
            string UserAgent = account.UserAgent;
            if (string.IsNullOrEmpty(UserAgent))
            {
                UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; SM-G610F Build/MMB29K; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/99.0.4844.88 Mobile Safari/537.36 Shopee Beeshop locale/vi version=599 appver=28531 rnver=1648821680 app_type=1";
            }
            _http.UserAgent = UserAgent;
            //: Mozilla/5.0 (Linux; Android 6.0.1; SM-G900H Build/MMB29K; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/88.0.4324.152 Mobile Safari/537.36 Shopee Beeshop locale/vi version=437 appver=27010 rnver=1613746323 app_type=1
            //Mozilla/5.0 (Linux; Android 4.2.1; en-us; Nexus 5 Build/JOP40D) AppleWebKit/535.19 (KHTML, like Gecko; googleweblight) Chrome/38.0.1025.166 Mobile Safari/535.19 Shopee Beeshop locale/vi version=376 appver=26008 rnver=1599037766 app_type=1




            // _http = //Tkiet.ConnectWeb(_http, val, dgv);
            //Add Thông Tin
            if (_http == null)
            {
                ////Tkiet.ChangeTextDgv(4, "Mạng Không Hoạt Động", val, dgv);
                return false;
            }

            _http.Cookies.Add("SPC_F", Finger);
            _http.Cookies.Add("csrftoken", Token);
            if (!LoginWeb(false, SDT, Token, Password, Finger, UserAgent))
            {
                return false;
            }



            ///////////////Phần Mobile//////////////////////////////
            string shopee_token = GetShopeeToken(Token);
            _http.Cookies.Add("shopee_token", shopee_token);

            if (string.IsNullOrEmpty(shopee_token))
            {
                err_msg = "Không có token!";
                return false;
            }

        
            string cookie_mobile = _http.Cookies.ToString();


            _http.Cookies.Add("shopee_app_version", shopee_app_version);
            _http.Cookies.Add("shopee_rn_version", shopee_rn_version);
            return LoginMobile(cookie_mobile);



        }

        public bool LoginMobile(string cookie_mobile)
        {
            _http.AddHeader("Cookie", cookie_mobile);
            _http.IgnoreProtocolErrors = true;
            var Get_login_acc = _http.Get("https://mall.shopee.vn/api/v2/user/login_status").ToString();
            if (Get_login_acc.Contains("error\":0"))
            {
                account.Cookies = _http.Cookies.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LoginWeb(bool isOTP, string SDT, string csrftoken, string Password, string finger, string userAgent)
        {
            string HasPassword = TkHelp.Sha256Hash(TkHelp.GetMD5(Password));

            _http.IgnoreProtocolErrors = true;
            try
            {
                _http.Get("https://shopee.vn/api/v1/account_info/?need_cart=1&skip_address=1");

            }
            catch (Exception)
            {

                //Tkiet.ChangeTextDgv(4, "SSL Error!", val, dgv);
                return false;

            }


            //string addressLogin = "_https://shopee.vn/api/v2/authentication/login";
            string addressLogin = "https://shopee.vn/api/v4/account/login_by_password";

            string datalogin = "{\"password\":\"" + HasPassword + "\",\"captcha\":\"\",\"support_whats_app\":true,\"phone\":\"" + SDT + "\"}";
            _http.Referer = "_https://shopee.vn/";
            _http.AddHeader("X-CSRFToken", csrftoken);
            _http.IgnoreProtocolErrors = true;
            string PostLogin = _http.Post(addressLogin, datalogin, "application/json").ToString();
            dynamic objuser = JsonConvert.DeserializeObject(PostLogin);
            string error = objuser.error.ToString();


            switch (error)
            {

                case "0":
                    err_msg = "success";
                    userid = objuser["data"]["userid"];
                    return true;
                case "":
                    err_msg = "success 2";
                    userid = objuser["data"]["userid"];
                    return true;
                case "2":
                    err_msg = "Tên tài khoản của bạn hoặc mật khẩu không đúng";
                    account.ShopeeStatusId = 3;
                    return false;
                case "4":
                    err_msg = "Tài khoản không tồn tại hoặc sai mật khẩu";
                    account.ShopeeStatusId = 3;
                    return false;
                case "9":
                    err_msg = "Tài khoản bị cấm";
                    account.ShopeeStatusId = 4;
                    return false;
                case "10027":
                    err_msg = "Tài khoản có captcha";
                    account.ShopeeStatusId = 5;
                    return false;
                case "35":
                    err_msg = "Tài khoản dính otp";
                    return false;
                case "77":
                    err_msg = "Tài khoản dính otp";
                    return false;
                case "98":
                    err_msg = "Login lỗi";
                    return false;
                default:
                    err_msg = $"chưa xác định {error}";
                    account.ShopeeStatusId = 7;
                    return false;
            }

        }
        public string GetShopeeToken(string csrftoken)
        {
            _http.IgnoreProtocolErrors = true;
            _http.AddHeader("X-CSRFToken", csrftoken);
            _http.Referer = "_https://banhang.shopee.vn/";
            string UID = Guid.NewGuid().ToString();

            string adressBanhang = "https://banhang.shopee.vn/api/v2/login/?SPC_CDS=" + UID + "&SPC_CDS_VER=2";

            var GetCookies = _http.Get(adressBanhang).ToString();
            if (GetCookies.Contains("username"))
            {
                Thread.Sleep(1000);
                _http.AddHeader("X-CSRFToken", csrftoken);
                _http.Referer = "_https://banhang.shopee.vn/";

                GetCookies = _http.Get(adressBanhang).ToString();
                if (GetCookies.Contains("username"))
                {
                    dynamic obj = JsonConvert.DeserializeObject(GetCookies);
                    string shopee_token = obj.token.ToString();
                    string url = "https://banhang.shopee.vn/api/v3/general/get_shop_address";
                    string get = _http.Get(url).ToString();
                    return shopee_token;

                }

                return "";
            }
            else
            {
                return "";
            }

        }
        #endregion
        public static void AddHeaderMulti(HttpRequest http, string csrftoken, string referer, string json_preview = "", string ifnonmatch = "")
        {
            http.AddHeader("X-API-SOURCE", "rn");
            http.AddHeader("Accept", "application/json");
            http.AddHeader("X-Requested-With", "XMLHttpRequest");
            http.AddHeader("x-csrftoken", csrftoken);
            http.AddHeader("Referer", referer);
            //http.AddHeader("UserAgent", FormSetting.IOSAgentVer);
            if (string.IsNullOrEmpty(ifnonmatch))
            {
                string inm = "55b03-" + TkHelp.GetMD5("55b03" + TkHelp.GetMD5(json_preview) + "55b03");
                http.AddHeader("if-none-match-", inm);
            }
            else
            {
                http.AddHeader("if-none-match-", ifnonmatch);

            }


            http.IgnoreProtocolErrors = true;

        }
        public bool LoginWeb()
        {
            _http.IgnoreProtocolErrors = true;
            try
            {
                _http.Get("https://shopee.vn/api/v1/account_info/?need_cart=1&skip_address=1");

            }
            catch (Exception)
            {

                //TkHelp.ChangeTextDgv(4, "SSL Error!", val, dgv);
                return false;

            }


            string HasPassword = TkHelp.Sha256Hash(TkHelp.GetMD5(account.Password));

            _http.IgnoreProtocolErrors = true;
            string addressLogin = "https://shopee.vn/api/v4/account/login_by_password";

            string datalogin = "{\"password\":\"" + HasPassword + "\",\"captcha\":\"\",\"support_whats_app\":true,\"phone\":\"" + account.Username + "\"}";
            //Đăng Nhập
            _http.Referer = "https://shopee.vn/";
            _http.AddHeader("X-CSRFToken", account.Token);
            _http.IgnoreProtocolErrors = true;
            var postdata = _http.Post(addressLogin, datalogin, "application/json; charset=utf-8").ToString();

            if (postdata.Contains("error"))
            {
                dynamic objuser = JsonConvert.DeserializeObject(postdata);

                string error = objuser.error.ToString();


                switch (error)
                {

                    case "0":
                        err_msg = "success";
                        userid = objuser["data"]["userid"];
                        return true;
                    case "":
                        err_msg = "success 2";
                        userid = objuser["data"]["userid"];
                        return true;
                    case "2":
                        err_msg = "Tên tài khoản của bạn hoặc mật khẩu không đúng";
                        account.ShopeeStatusId = 3;
                        return false;
                    case "4":
                        err_msg = "Tài khoản không tồn tại hoặc sai mật khẩu";
                        account.ShopeeStatusId = 3;
                        return false;
                    case "9":
                        err_msg = "Tài khoản bị cấm";
                        account.ShopeeStatusId = 4;
                        return false;
                    case "10027":
                        err_msg = "Tài khoản có captcha";
                        account.ShopeeStatusId = 5;
                        return false;
                    case "35":
                        err_msg = "Tài khoản dính otp";
                        return false;
                    case "77":
                        err_msg = "Tài khoản dính otp";
                        return false;
                    case "98":
                        err_msg = "Login lỗi";
                        return false;
                    default:
                        err_msg = "chưa xác định";
                        account.ShopeeStatusId = 7;
                        return false;
                }

            }
            err_msg = "Có lỗi khi đăng nhập";
            return false;

        }
        public bool LoginCheck()
        {
            _http = new HttpRequest
            {
                Cookies = new CookieDictionary(),
                IgnoreProtocolErrors = true,
                ConnectTimeout = 10000,
                KeepAliveTimeout = 10000,
                ReadWriteTimeout = 10000,
            };


            string content = "";
            _http = TkHelp.Connect(_http, out content);
            if (string.IsNullOrEmpty(content))
            {
                err_msg = "Proxy not auth";
                //Lỗi proxy
                return false;
            }
            _http.Cookies.Add("SPC_F", account.Finger);
            _http.Cookies.Add("csrftoken", account.Token);
            string HasPassword = TkHelp.Sha256Hash(TkHelp.GetMD5(account.Password));

            _http.IgnoreProtocolErrors = true;
            string addressLogin = "https://mall.shopee.vn/api/v2/authentication/login";

            string datalogin = "{\"captcha\":\"\",\"password\":\"" + HasPassword + "\",\"phone\":\"" + account.Username + "\",\"support_whats_app\":true,\"support_ivs\":true,\"username\":\"\"}";

            AddHeaderMulti(_http, account.Token, "https://mall.shopee.vn", datalogin);
            //Đăng Nhập
            var postdata = _http.Post(addressLogin, datalogin, "application/json; charset=utf-8").ToString();

            if (postdata.Contains("error_msg"))
            {
                dynamic objuser = JsonConvert.DeserializeObject(postdata);

                string error = objuser.error.ToString();


                switch (error)
                {

                    case "0":
                        err_msg = "success";
                        userid = objuser["data"]["userid"];
                        return true;
                    case "":
                        err_msg = "success 2";
                        userid = objuser["data"]["userid"];
                        return true;
                    case "2":
                        err_msg = "Tên tài khoản của bạn hoặc mật khẩu không đúng";
                        account.ShopeeStatusId = 3;
                        return false;
                    case "4":
                        err_msg = "Tài khoản không tồn tại hoặc sai mật khẩu";
                        account.ShopeeStatusId = 3;
                        return false;
                    case "9":
                        err_msg = "Tài khoản bị cấm";
                        account.ShopeeStatusId = 4;
                        return false;
                    case "10027":
                        err_msg = "Tài khoản có captcha";
                        account.ShopeeStatusId = 5;
                        return false;
                    case "35":
                        err_msg = "Tài khoản dính otp";
                        return false;
                    case "77":
                        err_msg = "Tài khoản dính otp";
                        return false;
                    case "98":
                        err_msg = "Login lỗi";
                        return false;
                    default:
                        err_msg = "chưa xác định";
                        account.ShopeeStatusId = 7;
                        return false;
                }

            }
            err_msg = "Có lỗi khi đăng nhập";
            return false;

        }

        public bool GetWallet()
        {
            AddHeaderMulti(account.Token, "https://mall.shopee.vn", "55b03-bc8aac519e7f02d5195c86cb7e95be321", "");
            string url = "https://mall.shopee.vn/api/v2/homepage/get_wallet_and_coin_balance";
            var get = _http.Get(url).ToString();
            if (get.Contains("coin"))
            {
                dynamic obj = JsonConvert.DeserializeObject(get);
                var error = obj["error"];
                if (error != null)
                {

                    string coin = obj.data.coin.balance.ToString();
                    account.Coin = long.Parse(coin);//Coin hiện tại
                                                    //err_msg = "cập nhật thành công";
                    return true;
                }

            }
            else
            {
                err_msg = "Lỗi cập nhật thông tin";
            }

            return false;
        }

        public bool CheckLogin()
        {
            if (string.IsNullOrEmpty(account.Cookies))//Nếu cookies bị nul
            {
                return false;
            }
            if (account.Cookies.Contains("csrftoken") == false)
            {
                return false;
            }

            return true;
        }
        public bool Get_Active_Login_Page()
        {
            if (string.IsNullOrEmpty(account.Finger))
            {
                account.Finger = TkHelp.RandomToken(32);

            }
            if (string.IsNullOrEmpty(account.Token))
            {
                account.Token = TkHelp.RandomToken(32);

            }

            string url = "https://shopee.vn/api/v2/authentication/get_active_login_page";
            _http.Cookies = new CookieDictionary();


            _http.Cookies.Add("csrftoken", account.Token);

            _http.Cookies.Add("shopee_app_version", shopee_app_version);
            _http.Cookies.Add("shopee_rn_version", shopee_rn_version);
            _http.Cookies.Add("SPC_F", account.Finger);


            _http.UserAgent = IOSAgentVer;
            var json = new JObject();
            var json_string = JsonConvert.SerializeObject(json);
            AddHeaderMulti(account.Token, "https://shopee.vn/buyer/login?next=https%3A%2F%2Fshopee.vn%2F", "", json_string);

            var post = _http.Post(url, json.ToString(), "application/json").ToString();

            if (post.Contains("error\":null"))
            {
                return true;
            }

            return false;
        }
        public void AddHeaderMulti(string csrftoken, string referer, string ifnonmatch, string json_preview)
        {
            _http.AddHeader("X-API-SOURCE", "rn");
            _http.AddHeader("Accept", "application/json");
            _http.AddHeader("X-Requested-With", "XMLHttpRequest");
            _http.AddHeader("x-csrftoken", csrftoken);
            _http.AddHeader("Referer", referer);
            _http.UserAgent = IOSAgent;
            if (string.IsNullOrEmpty(ifnonmatch))
            {
                string inm = "55b03-" + TkHelp.GetMD5("55b03" + TkHelp.GetMD5(json_preview) + "55b03");
                _http.AddHeader("if-none-match-", inm);
            }
            else
            {
                _http.AddHeader("if-none-match-", ifnonmatch);

            }


            _http.IgnoreProtocolErrors = true;

        }

        public bool Like(string shopid,string itemid)
        {
            var url = "https://shopee.vn/api/v4/pages/like_items";
            string data = "{\"shop_item_ids\":[{\"shop_id\":"+shopid+",\"item_id\":"+itemid+"}]}";
            AddHeaderMulti(account.Token, "https://shopee.vn/", "", "");
            var post = _http.Post(url, data, "application/json").ToString();
            if (post.Contains("error\":0"))
            {
                return true;
            }
            return false;
        }  
        public bool Follow(string shopid)
        {
            var url = "https://shopee.vn/api/v4/shop/follow";
            string data = "{\"shopid\":"+shopid+"}";
            AddHeaderMulti(account.Token, "https://shopee.vn/", "", "");
            var post = _http.Post(url, data, "application/json").ToString();
            if (post.Contains("error\":0"))
            {
                return true;
            }
            return false;
        }
    }
}
