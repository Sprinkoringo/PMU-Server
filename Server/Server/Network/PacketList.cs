/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PMU.Core;
using PMU.Sockets;


namespace Server.Network
{
    public class PacketList
    {
        public List<TcpPacket> Packets { get; set; }

        public PacketList() {
            Packets = new List<TcpPacket>();
        }

        public void AddPacket(TcpPacket packet) {
            Packets.Add(packet);
        }
        
        public void RemovePacket(TcpPacket packet) {
        	Packets.Remove(packet);
        }
        
        public bool ContainsPacket(string packetHeader) {
        	foreach (TcpPacket packet in Packets) {
        		if (packet.Header == packetHeader) {
        			return true;
        		}
        	}
        	return false;
        }
        
        public TcpPacket FindPacket(params string[] parameters) {
        	foreach (TcpPacket packet in Packets) {
        		string[] parse = packet.PacketString.Split(packet.SeperatorChar);
        		bool matches = true;
        		if (parse.Length >= parameters.Length) {
        			for (int i = 0; i < parameters.Length; i++) {
        				if (parameters[i] != null && parameters[i] != parse[i]) {
        					matches = false;
        				}
        			}
        			
        			if (matches) {
        				return packet;
        			}
        		}
        	}
        	return null;
        }
        
        

        public byte[] CombinePackets() {
            ByteArray[] packetBytes = new ByteArray[Packets.Count];
            int totalSize = 0;
            for (int i = 0; i < Packets.Count; i++) {
                packetBytes[i] = new ByteArray(ByteEncoder.StringToByteArray(Packets[i].PacketString));
                totalSize += packetBytes[i].Length() + GetPacketSegmentHeaderSize();
            }
            byte[] packet = new byte[totalSize];
            int position = 0;
            for (int i = 0; i < packetBytes.Length; i++) {
                // Add the size of the packet segment
                Array.Copy(ByteArray.IntToByteArray(packetBytes[i].Length()), 0, packet, position, 4);
                position += 4;
                // Add the packet data
                Array.Copy(packetBytes[i].ToArray(), 0, packet, position, packetBytes[i].Length());
                position += packetBytes[i].Length();
            }
            return packet;
        }

        public int GetPacketSegmentHeaderSize() {
            return
                4 // [int32] Size of the packet segment
                ;
        }
    }
}
