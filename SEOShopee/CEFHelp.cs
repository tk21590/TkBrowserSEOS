using CefSharp.WinForms;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace SEOShopee
{
    public class CEFHelp
    {
        public static void AddGroupReplace(DataGridView dgv, Size size, Point point)
        {
            int web_int = CEFStatic.start_index + 1;
           
            string nameGroup = "Browser_" + web_int;
            CEFStatic.start_index++;

            GroupBox group = new GroupBox();
            group.Size = size;

            group.Location = point;
            group.Name = nameGroup;
            group.Text = nameGroup;
            group.ForeColor = Color.White;
            TkHelp.AddRow(group.Name);
            CEFStatic.panel.Invoke((MethodInvoker)delegate
            {
                CEFStatic.panel.Controls.Add(group);

            });
        }
        public List<GroupBox> AddGroupBoxes()
        {
            List<GroupBox> groupBoxes = new List<GroupBox>();

            int Width = CEFStatic.panel.Width;
            int Height = CEFStatic.panel.Height;
            int groupbox_width = Width / CEFStatic.row_number;

            int groupbox_height = Height / 2;
            if (CEFStatic.value <= CEFStatic.row_number)
            {
                groupbox_height = Height;

            }
            // Tính toán số lượng groupbox có thể chứa được trên một hàng
            int boxes_per_row = CEFStatic.panel.Width / groupbox_width;

            // Tính toán số lượng hàng cần được thêm vào
            int num_rows = CEFStatic.value / boxes_per_row;
            if (CEFStatic.value % boxes_per_row > 0)
            {
                num_rows += 1;
            }

            // Thêm các groupbox vào CEFStatic.panel
            for (int i = 0; i < CEFStatic.value; i++)
            {
                Thread.Sleep(100);
                GroupBox groupbox = new GroupBox();

                string name_control = "Browser_" + CEFStatic.start_index;


                groupbox.Text = name_control;
                groupbox.Name = name_control;
                groupbox.Width = groupbox_width;
                groupbox.Height = groupbox_height;
                groupbox.ForeColor = Color.White;
                CEFStatic.start_index++;

                //TextBox textBox = new TextBox();
                //textBox.Width = groupbox.Width - 300;
                //textBox.BackColor = Color.FromArgb(31, 30, 68);
                //textBox.ForeColor = Color.White;
                //textBox.Location = new Point(70, 0);
                //groupbox.Controls.Add(textBox);

                //FontAwesome.Sharp.IconButton iconButton = new FontAwesome.Sharp.IconButton();

                //iconButton.FlatAppearance.BorderSize = 0;
                //iconButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                //iconButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                //iconButton.IconChar = FontAwesome.Sharp.IconChar.ArrowRightFromBracket;
                //iconButton.IconColor = System.Drawing.Color.White;
                //iconButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
                //iconButton.IconSize = 30;
                //iconButton.Location = new System.Drawing.Point(565, 82);
                //iconButton.Name = "iconButton";
                //iconButton.Size = new System.Drawing.Size(24, 25);
                //iconButton.TabIndex = 39;
                //iconButton.UseVisualStyleBackColor = true;
                //iconButton.Location = new Point(groupbox.Width - 100, 0);
                //groupbox.Controls.Add(iconButton);



                int row = i / boxes_per_row;
                int col = i % boxes_per_row;
                int x = col * groupbox_width;
                int y = row * groupbox_height;
                groupbox.Location = new Point(x, y);
                TkHelp.AddRow(name_control);
                CEFStatic.panel.Invoke((MethodInvoker)delegate
                {
                    CEFStatic.panel.Controls.Add(groupbox);

                });
                groupBoxes.Add(groupbox);
            }
            return groupBoxes;
        }

        public void SizeChanged()
        {
            int boxes_per_row = CEFStatic.row_number;
            int groupbox_width = CEFStatic.panel.Width / boxes_per_row;
            int groupbox_height = CEFStatic.panel.Height / 2;
            if (CEFStatic.value <= CEFStatic.row_number)
            {
                groupbox_height = CEFStatic.panel.Height;

            }
            CEFStatic.panel.Invoke((MethodInvoker)delegate
            {
                CEFStatic.panel.VerticalScroll.Value = 0;
            });

            for (int i = 0; i < CEFStatic.panel.Controls.Count; i++)
            {
                GroupBox groupbox = CEFStatic.panel.Controls[i] as GroupBox;

                if (groupbox != null)
                {
                    groupbox.Width = groupbox_width;
                    groupbox.Height = groupbox_height;
                    groupbox.Location = new Point((i % boxes_per_row) * groupbox_width, (i / boxes_per_row) * groupbox_height);
                }
            }

        }



        /// <summary>
        /// Cài đặt mặc định cho browser khi mở lên
        /// </summary>
        /// <param name="image">Mở tắt hình ảnh hiển thị (true = tắt)</param>
        /// <param name="webrtc">Mở tắt webrtc  (true = tắt)</param>
        /// <param name="gpu_acceleration">Mở tắt gpu acceleration  (true = tắt)</param>
        /// <param name="closed_all">Đóng tất cả qui trình con nếu qui trình mẹ bị tắt  (true = tắt)</param>
        /// <param name="defaul_useragent">useragent mặc định của browser</param>
        public void DefaultSetting(bool image, bool webrtc, bool gpu_acceleration, bool closed_all, string defaul_useragent)
        {
            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version = string.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}",
             Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);

            //Cài đặt chế độ hỗ trợ x86/64
            CefRuntime.SubscribeAnyCpuAssemblyResolver();
            //Kích hoạt dpi - thông tin thêm tại https://github.com/cefsharp/CefSharp/wiki/General-Usage#high-dpi-displayssupport

            //Phần Setting Browser
            var cef_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CEFDATA\\cef_data");
            var cefset = new CefSettings()
            {
                LogFile = Path.Combine(cef_path, "cef_log.txt"),
                CachePath = Path.Combine(cef_path, "cache"),
                UserDataPath = Path.Combine(cef_path, "userdata")
            };

            //Tắt  WebRTC
            if (webrtc)
                cefset.CefCommandLineArgs.Add("disable-media-stream");
            //Tắt tăng tốc phần cứng gpu acceleration
            if (gpu_acceleration)
                cefset.CefCommandLineArgs.Add("disable-gpu");

            //Tắt hình ảnh trên browser
            if (image)
                cefset.CefCommandLineArgs.Add("disable-image-loading", "1");


            cefset.UserAgent = defaul_useragent;

            Cef.Initialize(cefset, performDependencyCheck: true, browserProcessHandler: null);

            //Thoát quy trình con nếu quy trình mẹ bị tắt
            CefSharpSettings.SubprocessExitIfParentProcessClosed = closed_all;
        }

    }

}
