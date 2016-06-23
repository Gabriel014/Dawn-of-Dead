//This script toggles a sound source when clicked on. It also records the sound state (on/off) in a PlayerPrefs. In order to detect clicks you need to attach a collider to this object.
#pragma strict

//The source of the sound
var soundObject:Transform;

//The PlayerPrefs name of the sound
var playerPref:String = "SoundVolume";

//The index of the current value of the sound
internal var currentState:float = 1;

function Awake() 
{
	//Get the current state of the sound from PlayerPrefs
	if ( soundObject )    currentState = PlayerPrefs.GetInt( playerPref, soundObject.GetComponent.<AudioSource>().volume);
	else    currentState = PlayerPrefs.GetInt( playerPref, currentState);
	
	//Set the sound in the sound source
	SetSound();
}

function SetSound()
{
	//Set the sound in the PlayerPrefs
	PlayerPrefs.SetInt( playerPref, currentState);
	
	//Update the graphics of the button image to fit the sound state
	if ( currentState == 1 )    GetComponent(Image).color.a = 1;
	else    GetComponent(Image).color.a = 0.5;
	
	//Set the value of the sound state to the source object
	if ( soundObject )    soundObject.GetComponent.<AudioSource>().volume = currentState;
}

//Toggle the sound. Cycle through all sound modes and set the volume and icon accordingly
function ToggleSound()
{
	currentState = 1 - currentState;
	
	SetSound();
}

//Start playing the sound source
function StartSound()
{	
	if ( soundObject )    soundObject.GetComponent.<AudioSource>().Play();
}