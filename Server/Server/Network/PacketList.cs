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
