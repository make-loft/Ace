using System.Windows.Media.Media3D;

namespace Ace.Extensions
{
	public static class ProjectionCameraExtensions
	{
		public static TCamera Move<TCamera>(this TCamera camera, Vector3D axis, double step)
			where TCamera : ProjectionCamera
		{
			camera.Position += axis * step;
			return camera;
		}

		public static TCamera Rotate<TCamera>(this TCamera camera, Vector3D axis, double angle)
			where TCamera : ProjectionCamera
		{
			Matrix3D matrix3D = new();
			matrix3D.RotateAt(new(axis, angle), camera.Position);
			camera.LookDirection *= matrix3D;
			return camera;
		}

		public static Vector3D GetYawAxis(this ProjectionCamera camera) => camera.UpDirection;
		public static Vector3D GetRollAxis(this ProjectionCamera camera) => camera.LookDirection;
		public static Vector3D GetPitchAxis(this ProjectionCamera camera) => Vector3D.CrossProduct(camera.UpDirection, camera.LookDirection);
	}
}
