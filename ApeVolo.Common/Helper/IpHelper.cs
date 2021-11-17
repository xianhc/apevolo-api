using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using ApeVolo.Common.DI;
using ApeVolo.Common.WebApp;
using IP2Region;
using Shyjus.BrowserDetection;

namespace ApeVolo.Common.Helper
{
    /// <summary>
    /// Ip地址帮助类
    /// </summary>
    public static class IpHelper
    {
        static string _contentRoot = string.Empty;

        #region 外部接口

        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;
                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    return address.Address.ToString();
                }
            }

            return "";
        }

        /// <summary>
        /// 获取第一个可用的端口号
        /// </summary>
        /// <returns></returns>
        public static int GetFirstAvailablePort()
        {
            int BEGIN_PORT = 1024; //从这个端口开始检测
            int MAX_PORT = 65535; //系统tcp/udp端口数最大是65535            

            for (int i = BEGIN_PORT; i < MAX_PORT; i++)
            {
                if (PortIsAvailable(i)) return i;
            }

            return -1;
        }

        /// <summary>
        /// 检查指定端口是否已用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool PortIsAvailable(int port)
        {
            bool isAvailable = true;

            IList portUsed = PortIsUsed();

            foreach (int p in portUsed)
            {
                if (p == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            if (HttpContextCore.CurrentHttpContext.Connection.RemoteIpAddress != null)
                return HttpContextCore.CurrentHttpContext.Connection.RemoteIpAddress.ToString();

            return "0.0.0.0";
        }

        /// <summary>
        /// 获取IP详细地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            try
            {
                string ip = GetIp();
                string pattern = @"^(([1-9]\d?)|(1\d{2})|(2[01]\d)|(22[0-3]))(\.((1?\d\d?)|(2[04]/d)|(25[0-5]))){3}$";
                if (!Regex.IsMatch(ip, pattern))
                {
                    return "局域网IP";
                }

                string filePath = Path.Combine(_contentRoot, "wwwroot", "resources", "ip", "ip2region.db");

                using var search = new DbSearcher(filePath);
                var address = search.MemorySearch(ip).Region.Replace("0|", "");

                return (address.Substring(address.Length - 4, 4)) == "内网IP" ? "局域网IP" : address;
            }
            catch
            {
                // ignored
            }

            return "";
        }

        /// <summary>
        /// 获取端浏览器名称
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserName()
        {
            try
            {
                var browser = AutofacHelper.GetScopeService<IBrowserDetector>();
                return browser.Browser.Name;
            }
            catch
            {
                // ignored
            }

            return "";
        }

        /// <summary>
        /// 获取浏览器对象 包括类型 浏览器 版本 系统
        /// </summary>
        /// <returns></returns>
        public static object GetBrowser()
        {
            return AutofacHelper.GetScopeService<IBrowserDetector>();
        }

        #endregion

        #region 私有成员

        /// <summary>
        /// 获取操作系统已用的端口号
        /// </summary>
        /// <returns></returns>
        private static IList PortIsUsed()
        {
            //获取本地计算机的网络连接和通信统计数据的信息
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            //返回本地计算机上的所有Tcp监听程序
            IPEndPoint[] ipsTcp = ipGlobalProperties.GetActiveTcpListeners();

            //返回本地计算机上的所有UDP监听程序
            IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();

            //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            IList allPorts = new ArrayList();
            foreach (IPEndPoint ep in ipsTcp) allPorts.Add(ep.Port);
            foreach (IPEndPoint ep in ipsUDP) allPorts.Add(ep.Port);
            foreach (TcpConnectionInformation conn in tcpConnInfoArray) allPorts.Add(conn.LocalEndPoint.Port);

            return allPorts;
        }

        #endregion
    }
}