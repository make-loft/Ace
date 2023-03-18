using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ace.Extensions
{
	public static class Presentation
	{
		public static double GetDpiScaleX(this Visual visual) => PresentationSource.FromVisual(visual).GetDpiScaleX();
		public static double GetDpiScaleY(this Visual visual) => PresentationSource.FromVisual(visual).GetDpiScaleY();

		public static double GetDpiScaleX(this PresentationSource source) => source.CompositionTarget.TransformToDevice.M11;
		public static double GetDpiScaleY(this PresentationSource source) => source.CompositionTarget.TransformToDevice.M22;

		public static Color GetPixelColor(this FrameworkElement element, Point position)
		{
			var bytes = element.GetPixelBytes(position);
			var color = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);

			return color;
		}

		public static byte[] GetPixelBytes(this FrameworkElement element, Point position)
		{
			var visual = new DrawingVisual();
			var rect = new Rect(element.RenderSize);

			using (var dc = visual.RenderOpen())
				dc.DrawRectangle(new VisualBrush(element), null, rect);

			var bitmap = new RenderTargetBitmap((int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Pbgra32);
			bitmap.Render(visual);

			return bitmap.GetPixelBytes((int)position.X, (int)position.Y);
		}

		public static byte[] GetPixelBytes(this BitmapSource bitmap, int x, int y)
		{
			var bytesPerPixel = bitmap.Format.BitsPerPixel / 8;
			var offset = (y * bitmap.PixelWidth + x) * bytesPerPixel;
			var stride = bitmap.PixelWidth * bytesPerPixel;
			var length = bitmap.PixelHeight * stride;

			if (length <= offset)
				return BitConverter.GetBytes(0x00888888);

			var pixels = new byte[length];
			bitmap.CopyPixels(pixels, stride, 0);

			var bytes = new byte[bytesPerPixel];
			Array.Copy(pixels, offset, bytes, 0, bytes.Length);
			return bytes;
		}
	}
}
