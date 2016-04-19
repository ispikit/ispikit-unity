using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Timers;

// This is boilerplate code to start coding using the Ispikit Unity plugin
// On iOS, it assumes it belongs to a "GameObject" game object

[RequireComponent (typeof (AudioSource))]

public class Ispikit : MonoBehaviour {

// Below are the available plugin calls for all supported platforms

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
#elif UNITY_ANDROID
	public delegate void initCallbackDelegate(int n);
	public delegate void resultCallbackDelegate(int score, int speed, string words);
	public delegate void completionCallbackDelegate(int completion);
	public delegate void newWordsCallbackDelegate(string words);
	public delegate void newAudioCallbackDelegate(int volume, string pitch, string waveform);
	public delegate void playbackDoneCallbackDelegate();

	[DllImport("upal")]
	private static extern int startInitialization(initCallbackDelegate icb, string path);
	[DllImport("upal")]
	private static extern int setPlaybackDoneCallback(playbackDoneCallbackDelegate pcb);
	[DllImport("upal")]
	private static extern int setResultCallback(resultCallbackDelegate rcb);
	[DllImport("upal")]
	private static extern int setCompletionCallback(completionCallbackDelegate ccb);
	[DllImport("upal")]
	private static extern int setNewWordsCallback(newWordsCallbackDelegate nwcb);
	[DllImport("upal")]
	private static extern int setNewAudioCallback(newAudioCallbackDelegate nacb);
	[DllImport("upal")]
	private static extern int startRecording(string sentences);
	[DllImport("upal")]
	private static extern int stopRecording();
	[DllImport("upal")]
	private static extern int setStrictness(int strictness);
	[DllImport("upal")]
	private static extern int startPlayback();
	[DllImport("upal")]
	private static extern int stopPlayback();
	[DllImport("upal")]
	private static extern int addWord(string word, string pronunciation);
#elif UNITY_STANDALONE_OSX
	public delegate void initCallbackDelegate(int n);
	public delegate void resultCallbackDelegate(int score, int speed, string words);
	public delegate void completionCallbackDelegate(int completion);
	public delegate void newWordsCallbackDelegate(string words);
	public delegate void newAudioCallbackDelegate(int volume, string pitch, string waveform);
	public delegate void playbackDoneCallbackDelegate();
	
	[DllImport("ispikit")]
	private static extern int startInitialization(initCallbackDelegate icb, string path);
	[DllImport("ispikit")]
	private static extern int setPlaybackDoneCallback(playbackDoneCallbackDelegate pcb);
	[DllImport("ispikit")]
	private static extern int setResultCallback(resultCallbackDelegate rcb);
	[DllImport("ispikit")]
	private static extern int setCompletionCallback(completionCallbackDelegate ccb);
	[DllImport("ispikit")]
	private static extern int setNewWordsCallback(newWordsCallbackDelegate nwcb);
	[DllImport("ispikit")]
	private static extern int setNewAudioCallback(newAudioCallbackDelegate nacb);
	[DllImport("ispikit")]
	private static extern int startRecording(string sentences);
	[DllImport("ispikit")]
	private static extern int stopRecording();
	[DllImport("ispikit")]
	private static extern int setStrictness(int strictness);
	[DllImport("ispikit")]
	private static extern int startPlayback();
	[DllImport("ispikit")]
	private static extern int stopPlayback();
	[DllImport("ispikit")]
	private static extern int addWord(string word, string pronunciation);
#endif

