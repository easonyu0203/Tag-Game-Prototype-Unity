using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{

    [CreateAssetMenu(fileName = "New State", menuName = "State Machines/State")]
    public class StateSO : ScriptableObject{
        public ActionOS[] Actions = null;
    }

    public class State
    {
        public StateSO OriginSO;
        public StateMachine StateMachine;
        
    }
    
}
