using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TimeTag
{
    public class HelperInternet
    {
        public static bool IsOnline()
        {
            var pinger = new Ping();

            try
            {
                return pinger.Send("google.dk").Status == IPStatus.Success;
            }
            catch (SocketException) { return false; }
            catch (PingException) { return false; }
        }
    }
}
