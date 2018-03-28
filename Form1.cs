using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Web;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Diagnostics;


namespace FileUpload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var url = "http://127.0.0.1:8080/upload/batch";
            //var file1 = @"F:\学习资料\11：AJax课程\01 Ajax_原理和封装\妙味云课堂-05 Ajax原理-ajax流程-数据的获取_5.avi";
            var file1 = @"E:\壁纸\1.jpg";
            var file2 = @"E:\壁纸\back.jpg";
            var file3 = @"E:\壁纸\lyc.jpg";
            var file4 = @"F:\学习资料\11：AJax课程\01 Ajax_原理和封装\妙味云课堂-05 Ajax原理-ajax流程-数据的获取_5.avi";
            var file5 = @"F:\学习资料\11：AJax课程\01 Ajax_原理和封装\妙味云课堂-02 Ajax原理-第一个ajax程序_2.avi";
            var file6 = @"E:\公司项目汇总\立元视频客户端\geap参考软件\GlobalEye\Video\长盛路新昌南路西电警\20180316-145243-1.asf";
            var file7 = @"E:\公司项目汇总\立元视频客户端\geap参考软件\GlobalEye\Video\长盛路新昌南路西电警\20180316-145154-1.asf";
            var formDatas = new List<FormItemModel>();
            //添加文件  
            formDatas.Add(new FormItemModel()
            {
                Key = "fileList",
                Value = "",
                //FileName = "妙味云课堂-05 Ajax原理-ajax流程-数据的获取_5.avi",
                FileName = "1.jpg",
                FileContent = File.OpenRead(file1)
            });
            formDatas.Add(new FormItemModel()
            {
                Key = "fileList",
                Value = "",
                FileName = "back.jpg",
                FileContent = File.OpenRead(file2)
            });
            formDatas.Add(new FormItemModel()
            {
                Key = "fileList",
                Value = "",
                FileName = "lyc.jpg",
                FileContent = File.OpenRead(file3)
            });
            formDatas.Add(new FormItemModel()
            {
                Key = "fileList",
                Value = "",
                FileName = "妙味云课堂-05 Ajax原理-ajax流程-数据的获取_5.avi",
                FileContent = File.OpenRead(file4)
            });
            formDatas.Add(new FormItemModel()
            {
                Key = "fileList",
                Value = "",
                FileName = "妙味云课堂-02 Ajax原理-第一个ajax程序_2.avi",
                FileContent = File.OpenRead(file5)
            });
            formDatas.Add(new FormItemModel()
            {
                Key = "fileList",
                Value = "",
                FileName = "20180316-145243-1.asf",
                FileContent = File.OpenRead(file6)
            });
            formDatas.Add(new FormItemModel()
            {
                Key = "fileList",
                Value = "",
                FileName = "20180316-145154-1.asf",
                FileContent = File.OpenRead(file7)
            });
            //提交表单  
            string result = PostForm(url, formDatas);
            MessageBox.Show(result);

        }

        /// <summary>  
        /// 使用Post方法获取字符串结果  
        /// </summary>  
        /// <param name="url"></param>  
        /// <param name="formItems">Post表单内容</param>  
        /// <param name="cookieContainer"></param>  
        /// <param name="timeOut">默认20秒</param>  
        /// <param name="encoding">响应内容的编码类型（默认utf-8）</param>  
        /// <returns></returns>  
        private static string PostForm(string url, List<FormItemModel> formItems, CookieContainer cookieContainer = null, string refererUrl = null, Encoding encoding = null, int timeOut = 120000)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            #region 初始化请求对象
            request.Method = "POST";
            request.Timeout = timeOut;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";
            if (!string.IsNullOrEmpty(refererUrl))
                request.Referer = refererUrl;
            if (cookieContainer != null)
                request.CookieContainer = cookieContainer;
            #endregion

            string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分隔符  
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            //request.ContentType = string.Format("text/html;charset=UTF-8;boundary={0}", boundary);
            //请求流  
            var postStream = new MemoryStream();
            #region 处理Form表单请求内容
            //是否用Form上传文件  
            var formUploadFile = formItems != null && formItems.Count > 0;
            if (formUploadFile)
            {
                //文件数据模板  
                string fileFormdataTemplate =
                    "\r\n--" + boundary +
                    "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
                    "\r\nContent-Type: application/octet-stream" +
                    "\r\n\r\n";
                //文本数据模板  
                string dataFormdataTemplate =
                    "\r\n--" + boundary +
                    "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                    "\r\n\r\n{1}";
                foreach (var item in formItems)
                {
                    string formdata = null;
                    if (item.IsFile)
                    {
                        //上传文件  
                        formdata = string.Format(
                            fileFormdataTemplate,
                            item.Key, //表单键  
                            item.FileName);
                    }
                    else
                    {
                        //上传文本  
                        formdata = string.Format(
                            dataFormdataTemplate,
                            item.Key,
                            item.Value);
                    }

                    //统一处理  
                    byte[] formdataBytes = null;
                    //第一行不需要换行  
                    if (postStream.Length == 0)
                        formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));
                    else
                        formdataBytes = Encoding.UTF8.GetBytes(formdata);
                    postStream.Write(formdataBytes, 0, formdataBytes.Length);

                    //写入文件内容  
                    if (item.FileContent != null && item.FileContent.Length > 0)
                    {
                        using (var stream = item.FileContent)
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead = 0;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                postStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
                //结尾  
                var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                postStream.Write(footer, 0, footer.Length);

            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            #endregion

            request.ContentLength = postStream.Length;

            #region 输入二进制流
            if (postStream != null)
            {
                postStream.Position = 0;
                //直接写入流  
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                ////debug  
                //postStream.Seek(0, SeekOrigin.Begin);  
                //StreamReader sr = new StreamReader(postStream);  
                //var postStr = sr.ReadToEnd();  
                postStream.Close();//关闭文件访问  
            }
            #endregion

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (cookieContainer != null)
            {
                response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
            }

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(responseStream, encoding ?? Encoding.UTF8))
                {
                    string retString = myStreamReader.ReadToEnd();
                    Console.WriteLine(retString);
                    return retString;
                }
            }
        }
    }
}
