//This script allows the user to customize the a text string based on the platform type.
#pragma strict

//The text that will be displayed on PC/Mac/Webplayer
var computerText:String = "CLICK TO START";

//The text that will be displayed on Android/iOS/WinPhone
var mobileText:String = "TAP TO START";

//The text that will be displayed on Playstation, Xbox, Wii
var consoleText:String = "PRESS 'A' TO START";

function Start() 
{
	#if UNITY_STANDALONE || UNITY_WEBPLAYER
		GetComponent(Text).text = computerText;
	#endif
	
	#if IPHONE || ANDROID || UNITY_BLACKBERRY || UNITY_WP8 || UNITY_METRO
		GetComponent(Text).text = mobileText;
	#endif
	
	#if UNITY_WII || UNITY_PS3 || UNITY_XBOX360
		GetComponent(Text).text = consoleText;
	#endif
}