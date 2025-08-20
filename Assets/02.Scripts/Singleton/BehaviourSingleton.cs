using UnityEngine;
using System.Collections.Generic;

public class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T i = null;

	public static T Instance
	{
		get
		{
			if (i == null)
			{
				i = FindFirstObjectByType<T>(FindObjectsInactive.Include);
			}
			return i;
		}
		set
		{
			i = value;
		}
	}
}
