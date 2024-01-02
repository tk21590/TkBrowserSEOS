using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SEOShopee
{
    public class SetUp
    {
        public List<string> _list_user { get; set; }

        public IAccountServices _accountServices;

        public static List<ShopeeAccount> _accountsActive; //List acc hoạt động gửi
        public static List<ShopeeAccount> _accountsClaim; //List acc hoạt động nhận
        public static List<Thread> _threadRuns; //List acc hoạt động nhận
        public static long coin_send = 1500;
        public static string Token = "";

        public string autToken;
        public SetUp()
        {
            _accountServices = new AccountServices();
            _accountsActive = new List<ShopeeAccount>();
          var list=  TkHelp.ReadList("C:\\Users\\Administrator\\Desktop\\SHOPEE CLIENT\\Accxu.txt");
            foreach (var item in list)
            {
                var split_item = item.Split('|');
                var account = new ShopeeAccount
                {
                    Username = split_item[0],
                    Password = split_item[2],
                    Finger = split_item[3],
                    Token = split_item[4],
                   
                    UserAgent = split_item[5]
                   

                };
                _accountsActive.Add(account);
            }

        }

        public async Task<List<string>> ListUser()
        {

            try
            {
                autToken = await _accountServices.LoginAccount();
                if (!string.IsNullOrEmpty(autToken))
                {
                    _accountServices = new AccountServices();
                }

                return await _accountServices.GetListuserHaveAccount();
            }
            catch (Exception)
            {
                return new List<string>();
            }

        }

     

    }
}
