#if PRO
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;
using Google.YouTube;
using Google.GData.YouTube;
using Google.GData.Client;
using Google.GData.Extensions;

namespace VidLapse {
    public partial class VidLapse : MonoBehaviour {
		/**
       * The name of the finished video
     	* @note Pro and Developer Version only
       * @private
       */
		private string _videoname="";

		/**
       * The application's youtube developer key
       * @note Pro and Developer Version only
       * @private
       */
		private const string _youtubekey = "";

       /**
       * VidLapse Constructor
       * @param freq The frequency to collect screenshots
	   * @param folder The folder to store images in the applications directory
	   * @note Pro and Developer Version only
       */
		public VidLapse(float freq, string folder){
			_frequency=freq;
			_directory=folder;
			_fullpath=Application.persistentDataPath+"/"+_directory;
		}

		/**
       * VidLapse Constructor
       * @param folder The folder to store images in the applications directory
       * @note Pro and Developer Version only
       */
		public VidLapse(string folder){
			_frequency=1f;
			_directory=folder;
			_fullpath=Application.persistentDataPath+"/"+_directory;
		}

		/**
       * VidLapse Constructor
       * @param freq The frequency to collect screenshots
       * @param fullpath The fullpath to output the captured screenshots
       * @note Pro and Developer Version only
       */
		public VidLapse(int freq, string fullpath){
			_frequency=(float)freq;
			_fullpath=fullpath;
			_fullpathset=true;
		}

		/**
       * VidLapse Constructor
       * @param fullpath The fullpath to output the finished video
       * @param blank Not used
       * @note Pro and Developer Version only
       */
		public VidLapse(string fullpath,bool blank){
			_frequency=1f;
			_fullpath=fullpath;
			_fullpathset=true;
		}

        /**
       * Upload the finished movie to YouTube
       * @param user The username to use
       * @param pass The password to use
       * @return void
       * @note Pro and Developer Version only
       */
		public void Upload(string user, string pass){
			YouTubeRequestSettings settings = new YouTubeRequestSettings("VidLapse",_youtubekey,user,pass);
			YouTubeRequest request = new YouTubeRequest(settings);

			Video vid = new Video();
			vid.Title = _videoname;
			vid.Description = "";
			vid.YouTubeEntry.Private=false;

			if(SaveToDesktop)
				vid.YouTubeEntry.MediaSource = new MediaFileSource(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"/"+_Filename+".avi","video/avi");
			else
				vid.YouTubeEntry.MediaSource = new MediaFileSource(_fullpath+"/"+_Filename+".avi","video/avi");
			Video complete = request.Upload(vid);
		}

		/**
       * Upload the finished movie to YouTube
       * @param user The username to use
       * @param pass The password to use
       * @param videotitle The video title to use
       * @return void
       * @note Pro and Developer Version only
       */
		public void Upload(string user, string pass,string videotitle){
			YouTubeRequestSettings settings = new YouTubeRequestSettings("VidLapse",_youtubekey,user,pass);
			YouTubeRequest request = new YouTubeRequest(settings);
			
			Video vid = new Video();
			vid.Title = videotitle;
			vid.Description = "";
			vid.YouTubeEntry.Private=false;
			if(SaveToDesktop)
				vid.YouTubeEntry.MediaSource = new MediaFileSource(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"/"+_Filename+".avi","video/avi");
			else
				vid.YouTubeEntry.MediaSource = new MediaFileSource(_fullpath+"/"+_Filename+".avi","video/avi");
			Video complete = request.Upload(vid);
		}

		/**
       * Upload the finished movie to YouTube
       * @param user The username to use
       * @param pass The password to use
       * @param videotitle The video title to use
       * @param description The description to use
       * @return void
       * @note Pro and Developer Version only
       */
		public void Upload(string user, string pass,string videotitle,string description){
			YouTubeRequestSettings settings = new YouTubeRequestSettings("VidLapse",_youtubekey,user,pass);
			YouTubeRequest request = new YouTubeRequest(settings);
			
			Video vid = new Video();
			vid.Title = videotitle;
			vid.Description = description;
			vid.YouTubeEntry.Private=false;
			if(SaveToDesktop)
				vid.YouTubeEntry.MediaSource = new MediaFileSource(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"/"+_Filename+".avi","video/avi");
			else
				vid.YouTubeEntry.MediaSource = new MediaFileSource(_fullpath+"/"+_Filename+".avi","video/avi");
			Video complete = request.Upload(vid);
		}

