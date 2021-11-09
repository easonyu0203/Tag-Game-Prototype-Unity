using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{

    public abstract class ActionOS: DescriptionSMActionBaseSO{
		/// <summary>
		/// Will create a new custom <see cref="StateAction"/> or return an existing one inside <paramref name="createdInstances"/>
		/// </summary>
		public Action GetAction(StateMachine stateMachine, Dictionary<ScriptableObject, object> createdInstances)
		{
			if (createdInstances.TryGetValue(this, out var obj))
				return (Action)obj;

			var action = CreateAction();
			createdInstances.Add(this, action);
			action.OriginSO  = this;
			action.Awake(stateMachine);
			return action;
		}
		public abstract Action CreateAction();
    }

    public abstract class Action
    {
        public ActionOS OriginSO;
        public virtual void Awake(StateMachine stateMachine) {}

        public virtual void OnStateEnter() {}
        public virtual void OnStateExit() {}
        public virtual void OnUpdate() {}
    }
    
}
