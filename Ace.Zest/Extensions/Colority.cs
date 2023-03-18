using System;
using System.Collections.Generic;
using System.Linq;

#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Media;
#endif

namespace Ace.Extensions
{
	public static class Colority
	{
		private static byte Byte(in double value) => (byte)(255d * value);
		private static byte Byte(in float value) => (byte)(255f * value);

#if XAMARIN
		public static Color FromARGB(double a, double r, double g, double b) => Color.FromRgba(r, g, b, a);
		public static Color FromARGB(float a, float r, float g, float b) => Color.FromRgba(r, g, b, a);
		public static Color FromARGB(byte a, byte r, byte g, byte b) => Color.FromRgba(r, g, b, a);
		public static Color FromARGB(int a, int r, int g, int b) => Color.FromRgba(r, g, b, a);

		public static Color FromRGBA(double r, double g, double b, double a = 1d) => Color.FromRgba(r, g, b, a);
		public static Color FromRGBA(float r, float g, float b, float a = 1f) => Color.FromRgba(r, g, b, a);
		public static Color FromRGBA(byte r, byte g, byte b, byte a = 0xFF) => Color.FromRgba(r, g, b, a);
		public static Color FromRGBA(int r, int g, int b, int a = 255) => Color.FromRgba(r, g, b, a);

		public static Color FromHEX(string value) => Color.FromHex(value);

		public static Color Mix(this Color color, Channel channel, byte value) => channel switch
		{
			Channel.A => FromARGB(value, Byte(color.R), Byte(color.G), Byte(color.B)),
			Channel.R => FromARGB(Byte(color.A), value, Byte(color.G), Byte(color.B)),
			Channel.G => FromARGB(Byte(color.A), Byte(color.R), value, Byte(color.B)),
			Channel.B => FromARGB(Byte(color.A), Byte(color.R), Byte(color.G), value),
			_ => FromARGB(Byte(color.A), Byte(color.R), Byte(color.G), Byte(color.B)),
		};
#else
		public static Color FromARGB(double a, double r, double g, double b) => Color.FromArgb(Byte(a), Byte(r), Byte(g), Byte(b));
		public static Color FromARGB(float a, float r, float g, float b) => Color.FromArgb(Byte(a), Byte(r), Byte(g), Byte(b));
		public static Color FromARGB(byte a, byte r, byte g, byte b) => Color.FromArgb(a, r, g, b);
		public static Color FromARGB(int a, int r, int g, int b) => Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);

		public static Color FromRGBA(double r, double g, double b, double a = 1d) => Color.FromArgb(Byte(a), Byte(r), Byte(g), Byte(b));
		public static Color FromRGBA(float r, float g, float b, float a = 1f) => Color.FromArgb(Byte(a), Byte(r), Byte(g), Byte(b));
		public static Color FromRGBA(byte r, byte g, byte b, byte a = 0xFF) => Color.FromArgb(a, r, g, b);
		public static Color FromRGBA(int r, int g, int b, int a = 255) => Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);

		public static Color FromHEX(string value) => (Color)ColorConverter.ConvertFromString(value);

		public static Color Mix(this Color color, Channel channel, byte value) => channel switch
		{
			Channel.A => FromARGB(value, color.R, color.G, color.B),
			Channel.R => FromARGB(color.A, value, color.G, color.B),
			Channel.G => FromARGB(color.A, color.R, value, color.B),
			Channel.B => FromARGB(color.A, color.R, color.G, value),
			_ => FromARGB(color.A, color.R, color.G, color.B),
		};
#endif

		public enum Channel { A, R, G, B }

		public static Color Mix(this Color color, Channel channel, double value) => color.Mix(channel, Byte(value));
		public static Color Mix(this Color color, Channel channel, float value) => color.Mix(channel, Byte(value));
		public static Color Mix(this Color color, Channel channel, int value) => color.Mix(channel, (byte)value);


		public struct GradientPoint
		{
			public Color Color { get; set; }
			public double Offset { get; set; }
		}

		public static Color GetColor(IEnumerable<GradientPoint> gradientPoints, double offset)
		{
			GradientPoint fromPoint = default;
			GradientPoint tillPoint = default;

			foreach (var point in gradientPoints.OrderBy(s => s.Offset))
			{
				if (offset < point.Offset)
				{
					tillPoint = point;
					if (fromPoint.Is(default))
						fromPoint = tillPoint;
					break;
				}
				else
				{
					tillPoint = fromPoint = point;
				}
			}

			var offsetLength = tillPoint.Offset - fromPoint.Offset;
			var offsetValue = offset - fromPoint.Offset;

			double InterpolateValue(double fromValue, double tillValue) =>
				fromValue + (tillValue - fromValue) * offsetValue / offsetLength;

			double InterpolateChannel(Func<Color, double> getChannelValue, GradientPoint fromPoint, GradientPoint tillPoint) =>
				InterpolateValue(getChannelValue(fromPoint.Color), getChannelValue(tillPoint.Color));

			var r = InterpolateChannel(c => c.R, fromPoint, tillPoint);
			var g = InterpolateChannel(c => c.G, fromPoint, tillPoint);
			var b = InterpolateChannel(c => c.B, fromPoint, tillPoint);
			var a = InterpolateChannel(c => c.A, fromPoint, tillPoint);

#if XAMARIN
			return FromRGBA(r, g, b, a);
#else
			return FromRGBA(r / 255, g / 255, b / 255, a / 255);
#endif
		}

		public static Color FromHSVA(double h, double s, double v, double a = 1d)
		{
			var num0 = h * 6.0;
			var num1 = Math.Floor(num0);
			var num2 = num0 - num1;

			var num3 = v * (1.0 - s);
			var num4 = v * (1.0 - num2 * s);
			var num5 = v * (1.0 - (1.0 - num2) * s);

			var sector = (int)num1 % 6;

			return sector switch
			{
				0 => FromRGBA(v, num5, num3, a),
				1 => FromRGBA(num4, v, num3, a),
				2 => FromRGBA(num3, v, num5, a),
				3 => FromRGBA(num3, num4, v, a),
				4 => FromRGBA(num5, num3, v, a),
				5 => FromRGBA(v, num3, num4, a),
				_ => throw new NotImplementedException(),
			};
		}
	}
}
