using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;

namespace TicketRusherProj.Html
{
    /// <summary>
    /// 网络工具（提供获取本地所有网卡IP等信息）
    /// </summary>
    public class NetHelper
    {
        private static IList<IPAddress> address = new List<IPAddress>();

        /// <summary>
        /// 获取本机的所有IP
        /// </summary>
        /// <returns></returns>
        public static IList<IPAddress> GetLoaclAddressList()
        {
            if (address.Count == 0)
            {
                //IList<IPAddress> address = new List<IPAddress>();
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection allAddress = adapterProperties.UnicastAddresses;
                    if (allAddress.Count > 0)
                    {
                        foreach (UnicastIPAddressInformation addr in allAddress)
                        {
                            address.Add(addr.Address);
                        }
                    }
                }
            }
            return address;
        }

        /// <summary>
        /// 获取本机的所有IP4
        /// </summary>
        /// <returns></returns>
        public static IList<IPAddress> GetIp4LoaclAddressList()
        {
            if (address.Count == 0)
            {
                //IList<IPAddress> address = new List<IPAddress>();
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection allAddress = adapterProperties.UnicastAddresses;
                    if (allAddress.Count > 0)
                    {
                        foreach (UnicastIPAddressInformation addr in allAddress)
                        {
                            if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                                address.Add(addr.Address);
                        }
                    }
                }
            }
            else
            {
                address = address.Where(p => p.AddressFamily == AddressFamily.InterNetwork).ToList();
            }
            return address;
        }

        /// <summary>
        /// 返回与当前IP相差最小的本地IP地址
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public static string GetSimilarAddresses(string Address)
        {
            byte[] _StarByte = System.Net.IPAddress.Parse(Address).GetAddressBytes();
            uint serverIPAddressValue = BitConverter.ToUInt32(new byte[] { _StarByte[3], _StarByte[2], _StarByte[1], _StarByte[0] }, 0);

            List<IPSInfo> ips = new List<IPSInfo>();

            var localIPAddress = NetHelper.GetLoaclAddressList().Where(o => o.AddressFamily == AddressFamily.InterNetwork).ToList<IPAddress>();
            foreach (var ip in localIPAddress)
            {
                byte[] _ValueByte = System.Net.IPAddress.Parse(ip.ToString()).GetAddressBytes();
                uint _Value = BitConverter.ToUInt32(new byte[] { _ValueByte[3], _ValueByte[2], _ValueByte[1], _ValueByte[0] }, 0);
                ips.Add(new IPSInfo { IP = ip.ToString(), VL = serverIPAddressValue - _Value });
            }
            var query = from o in ips
                        orderby o.VL
                        select o;

            return query.FirstOrDefault().IP;
        }

        /// <summary>
        /// 判断指定端口是否已经被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool PortInUse(int port)
        {
            bool inUse = false;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }

        /// <summary>
        /// 判断一个网络资源是否存在,如果网络异常该方法可能存在0.01秒的延时,所以尽可能异步调用
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool UrlIsExist(string url)
        {
            System.Uri u = null;
            //exception = null;
            try
            {
                u = new Uri(url);
            }
            catch { return false; }

            bool isExist = false;

            System.Net.HttpWebRequest r = System.Net.HttpWebRequest.Create(u) as System.Net.HttpWebRequest;
            System.Net.HttpWebResponse s = null;
            r.Proxy = null;
            r.Timeout = 2000;
            r.Method = "HEAD";
            try
            {
                s = r.GetResponse() as System.Net.HttpWebResponse;
                isExist = (s.StatusCode == System.Net.HttpStatusCode.OK);
            }
            catch (System.Net.WebException x)
            {
                //exception = x;
                try
                {
                    isExist = ((x.Response as System.Net.HttpWebResponse).StatusCode != System.Net.HttpStatusCode.NotFound);
                    x.Response.Close();
                }
                catch
                {
                    isExist = (x.Status == System.Net.WebExceptionStatus.Success);
                }
            }
            finally
            {
                if (s != null)
                    s.Close();
            }

            return isExist;
        }

        /// <summary>
        /// 保存网络地址文件到指定目录
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public static bool SaveFile(string fileUrl, string savePath)
        {
            try
            {
                System.IO.Directory.CreateDirectory(savePath.Replace(System.IO.Path.GetFileName(savePath), string.Empty));
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(fileUrl, savePath);
                    wc.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// IP地址信息（包含字符串型IP地址和与之对应的long地址）
    /// </summary>
    public class IPSInfo
    {
        public string IP { get; set; }
        public long VL { get; set; }
    }
}