       /**
       * Create a movie from the captured shots
       * @param width The width of the final movie
       * @param height The height of the final movie
       * @param framerate The framerate of the final movie
       * @return void
	   * @note Pro and Developer Version only
       */
		public void CreateMovie(int width, int height, int framerate){
            if(_Filename==""){
				_Filename="VidLapse-Video"+DateTime.Now.Millisecond+DateTime.Now.Second;
			}
			Bitmap bmp;
			AviWriter aviw = new AviWriter();
			if(SaveToDesktop){
				bmp = aviw.Open(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"/"+_Filename+".avi",(uint)framerate,width,height);
			}else{
				bmp = aviw.Open(_fullpath+"/"+_Filename+".avi",(uint)framerate,width,height);
			}

			System.Drawing.Graphics canvas = System.Drawing.Graphics.FromImage(bmp);

            if (_saveMethod == SaveMethod.Disk) {
                DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath + "/");
                FileInfo[] files = d.GetFiles("*.png");

                foreach (FileInfo file in files) {
                    Bitmap bmp2 = new Bitmap(file.FullName);
                    bmp2.RotateFlip(RotateFlipType.Rotate180FlipX);
                    bmp2 = ReduceBitmap(bmp2, width, height);

                    canvas.Clear(System.Drawing.Color.White);
                    canvas.DrawImage(bmp2, 0, 0);
                    

                    aviw.AddFrame();
                }
            } else {
                for (int i = 0; i < _imageBytes.Count; i++) {
                    byte[] imgbytes = _imageBytes.Dequeue();
                    Bitmap bmp2 = ToBitmap(imgbytes);
                    bmp2.RotateFlip(RotateFlipType.Rotate180FlipX);
                    bmp2 = ReduceBitmap(bmp2, width, height);

                    canvas.Clear(System.Drawing.Color.White);
                    canvas.DrawImage(bmp2, 0, 0);
                    aviw.AddFrame();
                }
            }
			
			aviw.Close();
			_stored=0;
			_complete=0f;
			RemoveImages();
		}

		/**
       * Create a movie from the captured shots
       * @return void
	   * @note Pro and Developer Version only
       */
		public void CreateMovie(){
			int width=320;
			int height=240;
			int framerate=200;

			if(_Filename==""){
				_Filename="VidLapse-Video"+DateTime.Now.Millisecond+DateTime.Now.Second;
			}

			AviWriter aviw = new AviWriter();
			Bitmap bmp = aviw.Open(_fullpath+"/"+_Filename+".avi",(uint)framerate,width,height);
			
			System.Drawing.Graphics canvas = System.Drawing.Graphics.FromImage(bmp);

            if (_saveMethod == SaveMethod.Disk) {
                DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath + "/");
                FileInfo[] files = d.GetFiles("*.png");

                foreach (FileInfo file in files) {
                    Bitmap bmp2 = new Bitmap(file.FullName);
                    bmp2.RotateFlip(RotateFlipType.Rotate180FlipX);
                    bmp2 = ReduceBitmap(bmp2, width, height);

                    canvas.Clear(System.Drawing.Color.White);
                    canvas.DrawImage(bmp2, 0, 0);

                    aviw.AddFrame();
                }
            } else {
                for (int i = 0; i < _imageBytes.Count; i++) {
                    byte[] imgbytes = _imageBytes.Dequeue();
                    Bitmap bmp2 = ToBitmap(imgbytes);
                    bmp2.RotateFlip(RotateFlipType.Rotate180FlipX);
                    bmp2 = ReduceBitmap(bmp2, width, height);

                    canvas.Clear(System.Drawing.Color.White);
                    canvas.DrawImage(bmp2, 0, 0);
                    aviw.AddFrame();
                }
            }
			
			aviw.Close();
			_stored=0;
			_complete=0f;
			RemoveImages();
		}

        #region Getters/Setters
        /**
       * Gets/sets the frequency of image collection
       * @return float The frequency the system should collect screenshots
  	   * @note Pro and Developer Version only
       */
		public float Frequency{
			get{return _frequency;}
			set{_frequency=value;}
		}

		/**
       * Gets/sets whether the system should save to desktop
       * @return bool Whether or not the system should save to desktop
  	   * @note Pro and Developer Version only
       */
		public bool SaveToDesktop{
			get{return _savetodesktop;}
			set{_savetodesktop=value;}
		}
		
		/**
       * Gets the total count of images stored
       * @return int Count of images stored
  	   * @note Pro and Developer Version only
       */
		public int Stored{
			get{return _stored;}
			private set{_stored=value;}
		}

		/**
       * Gets/sets the finished movie's name
       * @return string Finished movie's name
  	   * @note Pro and Developer Version only
       */
		public string MovieName{
			get{return _Filename;}
			set{_Filename=value;}
		}

		/**
       * Gets/sets the Directory to store images
       * @return string Directory to store images
  	   * @note Pro and Developer Version only
       */
		public string Folder{
			get{
				if(_fullpathset)
					return "";
				else
					return _directory;
			}
			set{
				if(_fullpathset)
					throw new Exception("Cannot set new directory.  Full path has already been set!");
				_directory=value;}
        }
        #endregion
    }
}
#endif