using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;




namespace VidLapse{
	/*! 
	 *  \brief     Manages recording and writing process of AVI
	 *  \details   Manages recording and writing process of AVI
	 *  \author    Ernest Mallett
	 *  \date      March 2014
	 *  \copyright DarkCloud Games 2014
	 */
	public partial class VidLapse : MonoBehaviour {

        #region Variables
        /**
       * How many seconds to wait between each shot
       * @private
       */
		private float _frequency;
		/**
       * Directory to store shots
       * @private
       */
		private string _directory;
		/**
       * Full path and directory
       * @private
       */
		private string _fullpath;
		/**
       * Percentage complete of making movie
       * @private
       */
		private float _complete=0f;
		/**
       * Number of images stored
       * @private
       */
		private int _stored=0;
		/**
       * Can we record?
       * @private
       */
		private bool _record = true;
		/**
       * Save the video to desktop?
       * @private
       */
		private bool _savetodesktop = false; 
		/**
       * Filename to save the completed video as
       * @private
       */
		private string _Filename="";
		/**
       * Is the full path of the video set?
       * @private
       */
		private bool _fullpathset=false;

        /**
        * What method are we going to use to capture?
        * @private
        */
        private CaptureMethod _captureMethod = CaptureMethod.Pixels;

        /**
        * Queue for storing images in memory
        * @private
        */
        private Queue<byte[]> _imageBytes = new Queue<byte[]>();

        /**
        * What method are we going to use to capture?
        * @private
        */
        private SaveMethod _saveMethod = SaveMethod.Disk;

        #endregion
        
        /**
       * VidLapse Constructor
       */
		public VidLapse(){
			_frequency=1f;
			_directory="VidLapse";
			_fullpath=Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
		}
		
		/**
       * Delete all .png files in the specified directory
       * @return void
       */
		public void RemoveImages(){
            if (_saveMethod == SaveMethod.Disk) {
                DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath);
                FileInfo[] files = d.GetFiles("*.png");
                foreach (FileInfo file in files) {
                    file.Delete();
                }
            } else {
                _imageBytes = new Queue<byte[]>();
            }
		}

        #region Capture Methods

        public void CaptureImages(CaptureMethod capturemethod, SaveMethod savemethod) {
            _saveMethod = savemethod;

            switch (capturemethod) {
                case CaptureMethod.Pixels:
                    StartCoroutine(ReadPixels());
                break;

                case CaptureMethod.Tex:
                    StartCoroutine(RenderToTex());
                break;

                case CaptureMethod.App:
                    StartCoroutine(ScreenShot());
                break;
            }
        }

        /**
       * Take a screenshot using Unity's built-in method
       * @return IEnumerator
       */
		private IEnumerator ScreenShot(){
			_record=true;
			while(true){
                yield return new WaitForEndOfFrame();
                if (!_record) {
                    break;
                }
				Application.CaptureScreenshot(Application.persistentDataPath+"/"+DateTime.Now.Day+DateTime.Now.Hour+DateTime.Now.Minute+DateTime.Now.Second+DateTime.Now.Millisecond+".png");
				_stored++;
				yield return new WaitForSeconds(_frequency);
                if (!_record) {
                    break;
                }
			}
		}

        /**
        * Take a screenshot by reading screen pixels
        * @return IEnumerator
        */
        private IEnumerator ReadPixels() {
            _record = true;

            while (true) {
                yield return new WaitForEndOfFrame();
                if (!_record) {
                    break;
                }

                Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                yield return 0;

                byte[] bytes = texture.EncodeToPNG();

                if (_saveMethod == SaveMethod.Disk) {
                    File.WriteAllBytes(Application.persistentDataPath + "/" + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".png", bytes);
                    DestroyObject(texture);
                } else {
                    _imageBytes.Enqueue(bytes);
                }

                yield return new WaitForSeconds(_frequency);
                if (!_record) {
                    break;
                }
            }
        }

        /**
        * Take a screenshot by rendering camera to Texture
        * @return IEnumerator
        */
        private IEnumerator RenderToTex() {
            _record = true;

            while (true) {
                yield return new WaitForEndOfFrame();
                if (!_record) {
                    break;
                }

                RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
                Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

                foreach (Camera cam in Camera.allCameras) {
                    cam.targetTexture = rt;
                    cam.Render();
                    cam.targetTexture = null;
                }

                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                Camera.main.targetTexture = null;
                RenderTexture.active = null;
                Destroy(rt);

                yield return 0;

                byte[] bytes = screenShot.EncodeToPNG();

                if (_saveMethod == SaveMethod.Disk) {
                    File.WriteAllBytes(Application.persistentDataPath + "/" + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".png", bytes);
                } else {
                    _imageBytes.Enqueue(bytes);
                }

                yield return new WaitForSeconds(_frequency);
                if (!_record) {
                    break;
                }
            }
        }
        #endregion

        /**
       * Convert the Render size to a System rectangle
       * @param bounds The Bounds to create a rectangle from
       * @return Rectangle A System Rectangle created from the screen bounds
       * @private
       */
		private System.Drawing.Rectangle BoundsToScreenRect(Bounds bounds)
		{
			// Get mesh origin and farthest extent (this works best with simple convex meshes)
			Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
			Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));
			
			// Create rect in screen space and return - does not account for camera perspective
			return new System.Drawing.Rectangle((int)origin.x,(int)Screen.height - (int)origin.y,(int)extent.x - (int)origin.x,(int)origin.y - (int)extent.y);
		}

		#region Setup Bitmap
		/**
       * Creates a bitmap from a byte array
       * @param arrayin A byte array to use
       * @return Bitmap The created Bitmap
       * @private
       */
		private Bitmap ToBitmap(byte[] arrayin){
			MemoryStream ms = new MemoryStream(arrayin);
			Image returnimage = Image.FromStream(ms);
			Bitmap bitmap = new Bitmap(returnimage);
			return bitmap;
		}

		/**
       * Resize the supplied bitmap
       * @param original The original bitmap
       * @param reducedWidth The width of the new bitmap
       * @param reducedHeight The height of the new bitmap
       * @return Bitmap The reduced bitmap
       * @private
       */
		private Bitmap ReduceBitmap(Bitmap original, int reducedWidth, int reducedHeight)
		{
			var reduced = new Bitmap(reducedWidth, reducedHeight);
			using (var dc = System.Drawing.Graphics.FromImage(reduced))
			{
				dc.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				dc.DrawImage(original, new Rectangle(0, 0, reducedWidth, reducedHeight), new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
			}
			
			return reduced;
		}
		#endregion

		#region Getters/Setters
		/**
       * Gets the video creation progress
       * @return float Progress of video creation
       * @note Currently not functional will fix in later update
       */
		public float VideoProgress{
			get{return _complete;}
			private set{_complete=value;}
		}

		/**
       * Gets/Sets the current state of recroding
       * @return bool Whether or not the system is recording
       */
		public bool Record{
			get{return _record;}
			set{_record=value;}
		}
		#endregion
	}
}