	private static System.Timers.Timer timer;
	// We need to keep this information
	// for desktop platforms otherwise the timer
	// keep running in the background
	private static bool stopped;
	void Awake()
	{
		Debug.Log ("About to initialize plugin");
		stopped = false;
#if UNITY_IOS
		string gameObjectName = "GameObject";
		string callbackName = "initCallback";
		// When calling the startInitialization function, we provide the name
		// of the Game Object and Callback for when init is done
		Debug.Log (startInitialization(gameObjectName, callbackName));
#elif UNITY_ANDROID || UNITY_STANDALONE_OSX
		Debug.Log (startInitialization(new initCallbackDelegate( this.initCallback ), Application.persistentDataPath));
#endif
	}
	void StopAll()
	{
		Debug.Log ("Stopping all");
		stopped = true;
		stopRecording ();
		stopPlayback ();
	}
	void OnApplicationQuit()
	{
		StopAll ();
	}
	void Start () {
	}
	void Update () {
	}

#if UNITY_IOS
	public void initCallback(string status) {
#elif UNITY_ANDROID || UNITY_STANDALONE_OSX
	public void initCallback(int status) {
#endif
		// This is for when plugin is initialized, status should be "0"
		// if successful
		Debug.Log ("Plugin initialization done");
		Debug.Log (status);
		// We now register all callbacks
#if UNITY_IOS
		setPlaybackDoneCallback ("GameObject", "playbackDoneCallback");
		setResultCallback ("GameObject", "resultCallback");
		setCompletionCallback ("GameObject", "completionCallback");
		setNewWordsCallback ("GameObject", "newWordsCallback");
		setNewAudioCallback ("GameObject", "newAudioCallback");
#elif UNITY_ANDROID || UNITY_STANDALONE_OSX
		Debug.Log ("About to wire callbacks");
		setResultCallback (new resultCallbackDelegate( this.resultCallback ));
		setCompletionCallback (new completionCallbackDelegate( this.completionCallback ));
		setNewWordsCallback (new newWordsCallbackDelegate( this.newWordsCallback ));
		setNewAudioCallback (new newAudioCallbackDelegate( this.newAudioCallback ));
		setPlaybackDoneCallback (new playbackDoneCallbackDelegate( this.playbackDoneCallback ));
#endif
		// We then start recording, with three possible inputs: "first", "second", and "third"
		startRecording ("first,second,third");
		Debug.Log ("Starting recording");
		// Three seconds later, we wiĺl stop recording in the provided onRecordingDone function
		if (stopped)
			return;
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}

	public static void playbackDoneCallback() {
		// This is called once playback is done. It will start another timer
		// after which recording will start again.
		Debug.Log ("Playback Done");
		if (stopped)
			return;
		timer = new System.Timers.Timer (1000);
		timer.Elapsed += onRestarting;
		timer.AutoReset = false;
		timer.Enabled = true;
	}

	private static void onRecordingDone(object source, ElapsedEventArgs e) {
		// This just stops recording. In the background, analysis will start
		// and result callback will be called once done.
		Debug.Log ("Stopping recording");
		stopRecording ();
	}
#if UNITY_IOS
	public void resultCallback(string status) {
#elif UNITY_ANDROID || UNITY_STANDALONE_OSX
	public static void resultCallback(int score, int speed, string words) {
#endif
		// Callback when result is available, a few seconds after stopRecording, typically.
		// See docs on how to parse the result.
		Debug.Log ("Result");
#if UNITY_IOS
		Debug.Log (status);
#elif UNITY_ANDROID || UNITY_STANDALONE_OSX
		Debug.Log (score);
		Debug.Log (speed);
		Debug.Log (words);
#endif
		// Starts replaying the userś voice after one second
		if (stopped)
			return;
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
	private static void onRestarting(object source, ElapsedEventArgs e) {
		// This is a function to be called by a timer to start another recognition
		// This boilerplate application will continue looping indefinitely through
		// recording - analysis - playback cycles
		Debug.Log ("Starting recording again");
		if (stopped)
			return;
		startRecording ("I am learning English,hello goodbye,one two three four");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
#if UNITY_IOS
	public void completionCallback(string status) {
#elif UNITY_ANDROID || UNITY_STANDALONE_OSX
	public static void completionCallback(int completion) {
#endif
		// This callback is called during analysis
		// Status is between "0" and "100", it is the percentage of completion of
		// analysis, it can be used to display a progress bar.
		Debug.Log ("Completion");
#if UNITY_IOS
		Debug.Log (status);
#elif UNITY_ANDROID || UNITY_STANDALONE_OSX
		Debug.Log (completion);
#endif
	}
	public static void newWordsCallback(string words) {
		// This callback comes during recording, it gives the words recognized
		// see docs on how to parse the string
		Debug.Log ("New words");
		Debug.Log (words);
	}
#if UNITY_IOS
	public void newAudioCallback(string status) {
#elif UNITY_ANDROID || UNITY_STANDALONE_OSX
	public static void newAudioCallback(int volume, string pitch, string waveform) {
#endif
		// This callback also comes during recording, it gives data about the recording
		// that can be used for UI effects: audio volume, pitch and waveform
		// see docs on how to parse it
		Debug.Log ("New audio data");
#if UNITY_IOS
		Debug.Log (status);
#elif UNITY_ANDROID || UNITY_STANDALONE_OSX
		Debug.Log (volume);
		Debug.Log (pitch);
		Debug.Log (waveform);
#endif
	}
}
