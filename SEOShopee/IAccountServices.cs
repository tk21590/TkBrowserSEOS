using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SEOShopee
{
    public class Services
    {
        public static HttpClient _httpClient { get; set; }
    }
    public interface IAccountServices
    {
        Task<List<ShopeeAccount>> SearchListAccount(SearchViewModels searchViewModels);
        Task<List<ShopeeAccount>> GetListAccountTool(int pageNumber, int pageSize, string extend_userid, bool acc_send);
        Task<ApiRespone> EditListAccount(List<ShopeeAccount> accounts);
        Task<ApiRespone> EditAccount(ShopeeAccount accounts);
        Task<ApiRespone> ResetDataAccount();
        Task<List<LiscenseKey>> GetAllListKey();
        Task<ApiRespone> CreateHistory(string extend_userid);
        Task<ApiRespone> EditKey(LiscenseKey model);
        Task<List<string>> GetListuserHaveAccount();
        Task<string> LoginAccount();
        Task SetProxy();
    }

    public class AccountServices : IAccountServices
    {
        public SendRequest send_request = new SendRequest();
        public async Task<List<LiscenseKey>> GetAllListKey()
        {
            List<LiscenseKey> list_key = new List<LiscenseKey>();

            var respone = await send_request.SEND($"v1/key/list");
            if (respone.err_code == 0)
            {
                string data = respone.data.ToString();
                if (!string.IsNullOrEmpty(data) && data.Contains("username"))
                {

                    list_key = JsonConvert.DeserializeObject<List<LiscenseKey>>(data);

                }

            }
            return list_key;
        }
        public async Task<ApiRespone> EditKey(LiscenseKey model)
        {
            var respone = await send_request.SEND($"v1/key/edit/", model);
            return respone;
        }

        public async Task<ApiRespone> EditAccount(ShopeeAccount account)
        {
            var respone = await send_request.SEND($"v1/shopee/account/edit", account);
            return respone;
        }

        public async Task<ApiRespone> EditListAccount(List<ShopeeAccount> accounts)
        {
            var respone = await send_request.SEND($"v1/shopee/account/edit/list", accounts);

            return respone;

        }

        public async Task<ApiRespone> CreateHistory(string extend_userid)
        {
            var respone = await send_request.SEND($"v1/shopee/account/history/create/{extend_userid}");

            return respone;


        }
        public async Task<List<string>> GetListuserHaveAccount()
        {
            List<string> list_user = new List<string>();
            var respone = await send_request.SEND($"v1/user/account/list");
            if (respone.err_code == 0)
            {
                string data = respone.data.ToString();
                if (!string.IsNullOrEmpty(data))
                {

                    list_user = JsonConvert.DeserializeObject<List<string>>(data);

                }

            }

            return list_user;


        }

        public async Task<List<ShopeeAccount>> GetListAccountTool(int pageNumber, int pageSize, string extend_userid, bool acc_send)
        {
            List<ShopeeAccount> accounts = new List<ShopeeAccount>();
            //account/list/extend_userid/98d6fac9-db1d-4f60-af2e-25de5989d343/send/true?PageNumber=1&PageSize=10'
            var respone = await send_request.SEND($"v1/shopee/account/list/extend_userid/{extend_userid}/send/{acc_send}?PageNumber={pageNumber}&PageSize={pageSize}");

            if (respone.err_code == 0)
            {
                string data = respone.data.ToString();
                if (!string.IsNullOrEmpty(data) && data.Contains("subId"))
                {

                    accounts = JsonConvert.DeserializeObject<List<ShopeeAccount>>(data);

                }

            }
            return accounts;
        }

        public async Task<ApiRespone> ResetDataAccount()
        {
            var respone = await send_request.SEND($"v1/shopee/account/reset");
            return respone;

        }

        public async Task<List<ShopeeAccount>> SearchListAccount(SearchViewModels searchViewModels)
        {
            List<ShopeeAccount> accounts = new List<ShopeeAccount>();

            var respone = await send_request.SEND($"v1/shopee/account/search", searchViewModels);
            if (respone.err_code == 0)
            {
                string data = respone.data.ToString();
                if (!string.IsNullOrEmpty(data) && data.Contains("subId"))
                {

                    accounts = JsonConvert.DeserializeObject<List<ShopeeAccount>>(data);

                }

            }
            return accounts;
        }

        public async Task<string> LoginAccount()
        {
            try
            {
                LoginModel login_model = new LoginModel
                {
                    Username = "Admin001",
                    Password = "Nothing21590"
                };
                var respone = await send_request.SEND($"v1/auth/login", login_model);
                if (respone.err_code == 0)
                {
                    string data = respone.data.ToString();

                    return data;
                }
            }
            catch (Exception)
            {

            }
       
            return "";
        }

        public async Task SetProxy()
        {
            HttpClient _httpClient = new HttpClient();
            string api_key = "mYLT14yZFCMfpjgzR7G6BVdtJKx2rqXQ";
            _httpClient.BaseAddress = new Uri($"https://proxyaz.com");
            //string ip1 = "ip[]=123.156.122.164";
            //string ip2 = "ip[]=123.123.123.123";
            //string ip3 = "ip[]=456.456.456.456";
            //string url = $"/api/{api_key}/whitelist?type=set&{ip1}&{ip2}&ip[]={ip3}";

            string url = $"/api/{api_key}/getproxy";
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            var respone = await _httpClient.SendAsync(requestMessage);
            var code_status = (int)respone.StatusCode;
            var content_success = await respone.Content.ReadAsStringAsync();
            if (code_status == 200)//Request thành công
            {


            }
        }
    }
    public class SendRequest
    {


        public SendRequest()
        {

            Setting();


        }
        public async Task<ApiRespone> SEND(string url, object model = null)
        {
            HttpRequestMessage requestMessage;
            try
            {
                if (model != null)
                {
                    var datajson = JsonConvert.SerializeObject(model);
                    var datapost = new StringContent(datajson, Encoding.UTF8, "application/json");
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                    requestMessage.Content = datapost;
                }
                else
                {
                    requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                }

                var respone = await Services._httpClient.SendAsync(requestMessage);
                var code_status = (int)respone.StatusCode;
                if (code_status == 200)//Request thành công
                {
                    var content_success = await respone.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiRespone>(content_success);
                }
                return new ApiRespone { err_code = 99, err_msg = $"error server code :{code_status}", data = null };

            }
            catch (Exception ex)
            {
                return new ApiRespone { err_code = 99, err_msg = $"exception :{ex.Message}", data = null };

            }


        }
        public void Setting()
        {
            Services._httpClient = new HttpClient();
            Services._httpClient.BaseAddress = new Uri("http://shop-follow.xyz:999");
            string bearerToken = "Bearer " + SetUp.Token;
            // Add authorization if found
            if (!string.IsNullOrEmpty(bearerToken))
                Services._httpClient.DefaultRequestHeaders.Add("Authorization", bearerToken);

            Services._httpClient.DefaultRequestHeaders.Add("Accept", "application/json"); // Github API versioning
            Services._httpClient.DefaultRequestHeaders.Add("User-Agent", "ShopFollow Agent"); // Github requires a user-agent
        }
    }

}
