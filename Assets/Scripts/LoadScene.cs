using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

	[SerializeField] string scene;	

	public void loadNextScene() {
		SceneManager.LoadScene(scene, LoadSceneMode.Additive);
	}

}
