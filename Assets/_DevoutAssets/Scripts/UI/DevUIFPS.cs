using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class DevUIFPS : MonoBehaviour {
	Text _text;
	float deltaTime = 0.0f;

//	void Update()
//	{
//		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
//	}
	// Use this for initialization
	void Start () {
		_text = GetComponent<Text> ();
		InvokeRepeating ("updateUIFPS", 0.1f, 0.1f);
	}
	void updateUIFPS(){
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;	
		string content = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
//		_text.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		content += "\nPing : " + PhotonNetwork.GetPing();
		_text.text = content + "\nResentRCommands : " + PhotonNetwork.ResentReliableCommands;
	}
}
