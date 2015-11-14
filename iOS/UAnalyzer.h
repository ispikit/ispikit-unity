#import <Foundation/Foundation.h>

extern "C" {
  int startInitialization(const char*, const char*);
  int setPlaybackDoneCallback(const char*, const char*);
  int setResultCallback(const char*, const char*);
  int setCompletionCallback(const char*, const char*);
  int setNewWordsCallback(const char*, const char*);
  int setNewAudioCallback(const char*, const char*);
  int startRecording(const char*);
  int stopRecording();
  int setStrictness(int strictness);
  int startPlayback();
  int stopPlayback();
  int addWord(char* word, char* pronunciation);
}
