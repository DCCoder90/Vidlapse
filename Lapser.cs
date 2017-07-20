using System;
using UnityEngine;
namespace VidLapse{
	public class Lapser : MonoBehaviour{
		public KeyCode StartRecordingKey = KeyCode.A;
		public KeyCode StopRecordingKey = KeyCode.S;
		public KeyCode CancelRecordingKey = KeyCode.D;
		public bool AdjustCaptureTime=false;
		public int CaptureTime=5;
        public SaveMethod Savemethod = SaveMethod.Disk;
        public CaptureMethod Capturemethod = CaptureMethod.Pixels;
	

        #if PRO
		public bool SaveToDesktop=true;

		public int VideoW = 160;
		public int VideoH = 120;
		public int FrameRate = 10;
		public bool UseDefaultName=false;
		public string VideoName = "New Video";

		public bool UploadWhenComplete = false;
		public string Username = "";
		public string Password = "";
        #endif

		private VidLapse _lapser;
		private int _cachedcapturetime;


		public void Start(){
			_lapser=this.gameObject.AddComponent<VidLapse>();
            if (AdjustCaptureTime) {
                _cachedcapturetime = Time.captureFramerate;
            }

            #if PRO
			_lapser.SaveToDesktop=SaveToDesktop;
            #endif
		}

		public void Update(){
			if(Input.GetKeyDown(StartRecordingKey)){
                StartRecording();
			}

			if(Input.GetKeyDown(StopRecordingKey)){
                StopRecording();
			}

			if(Input.GetKeyDown(CancelRecordingKey)){
                CancelRecording();
			}
		}

        private void StartRecording(){
            if (!_lapser.Record) {
                if (AdjustCaptureTime) {
                    Time.captureFramerate = CaptureTime;
                }
                _lapser.CaptureImages(Capturemethod, Savemethod);
            }
        }

        private void StopRecording(){
            if (_lapser.Record) {
                _lapser.Record = false;

                if (AdjustCaptureTime) {
                    Time.captureFramerate = _cachedcapturetime;
                }
#if PRO
                if (!UseDefaultName) {
                    _lapser.MovieName = VideoName;
                }

                if (VideoH == 0 && VideoW == 0 && FrameRate == 0 || FrameRate == 0) {
                    _lapser.CreateMovie();
                }else{
                    _lapser.CreateMovie(VideoW, VideoH, FrameRate);
                }

                if (UploadWhenComplete) {
                    _lapser.Upload(Username, Password);
                }
#else
                    _lapser.CreateMovie();
#endif
            }
        }

        private void CancelRecording(){
            if (_lapser.Record) {
                _lapser.Record = false;

                if (AdjustCaptureTime) {
                    Time.captureFramerate = _cachedcapturetime;
                }
                _lapser.RemoveImages();
            }
        }
	}
}