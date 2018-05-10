using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On 2016-12-15
/// 
/// <summary>
/// Gets a list of characters currently in this area(entered and not exited collider attached to this object)
/// </summary>
[RequireComponent(typeof(Collider))]
public class CharactersInArea : MonoBehaviour {
	/// <summary>
	/// List of characters
	/// </summary>
	List<CharacterStatusHandler> _chrInArea = new List<CharacterStatusHandler> ();
	/// <summary>
	/// Gets the list of characters currently in this area.
	/// </summary>
	/// <returns>The characters in area.</returns>
	public List<CharacterStatusHandler> GetCharactersInArea(){
		return _chrInArea;
	}
	void OnTriggerEnter(Collider other) {
		CharacterStatusHandler _chr = other.GetComponent<CharacterStatusHandler> ();
		if (_chr != null) {
			if (!_chrInArea.Contains (_chr)) {
				_chrInArea.Add (_chr);
			}
		}
	}
	void OnTriggerExit(Collider other) {
		CharacterStatusHandler _chr = other.GetComponent<CharacterStatusHandler> ();
		if (_chr != null) {
			if (_chrInArea.Contains (_chr)) {
				_chrInArea.Remove (_chr);
			}
		}		
	}
}
