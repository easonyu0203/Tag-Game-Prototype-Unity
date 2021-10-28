using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config{

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NetworkSetting")]
    public class NetworkSetting : ScriptableObject
    {
        public int MaxClientCount;
    }

}
