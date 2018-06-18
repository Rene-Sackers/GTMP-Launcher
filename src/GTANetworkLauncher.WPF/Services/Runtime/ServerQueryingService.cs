using System;
using System.IO;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using GrandTheftMultiplayer.Shared;
using Lidgren.Network;
using ProtoBuf;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class ServerQueryingService : IServerQueryingService
    {
        private const string NetClientId = "GRANDTHEFTMULTIPLAYER";
        private const int DiscoveryTimeoutInMilliseconds = 1000;
        private const int DiscoveryLoopSleepInMilliseconds = 50;
        private const int MaxServerRefreshAtOnce = 1;

        private readonly TaskQueue _refreshQueue = new TaskQueue(MaxServerRefreshAtOnce);
        private readonly NetClient _queryingNetClient = new NetClient(new NetPeerConfiguration(NetClientId));

        public ServerQueryingService()
        {
            _queryingNetClient.Start();
        }
        
        public Task<DiscoveryResponse> QueryServer(Models.ServerApi.Server server)
        {
            // Lidgren does not support IPv6
            if (server.IsIpv6) return null;

            return _refreshQueue.Enqueue(() => QueryTask(server.Ip, server.Port));
        }

        public async Task ClearRefreshQueue()
        {
            await _refreshQueue.ClearQueue();
        }

        public async Task<DiscoveryResponse> QueryTask(string address, int port)
        {
            try
            {
                return await TryQueryServer(address, port);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private async Task<DiscoveryResponse> TryQueryServer(string address, int port)
        {
            if (!_queryingNetClient.DiscoverKnownPeer(address, port)) return null;

            var queryStartTime = Environment.TickCount;

            do
            {
                NetIncomingMessage msg;
                while ((msg = _queryingNetClient.ReadMessage()) != null)
                {
                    if (msg.MessageType == NetIncomingMessageType.DiscoveryResponse)
                    {
                        msg.ReadByte(); // Discovery type byte
                        var messageBodyLength = msg.ReadInt32();
                        var messageBody = msg.ReadBytes(messageBodyLength);

                        return DeserializeBinary<DiscoveryResponse>(messageBody);
                    }

                    _queryingNetClient.Recycle(msg);
                }

                await Task.Delay(DiscoveryLoopSleepInMilliseconds);
            } while (Environment.TickCount - queryStartTime < DiscoveryTimeoutInMilliseconds);

            return null;
        }

        private static T DeserializeBinary<T>(byte[] data) where T : class
        {
            using (var stream = new MemoryStream(data))
            {
                try
                {
                    return Serializer.Deserialize<T>(stream);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }
    }
}