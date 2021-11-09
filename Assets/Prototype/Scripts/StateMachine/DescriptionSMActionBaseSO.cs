using UnityEngine;

namespace StateMachine
{
	/// <summary>
	/// Base class for StateMachine ScriptableObjects that need a public description field.
	/// </summary>
	public class DescriptionSMActionBaseSO : ScriptableObject
	{
		[TextArea] public string description;
	}

}