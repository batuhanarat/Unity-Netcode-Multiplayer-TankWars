using System;
using System.Runtime.Serialization;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace UI.Leaderboard
{
    public struct LeaderboardEntityStruct : INetworkSerializable, IEquatable<LeaderboardEntityStruct>
    {
        public ulong PlayerID;
        public FixedString32Bytes PlayerName;
        public int Coins;
        
        
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerID);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Coins);

        }

        public bool Equals(LeaderboardEntityStruct other)
        {
            return PlayerID == other.PlayerID &&
                   PlayerName.Equals(other.PlayerName) &&
                   Coins == other.Coins;
        }
    }
}