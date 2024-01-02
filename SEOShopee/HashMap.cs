using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SEOShopee
{
    public class HashMap
    {
        public static string GetUniqueIdentifier()
        {
            StringBuilder identifierBuilder = new StringBuilder();

            // Lấy địa chỉ MAC của máy tính
            string macAddress = GetMACAddress();
            identifierBuilder.Append(macAddress);

            // Lấy tên máy tính
            string computerName = GetComputerName();
            identifierBuilder.Append(computerName);

            // Lấy tên người dùng
            string userName = GetUserName();
            identifierBuilder.Append(userName);

            // Lấy các thông số hệ thống khác
            string systemInfo = GetSystemInfo();
            identifierBuilder.Append(systemInfo);

            return identifierBuilder.ToString();
        }

        public static string GetMACAddress()
        {
            // Lấy danh sách các card mạng
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT MACAddress FROM Win32_NetworkAdapter WHERE MACAddress IS NOT NULL");
            ManagementObjectCollection objects = searcher.Get();

            StringBuilder macBuilder = new StringBuilder();

            foreach (ManagementObject obj in objects)
            {
                macBuilder.Append(obj["MACAddress"].ToString());
            }

            return macBuilder.ToString();
        }

        public static string GetComputerName()
        {
            return Environment.MachineName;
        }

        public static string GetUserName()
        {
            return Environment.UserName;
        }

        public static string GetSystemInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectCollection objects = searcher.Get();

            StringBuilder systemInfoBuilder = new StringBuilder();

            foreach (ManagementObject obj in objects)
            {
                systemInfoBuilder.Append(obj["Caption"].ToString());
                systemInfoBuilder.Append(obj["OSArchitecture"].ToString());

                // Kiểm tra trường CSDVersion
                if (obj["CSDVersion"] != null)
                {
                    systemInfoBuilder.Append(obj["CSDVersion"].ToString());
                }
            }

            return systemInfoBuilder.ToString();
        }

        public static string CalculateHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // Chuyển đổi thành chuỗi hex
                }

                return builder.ToString();
            }
        }
    }
}

