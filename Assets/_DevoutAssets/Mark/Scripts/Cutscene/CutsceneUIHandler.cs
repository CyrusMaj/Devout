using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CutsceneUIHandler : MonoBehaviour {

	public Image _img;
	public Text _text;

	public void setImg(Image img)
	{
		_img = img;
	}

	public void setText(string text)
	{
		_text.text = text;

	}
}
