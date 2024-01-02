using CefSharp.DevTools.FedCm;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEOShopee
{

    public class SEO
    {
        public string shop_id { get; set; }
        public string item_id { get; set; }

    }
    public class Shop
    {
        private static int _nextId = 1;

        public int id { get; }
        public long shopid { get; set; }
        public long itemid { get; set; }
        public string name { get; set; }
        public string ctime { get; set; }
        public int follower_count { get; set; }
        public string shop_location { get; set; }
        public string place { get; set; }
        public int item_count { get; set; }
        public int liked_count { get; set; }
        public int rating_count { get; set; }


        public Shop()
        {
            id = _nextId;
            _nextId++;
        }

    }
    public class Flag
    {
        public int browser_index { get; set; } //Vị trí của browser
        public string browser_name { get; set; } //Tên của browser
        public string message { get; set; } //Nội dung truyền đến browser
        public bool is_completed { get; set; } //hoàn thành nhận coin
        public bool is_running { get; set; } //Acc đang sống 
        public bool is_running_main { get; set; } //Acc đang sống 
        public bool end_page { get; set; } //Acc đang sống 
        public int current_item { get; set; }
        public int next_page { get; set; }
        public int index_keyword { get; set; }
        public int checkDeleteWindow { get; set; }
        public string current_main_page { get; set; } 
        public string next_main_page { get; set; } 
        public Shopee shopee { get; set; } = new Shopee();

        public string user_agent { get; set; }
        public Proxy proxy { get; set; }
        public ChromiumWebBrowser browser { get; set; }



    }
    public class Proxy
    {
        public string username { get; set; }
        public string password { get; set; }
        public string host { get; set; }
        public int port { get; set; }


    }
    public class Setting
    {
        public string control_name { get; set; } //nằm ở control nào
        public object value { get; set; } //Giá trị dữ liệu
    }
    public class ApiRespone
    {
        public int err_code { get; set; }
        public string err_msg { get; set; }
        public int total { get; set; }
        public object data { get; set; }
        public IEnumerable<string> errors { get; set; }
        public double timestamp { get; set; }
    }
    public class HistoryAccount
    {
        public int Id { get; set; }
        public int SubId { get; set; }
        public string extend_userid { get; set; }
        public double timestamp { get; set; }
        public int total_account { get; set; } //tổng acc hôm nay
        public long total_coin_all { get; set; } //tổng xu có trong tất cả account
        public long total_coin_day { get; set; }//tổng xu hôm nay lấy được
        public int total_success { get; set; }//tổng acc điểm danh thành công hôm nay
        public int total_failed { get; set; }//tổng acc thất bại hôm nay
        public int total_block { get; set; }//tổng acc bị khóa
        public int total_other { get; set; }//tổng acc có trạng thái khác
    }
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class LiscenseKey
    {

        public int Id { get; set; }
        public string Username { get; set; } //Tên người dùng
        public string KeyName { get; set; } //Tên key
        public string Key { get; set; } //Key
        public string Notification { get; set; } //Thông báo đến người dùng
        public string Data { get; set; } //Data kèm theo
        public string Content { get; set; } //Nội dung thêm
        public string Version { get; set; } //Version của tool
        public double DateStart { get; set; }  //Ngày bắt đầu tạo key
        public bool Pause { get; set; }  //Tạm dừng đếm 
        public int DateOrder { get; set; } //Số ngày gia hạn 
        public int Price { get; set; } //Giá
        public bool Avaiable { get; set; } = true; //Còn hoạt động hay không
    }
    public class SearchViewModels
    {

        public string extend_userid { get; set; } //Mặc định của tài khoản
        public string username { get; set; } //Tìm kiếm theo username
        public int statusid { get; set; } = 1;//Tìm kiếm theo trạng thái account
        public int coin_min { get; set; } = 0; //Tìm kiếm theo số xu
        public int coin_max { get; set; } = 9999999;//Tìm kiếm theo số xu
        public int subid_max { get; set; } = 9999999;//Bắt đầu tìm kiếm với subid
        public int subid_min { get; set; } = 0;//Bắt đầu tìm kiếm với subid
        public int respositoryid { get; set; } //Phân loại acc
        public bool coin_checked_today { get; set; } //Đã nhận coin hôm nay hay chưa
        public int coin_claim_min_today { get; set; } = 0; //Số lần nhận coin nhỏ nhất từ acc khác đến
        public int coin_claim_max_today { get; set; } = 99; //Số lần nhận coin lớn nhất từ acc khác đến
        public double time_to { get; set; } = 9999999999; //Thời gian x10
        public double time_from { get; set; } = 0;
    }
    public class ShopeeAccount
    {

        public int Id { get; set; }
        public string ExtendUserId { get; set; }//String userId của tài khoản sở hữu
        public string ExtendUserName { get; set; } //Tên của tài khoản  sở hữu
        public int SubId { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public long IdUser { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Finger { get; set; }
        public string UserAgent { get; set; }
        public string Respone { get; set; } //phản hồi
        public string Cookies { get; set; }
        public long Coin { get; set; }
        public long Total_Claim { get; set; }//Tổng Số coin nhận từ tài khoản khác
        public long Total_Send { get; set; }//Tổng Xu gửi đến tài khoản khác
        public int Total_Checked { get; set; }//Tổng Số lần điểm danh hằng ngày
        public int ShopeeStatusId { get; set; }
        public int ShopeeRespositoryId { get; set; }
        public bool Coin_Checked { get; set; } //Điểm danh hàng ngày
        public int Coin_Claim { get; set; } //Số lần nhận xu hàng ngày từ tài khoản khác
        public int Coin_Send { get; set; } //Số lần gửi xu đi đến tài khoản khác
        public long Coin_Day { get; set; }//Xu điểm danh nhận được
        public double DateUpdated { get; set; }
        public double DateCreated { get; set; }



    }
}
