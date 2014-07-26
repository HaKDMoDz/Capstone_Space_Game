using UnityEngine;
using System.Collections;

public abstract class SingletonComponent<T> : MonoBehaviour where T : SingletonComponent<T>
{
	private static T instance = null;
	public static T Instance
	{
		get { return instance; }
	}
	protected virtual void Awake()
	{
		if (instance != null)
		{
			Debug.LogError(name + ": error: already initialized", this);
		}
		instance = (T)this;
	}
}