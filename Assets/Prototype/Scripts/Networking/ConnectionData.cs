using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Networking{

    [Serializable]
    public class ConnectionData
    {
        [Header("Attributes")]
        public string Name;

        public ConnectionData(string name){
            Name = name;
        }

        /// <summary>
        /// the way of encoding
        /// </summary>
        static private Encoding unicode = Encoding.Unicode;

        /// <summary>
        /// Encode a Connection Data to byte[]
        /// </summary>
        /// <param name="connectionData">connectionData</param>
        /// <returns>payload</returns>
        static public byte[] Encode(ConnectionData connectionData){
            string payload_json = JsonUtility.ToJson(connectionData);
            byte[] payload_byte = unicode.GetBytes(payload_json);
            return payload_byte;
        }

        /// <summary>
        /// Decode byte[] to ConnectionData
        /// </summary>
        /// <param name="payload">payload</param>
        /// <returns>connectionData</returns>
        static public ConnectionData Decode(byte[] payload){
            string payload_str = unicode.GetString(payload);
            ConnectionData connectionData = JsonUtility.FromJson<ConnectionData>(payload_str);
            return connectionData;
        }
    }

}
