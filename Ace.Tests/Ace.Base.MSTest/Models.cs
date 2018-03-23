namespace Ace.Base.MSTest
{
	public class Point
	{
		public int? X { get; set; }
		public int? Y { get; set; }
	}
	
	public interface IModel {}
	public class ModelA : IModel {}
	public class ModelB : IModel {}
	public class ModelC : IModel {}
	
	public class Shape {}
	public class Line : Shape { public double Length; }
	public class Circle : Shape { public double Radius; }
	public class Triangle : Shape {}
	public class Rectangle : Shape { public double Width, Height; }
}