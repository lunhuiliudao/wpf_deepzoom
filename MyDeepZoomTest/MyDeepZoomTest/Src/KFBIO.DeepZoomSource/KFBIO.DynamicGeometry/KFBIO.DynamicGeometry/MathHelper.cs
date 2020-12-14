using System;
using System.Collections.Generic;
using System.Windows;

namespace KFBIO.DynamicGeometry
{
	public class MathHelper
	{
		public static Point AngleArcPoint(Point middlePoint, Point p, double arcLength)
		{
			return AngleArcPoint(middlePoint.X, middlePoint.Y, p.X, p.Y, arcLength);
		}

		public static Point AngleArcPoint(double xCenter, double yCenter, double x, double y, double arcLength)
		{
			double num = Math.Atan2(y - yCenter, x - xCenter);
			double num2 = Math.Sin(num) * arcLength;
			double num3 = Math.Cos(num) * arcLength;
			return new Point(xCenter + num3, yCenter + num2);
		}

		public static double AngleDegree(Point p1, Point p2, Point p3)
		{
			return AngleDegree(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
		}

		public static double AngleDegree(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			double num = Math.Atan2(y1 - y2, x1 - x2);
			double num2 = (Math.Atan2(y3 - y2, x3 - x2) - num) / Math.PI * 180.0;
			if (num2 < 0.0)
			{
				return 360.0 + num2;
			}
			return num2;
		}

		public static double AngleRadian(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			double num = Math.Atan2(y1 - y2, x1 - x2);
			return Math.Atan2(y3 - y2, x3 - x2) - num;
		}

		public static double CalcArea(IList<Point> points)
		{
			int count = points.Count;
			if (count < 3)
			{
				return 0.0;
			}
			double num = 0.0;
			int num2 = 0;
			int index = num2 + 1;
			for (int i = num2 + 2; i < count; i++)
			{
				Point p = points[num2];
				Point p2 = points[index];
				Point p3 = points[i];
				num = ((!(RadianOfTwoLine(p, p2, p3) > 0.0)) ? (num - TriangleArea(p, p2, p3)) : (num + TriangleArea(p, p2, p3)));
				index = i;
			}
			return Math.Abs(num);
		}

		public static Point CalcCenterPoint(Point p1, Point p2, Point p3)
		{
			double x = CalcCenterX(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
			return new Point(x, CalcCenterY(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y));
		}

		public static double CalcCenterX(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			return ((x1 * x1 + y1 * y1) * (y2 - y3) - (x2 * x2 + y2 * y2) * (y2 - y3) - (x2 * x2 + y2 * y2) * (y1 - y2) + (x3 * x3 + y3 * y3) * (y1 - y2)) / (2.0 * ((x1 - x2) * (y2 - y3) - (x2 - x3) * (y1 - y2)));
		}

		public static double CalcCenterY(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			double num = CalcCenterX(x1, y1, x2, y2, x3, y3);
			return (x2 * x2 + y2 * y2 - (x3 * x3 + y3 * y3) - 2.0 * num * (x2 - x3)) / (2.0 * (y2 - y3));
		}

		public static double CalcLength(IList<Point> points)
		{
			if (points == null)
			{
				return 0.0;
			}
			int count = points.Count;
			if (count < 2)
			{
				return 0.0;
			}
			double num = 0.0;
			int index = 0;
			for (int i = 1; i < count; i++)
			{
				num += LineLength(points[index], points[i]);
				index = i;
			}
			return num;
		}

		public static double CalcLengthClosed(IList<Point> points)
		{
			if (points == null)
			{
				return 0.0;
			}
			int count = points.Count;
			if (count < 2)
			{
				return 0.0;
			}
			if (points[0] != points[count - 1])
			{
				List<Point> list = new List<Point>();
				list.AddRange(points);
				list.Add(points[0]);
				return CalcLength(list);
			}
			return CalcLength(points);
		}

		public static double CalcRadius(Point centerPoint, Point edgePoint)
		{
			return Math.Sqrt((edgePoint.X - centerPoint.X) * (edgePoint.X - centerPoint.X) + (edgePoint.Y - centerPoint.Y) * (edgePoint.Y - centerPoint.Y));
		}

		public static bool IsCounterClockwise(double radian)
		{
			double num = (radian < 0.0) ? (Math.PI * 2.0 + radian) : radian;
			if (num > 0.0)
			{
				return num < Math.PI;
			}
			return false;
		}

		public static bool IsLargeArc(double radian)
		{
			double num = (radian < 0.0) ? (Math.PI * 2.0 + radian) : radian;
			if (!(num < Math.PI / 2.0))
			{
				return num > 4.71238898038469;
			}
			return true;
		}

		public static double LineLength(Point p1, Point p2)
		{
			double num = p2.X - p1.X;
			double num2 = p2.Y - p1.Y;
			return Math.Sqrt(num * num + num2 * num2);
		}

		private static double RadianOfTwoLine(Point p1, Point p2, Point p3)
		{
			double num = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
			return Math.Atan2(p3.Y - p1.Y, p3.X - p1.X) - num;
		}

		public static double TriangleArea(Point p1, Point p2, Point p3)
		{
			double num = LineLength(p1, p2);
			double num2 = LineLength(p1, p3);
			double num3 = LineLength(p2, p3);
			if (num + num2 <= num3 || num2 + num3 <= num || num3 + num <= num2)
			{
				return 0.0;
			}
			double num4 = (num + num2 + num3) / 2.0;
			return Math.Sqrt(Math.Abs(num4 * (num4 - num) * (num4 - num2) * (num4 - num3)));
		}
	}
}
