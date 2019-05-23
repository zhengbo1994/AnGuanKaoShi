using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace IDCardInformationReader
{
    public class ImageHelper
    {

        public static string formatPhoto(string fileName, string filePath,string idNumber)
        {
            int widthRatio = 8;
            int heightRatio = 11;


            Image pic = Image.FromFile(fileName);//strFilePath是该图片的绝对路径
            int intWidth = pic.Width;//长度像素值
            int intHeight = pic.Height;//高度像素值 

            int width = 0;
            int height = 0;
            int offsetX = 0;
            int offsetY = 0;

            if (intWidth * heightRatio / widthRatio > intHeight)
            {
                width = intHeight * widthRatio / heightRatio;
                height = intHeight;
            }
            else
            {
                width = intWidth;
                height = intWidth * heightRatio / widthRatio;
            }
            offsetX = (intWidth - width) / 2;
            offsetY = (intHeight - height) / 2;

            Bitmap imageResult = GetPartOfImage(fileName, width, height, offsetX, offsetY);

            string formatPhotoName = filePath + "\\" + "employeeResult" + idNumber + ".jpg"; ;

            imageResult.Save(formatPhotoName, System.Drawing.Imaging.ImageFormat.Jpeg);

            return formatPhotoName;


        }
        private static Bitmap GetPartOfImage(string bitmapPathAndName, int width, int height, int offsetX, int offsetY)
        {
            FileStream fs = new FileStream(bitmapPathAndName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            MemoryStream ms = new MemoryStream(bytes);

            Bitmap sourceBitmap = new Bitmap(ms);
            




            //Bitmap sourceBitmap = new Bitmap(bitmapPathAndName);
            Bitmap resultBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resultBitmap))
            {
                Rectangle resultRectangle = new Rectangle(0, 0, width, height);
                Rectangle sourceRectangle = new Rectangle(0 + offsetX, 0 + offsetY, width, height);
                g.DrawImage(sourceBitmap, resultRectangle, sourceRectangle, GraphicsUnit.Pixel);
                sourceBitmap.Dispose();
            }
            return resultBitmap;
        }

    }
}
