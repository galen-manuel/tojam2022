using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviour {

	private static DontDestroy s_instance;
	void Start()
	{
		if (s_instance == null)
        {
			s_instance = this;
			//Causes UI object not to be destroyed when loading a new scene. If you want it to be destroyed, destroy it manually via script.
			DontDestroyOnLoad(this.gameObject);
		}
		else
        {
			Destroy(transform.root.gameObject);
        }
	}
}
