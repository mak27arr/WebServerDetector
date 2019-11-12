﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace WebServerDetector.Classes.Helper
{
    public static class IPAddressExtensions
    {
        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }
        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }
        public static IPAddress GetNetworkFirstAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            broadcastAddress[broadcastAddress.Length - 1] += 1;
            return new IPAddress(broadcastAddress);
        }
        public static IPAddress GetNetworkLastAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            broadcastAddress[broadcastAddress.Length - 1] -= 1;
            return new IPAddress(broadcastAddress);
        }
        public static IPAddress GetNextAddress(this IPAddress address)
        {
            int[] ipAdressBytes = address.GetAddressBytes().Select(x => (int)x).ToArray();
            ipAdressBytes[ipAdressBytes.Length - 1] += 1;
            for (int i = ipAdressBytes.Length - 1; i < 0; i++)
            {
                if (ipAdressBytes[i] > 255)
                {
                    ipAdressBytes[i] = ipAdressBytes[i] % 255;
                    ipAdressBytes[i - 1] += ipAdressBytes[i] / 255;
                }
            }
            return new IPAddress(ipAdressBytes.Select(x => (byte)x).ToArray());
        }
        public static IPAddress AddToAddress(this IPAddress address,int count)
        {
            int[] ipAdressBytes = address.GetAddressBytes().Select(x=>(int)x).ToArray();
            ipAdressBytes[ipAdressBytes.Length - 1] += count;
            for (int i = ipAdressBytes.Length - 1; i < 0; i++)
            {
                if (ipAdressBytes[i] > 255)
                {
                    ipAdressBytes[i] = ipAdressBytes[i] % 255;
                    ipAdressBytes[i - 1] += ipAdressBytes[i] / 255;
                }
            }
            return new IPAddress(ipAdressBytes.Select(x=>(byte)x).ToArray());
        }
        public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            IPAddress network1 = address.GetNetworkAddress(subnetMask);
            IPAddress network2 = address2.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }
        public static IPAddress GetSubnetMask(this IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }
        public static int GetPosibleAddressCount(this IPAddress address, IPAddress subnetMask)
        {
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();
            int count = 0;
            foreach(var octet in subnetMaskBytes)
            {
                count += 255 - octet;
            }
            return count-2;
        }
    }
}
