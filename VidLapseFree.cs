#if !PRO
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;

namespace VidLapse {
    public partial class VidLapse : MonoBehaviour {
        /**
       * Create a movie from the captured shots
       * @return void
       */
        public void CreateMovie() {
            _Filename = "VidLapse-Video" + DateTime.Now.Millisecond + DateTime.Now.Second;
            int width = 160;
            int height = 120;
            int framerate = 10;

            //Debug.Log("Creating Movie");
            AviWriter aviw = new AviWriter();
            Bitmap bmp = aviw.Open(_fullpath + "/" + _Filename + ".avi", (uint)framerate, width, height);

            System.Drawing.Graphics canvas = System.Drawing.Graphics.FromImage(bmp);

            System.Drawing.Font font = new System.Drawing.Font("Arial", 16);
            SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);

            if (_saveMethod == SaveMethod.Disk) {
                DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath + "/");
                FileInfo[] files = d.GetFiles("*.png");

                foreach (FileInfo file in files) {
                    //byte[] imagebytes = File.ReadAllBytes(file.FullName);
                    Bitmap bmp2 = new Bitmap(file.FullName);
                    bmp2.RotateFlip(RotateFlipType.Rotate180FlipX);
                    bmp2 = ReduceBitmap(bmp2, width, height);

                    canvas.Clear(System.Drawing.Color.White);
                    canvas.DrawImage(bmp2, 0, 0);
                    canvas.DrawString("VidLapse Free", font, brush,new PointF(0,0,));                    

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
            _stored = 0;
            _complete = 0f;
            RemoveImages();
        }
    }
}
#endif