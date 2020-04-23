using System;

public class Class1
{
    /// <summary>
    /// из изображения в строку байтов
    /// </summary>
    /// <param name="imageIn"></param>
    /// <returns></returns>
    public byte[] ImageToByteArray(System.Drawing.Image imageIn)
    {
        using (var ms = new MemoryStream())
        {
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }
    }
    /// <summary>
    /// Из строки байтов в изображение
    /// </summary>
    /// <param name="byteArrayIn"></param>
    /// <returns></returns>
    public BitmapImage ByteArrayToImage(byte[] byteArrayIn)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.StreamSource = new MemoryStream(byteArrayIn);
        bitmap.EndInit();
        return bitmap;
    }
}
