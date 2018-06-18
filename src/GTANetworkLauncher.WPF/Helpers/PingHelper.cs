using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GrandTheftMultiplayer.Launcher.Helpers
{
    public class PingHelper
    {
        /// <summary>
        /// enum to hold the possible connection states
        /// </summary>
        [Flags]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private enum ConnectionStatusEnum
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }

        [DllImport("wininet", CharSet = CharSet.Auto)]
        private static extern bool InternetGetConnectedState(ref ConnectionStatusEnum flags, int dw);

        /// <summary>
        /// method to check the status of the pinging machines internet connection
        /// </summary>
        /// <returns></returns>
        private static bool HasConnection()
        {
            ConnectionStatusEnum state = 0;
            InternetGetConnectedState(ref state, 0);

            return ((int)ConnectionStatusEnum.INTERNET_CONNECTION_OFFLINE & (int)state) == 0;
        }

        /// <summary>
        /// method for retrieving the IP address from the host provided
        /// </summary>
        /// <param name="host">the host we need the address for</param>
        /// <returns></returns>
        private static IPAddress GetIpFromHost(ref string host)
        {
            IPAddress address;
            try
            {
                address = Dns.GetHostEntry(host).AddressList[0];
            }
            catch (SocketException)
            {
                return null;
            }
            return address;
        }

        public static string PingHost(string host)
        {
            var returnMessage = string.Empty;
            var address = IPAddress.Parse(host);
            var pingOptions = new PingOptions(128, true);
            var ping = new Ping();

            //32 byte buffer (create empty)
            var buffer = new byte[32];

            //first make sure we actually have an internet connection
            if (!HasConnection())
                returnMessage = "No Internet connection found...";

            //here we will ping the host 4 times (standard)
            for (var i = 0; i < 4; i++)
            {
                try
                {
                    //send the ping 4 times to the host and record the returned data.
                    //The Send() method expects 4 items:
                    //1) The IPAddress we are pinging
                    //2) The timeout value
                    //3) A buffer (our byte array)
                    //4) PingOptions
                    var pingReply = ping.Send(address, 1000, buffer, pingOptions);

                    //make sure we dont have a null reply
                    if (pingReply != null)
                    {
                        switch (pingReply.Status)
                        {
                            case IPStatus.Success:
                                returnMessage =
                                    $"Reply from {pingReply.Address}: bytes={pingReply.Buffer.Length} time={pingReply.RoundtripTime}ms TTL={pingReply.Options.Ttl}";
                                break;
                            case IPStatus.TimedOut:
                                returnMessage = "Connection has timed out...";
                                break;
                            default:
                                returnMessage = $"Ping failed: {pingReply.Status}";
                                break;
                        }
                    }
                    else
                        returnMessage = "Connection failed for an unknown reason...";
                }
                catch (PingException ex)
                {
                    returnMessage = $"Connection Error: {ex.Message}";
                }
                catch (SocketException ex)
                {
                    returnMessage = $"Connection Error: {ex.Message}";
                }
            }

            //return the message
            return returnMessage;
        }

        public static long GetPing(string host)
        {
            if (!IPAddress.TryParse(host, out IPAddress address))
                address = GetIpFromHost(ref host);

            var pingOptions = new PingOptions(128, true);
            var ping = new Ping();
            var buffer = new byte[32];

            if (!HasConnection()) return -1;

            try
            {
                var pingReply = ping.Send(address, 1000, buffer, pingOptions);
                if (pingReply == null) return -1;

                return pingReply.Status == IPStatus.Success ? pingReply.RoundtripTime : -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static async Task<long> GetPingAsync(string host)
        {
            return await Task.Factory.StartNew(() => GetPing(host));
        }
    }
}
