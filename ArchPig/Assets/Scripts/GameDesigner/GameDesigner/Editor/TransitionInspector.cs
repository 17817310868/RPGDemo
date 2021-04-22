using UnityEngine;
using System.Collections;
using UnityEditor;

namespace GameDesigner
{
	[CustomEditor(typeof(Transition) , true)]
	public class TransitionInspector : Editor
	{
		static public Transition transition = null;

		void OnEnable ()
		{
			transition = target as Transition;
			transition.transform.hideFlags = HideFlags.HideInInspector;
			transition.transform.localPosition = Vector3.zero;
		}

		public override void OnInspectorGUI ()
		{
			StateManagerEditor.DrawTransition ( transition );
		}
	}
}