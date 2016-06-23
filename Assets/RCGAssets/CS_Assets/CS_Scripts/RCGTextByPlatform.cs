using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RoadCrossing
{
	/// <summary>
	/// RCG text by platform.
	/// </summary>
	public class RCGTextByPlatform : MonoBehaviour
	{

		// The text that will be displayed on PC/Mac/Webplayer
		public string computerText = "CLICK TO START";
	
		// The text that will be displayed on Android/iOS/WinPhone
		public string mobileText = "TAP TO START";
	
		// The text that will be displayed on Playstation, Xbox, Wii
		public string consoleText = "PRESS 'A' TO START";
	
		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start()
		{
			#if UNITY_STANDALONE || UNITY_WEBPLAYER
			GetComponent<Text>().text = computerText;
			#endif
		
			#if IPHONE || ANDROID || UNITY_BLACKBERRY || UNITY_WP8 || UNITY_METRO
			GetComponent<Text>().text = mobileText;
			#endif
		
			#if UNITY_WII || UNITY_PS3 || UNITY_XBOX360
			GetComponent<Text>().text = consoleText;
			#endif
		}
	}
}