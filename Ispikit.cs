using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Timers;

// This is boilerplate code to start coding using the Ispikit Unity plugin
// It assumes it belongs to a "GameObject" game object
public class Ispikit : MonoBehaviour {

// Below are the available plugin calls
#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern int startInitialization(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setPlaybackDoneCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setResultCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setCompletionCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setNewWordsCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setNewAudioCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int startRecording(string sentences);
	[DllImport("__Internal")]
	private static extern int stopRecording();
	[DllImport("__Internal")]
	private static extern int setStrictness(int strictness);
	[DllImport("__Internal")]
	private static extern int startPlayback();
	[DllImport("__Internal")]
	private static extern int stopPlayback();
	[DllImport("__Internal")]
	private static extern int addWord(string word, string pronunciation);
#endif
	private static System.Timers.Timer timer;
	void Awake () {

		// As soon as Game Object is awake, we start initializing the plugin
		Debug.Log ("About to initialize plugin");
		string gameObjectName = "GameObject";
		string callbackName = "initCallback";
		// When calling the startInitialization function, we provide the name
		// of the Game Object and Callback for when init is done
		Debug.Log (startInitialization(gameObjectName, callbackName));
		Debug.Log ("Initialization started");

		//audio permissions check
		AudioSource audPermissions = GetComponent<AudioSource>();
		audPermissions.clip = Microphone.Start("Built-in Microphone", true, 1, 1);

	}

	void Update () {
	}
	public void initCallback(string status) {
		// This is for when plugin is initialized, status should be "0"
		// if successful
		Debug.Log ("Plugin initialization done");
		Debug.Log (status);
		// We now register all callbacks
		setPlaybackDoneCallback ("GameObject", "playbackDoneCallback");
		setResultCallback ("GameObject", "resultCallback");
		setCompletionCallback ("GameObject", "completionCallback");
		setNewWordsCallback ("GameObject", "newWordsCallback");
		setNewAudioCallback ("GameObject", "newAudioCallback");
		// We then start recording, with three possible inputs: "first", "second", and "third"
		startRecording ("first,second,third");
		Debug.Log ("Starting recording");
		// Three seconds later, we wiĺl stop recording in the provided onRecordingDone function
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private static void onRecordingDone(object source, ElapsedEventArgs e) {
		// This just stops recording. In the background, analysis will start
		// and result callback will be called once done.
		Debug.Log ("Stopping recording");
		stopRecording ();
	}
	public void resultCallback(string status) {
		// Callback when result is available, a few seconds after stopRecording, typically.
		// See docs on how to parse the result.
		Debug.Log ("Result");
		Debug.Log (status);
		// Starts replaying the userś voice after one second
		timer = new System.Timers.Timer (1000);
		timer.Elapsed += onStartPlayback;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private static void onStartPlayback(object source, ElapsedEventArgs e) {
		// This just starts playing back previously recorded audio
		// once audio is played back, it calls back using the provide callback
		// function
		Debug.Log ("Starting playback");
		startPlayback();
	}
	public void playbackDoneCallback(string status) {
		// This is called once playback is done. It will start another timer
		// after which recording will start again.
		Debug.Log ("Playback Done");
		Debug.Log (status);
		timer = new System.Timers.Timer (1000);
		timer.Elapsed += onRestarting;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private static void onRestarting(object source, ElapsedEventArgs e) {
		// This is a function to be called by a timer to start another recognition
		// This boilerplate application will continue looping indefinitely through
		// recording - analysis - playback cycles
		Debug.Log ("Starting recording");
		startRecording ("I am learning English,hello goodbye,one two three four");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	public void completionCallback(string status) {
		// This callback is called during analysis
		// Status is between "0" and "100", it is the percentage of completion of
		// analysis, it can be used to display a progress bar.
		Debug.Log ("Completion");
		Debug.Log (status);
	}
	public void newWordsCallback(string status) {
		// This callback comes during recording, it gives the words recognized
		// see docs on how to parse the string
		Debug.Log ("New words");
		Debug.Log (status);
	}
	public void newAudioCallback(string status) {
		// This callback also comes during recording, it gives data about the recording
		// that can be used for UI effects: audio volume, pitch and waveform
		// see docs on how to parse it
		Debug.Log ("New audio");
		Debug.Log (status);
	}
}
