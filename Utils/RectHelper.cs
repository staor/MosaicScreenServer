using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    class RectHelper
    {
    }

    public struct Rect
    {
        //
        // 摘要:
        //     获取一个值，该值指示矩形是否为 System.Windows.Rect.Empty 矩形。
        //
        // 返回结果:
        //     如果矩形为 System.Windows.Rect.Empty 矩形，则为 true；否则为 false。
        public bool IsEmpty { get => this == _empty; }
        private static Rect _empty = new Rect(0, 0, 0, 0);
        public static Rect Empty { get => _empty; }
        //
        // 摘要:
        //     获取或设置矩形的宽度。
        //
        // 返回结果:
        //     一个正数，表示矩形的宽度。默认值为 0。
        //
        // 异常:
        //   T:System.ArgumentException:
        //     System.Windows.Rect.Width 设置为一个负值。
        //
        //   T:System.InvalidOperationException:
        //     在 System.Windows.Rect.Empty 矩形上设置 System.Windows.Rect.Width。
        public int Width;
        //
        // 摘要:
        //     获取或设置矩形的高度。
        //
        // 返回结果:
        //     一个正数，表示矩形的高度。默认值为 0。
        //
        // 异常:
        //   T:System.ArgumentException:
        //     System.Windows.Rect.Height 设置为一个负值。
        //
        //   T:System.InvalidOperationException:
        //     在 System.Windows.Rect.Empty 矩形上设置 System.Windows.Rect.Height。
        public int Height;

        //
        // 摘要:
        //     获取矩形左边的 x 轴值。
        //
        // 返回结果:
        //     矩形左边的 x 轴值。
        public int Left { get=>X; }
        //
        // 摘要:
        //     获取矩形顶边的 y 轴位置。
        //
        // 返回结果:
        //     矩形顶边的 y 轴位置。
        public int Top { get => Y; }
        //
        // 摘要:
        //     获取或设置矩形左边的 x 轴值。
        //
        // 返回结果:
        //     矩形左边的 x 轴值。
        //
        // 异常:
        //   T:System.InvalidOperationException:
        //     在 System.Windows.Rect.Empty 矩形上设置 System.Windows.Rect.X。
        public int X;
        //
        // 摘要:
        //     获取矩形右边的 x 轴值。
        //
        // 返回结果:
        //     矩形右边的 x 轴值。
        public int Right { get=>X+Width; }
        //
        // 摘要:
        //     获取或设置矩形顶边的 y 轴值。
        //
        // 返回结果:
        //     矩形顶边的 y 轴值。
        //
        // 异常:
        //   T:System.InvalidOperationException:
        //     在 System.Windows.Rect.Empty 矩形上设置 System.Windows.Rect.Y。
        public int Y;

        //
        // 摘要:
        //     获取矩形底边的 y 轴值。
        //
        // 返回结果:
        //     矩形底边的 y 轴值。如果矩形为空，则该值为 System.Double.NegativeInfinity。
        public int Bottom { get=>Y+Height; }
        
        //public static Rect Empty { get; }
        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        /// <summary>
        /// r1与r2矩形相交
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>交集矩形，否则为IsEmpty</returns>
        public static Rect Intersect(Rect r1, Rect r2)
        {
            Rect rect = new Rect();
            if (r1.Width <= 0 || r1.Height <= 0 || r2.Width <= 0 || r2.Height <= 0 || r1.X >= r2.Right || r1.Right <= r2.X || r1.Y >= r2.Bottom || r1.Bottom <= r2.Y) 
            {
                return rect;
            }

            rect.X = r1.X < r2.X ? r2.X : r1.X;
            rect.Y = r1.Y < r2.Y ? r2.Y : r1.Y;
 
            if (r1.X <= r2.X)
            {
                rect.Width = r1.Right <= r2.Right ? r1.Right - r2.X : r2.Width;
            }
            else
            {
                rect.Width = r1.Right <= r2.Right ? r1.Width : r2.Right - r1.X;
            }
            if (r1.Y<=r2.Y)
            {
                rect.Height = r1.Bottom <= r2.Bottom ? r1.Bottom - r2.Y : r2.Height;
            }
            else
            {
                rect.Height = r1.Bottom <= r2.Bottom ? r1.Height : r2.Bottom - r1.Y;
            }
             
            return rect;
        }
        /// <summary>
        /// Rect0在Rect1绝对位置 对应返回在Rect2 的相对位置Rect
        /// </summary>
        /// <param name="r0">原物理大屏坐标点</param>
        /// <param name="r1">原物理大屏框架</param>
        /// <param name="r2">相对Canvas框架</param>
        /// <returns></returns>
        public static Rect ConverterRect0ToRect1(Rect r0, Rect r1, Rect r2)
        {
             
            int pX0 = ((r0.X - r1.X) / r1.Width) * r2.Width + r2.X;
            int pY0 = ((r0.Y - r1.Y) / r1.Height) * r2.Height + r2.Y;
            int pX1 = (r0.Width / r1.Width) * r2.Width; //宽度
            int pY1 = (r0.Height / r1.Height) * r2.Height; //高度
            return new Rect(pX0, pY0, pX1, pY1);
             
        }

        //
        // 摘要:
        //     比较两个矩形是否完全相等。
        //
        // 参数:
        //   rect1:
        //     要比较的第一个矩形。
        //
        //   rect2:
        //     要比较的第二个矩形。
        //
        // 返回结果:
        //     如果这些矩形具有相同的 System.Windows.Rect.Location 和 System.Windows.Rect.Size 值，则为 true；否则为
        //     false。
        public static bool operator ==(Rect rect1, Rect rect2)
        {
            if (rect1.X!=rect2.X||rect1.Y != rect2.Y||rect1.Width != rect2.Width||rect1.Height!=rect2.Height)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //
        // 摘要:
        //     比较两个矩形是否不相等。
        //
        // 参数:
        //   rect1:
        //     要比较的第一个矩形。
        //
        //   rect2:
        //     要比较的第二个矩形。
        //
        // 返回结果:
        //     如果这些矩形具有不相同的 System.Windows.Rect.Location 和 System.Windows.Rect.Size 值，则为 true；否则为
        //     false。
        public static bool operator !=(Rect rect1, Rect rect2)
        {
            return rect1 == rect2 ? false : true;
        }
        //
        // 摘要:
        //     指示指定的矩形是否相等。
        //
        // 参数:
        //   rect1:
        //     要比较的第一个矩形。
        //
        //   rect2:
        //     要比较的第二个矩形。
        //
        // 返回结果:
        //     true 如果这些矩形具有相同 System.Windows.Rect.Location 和 System.Windows.Rect.Size 值; 否则为
        //     false。
        public static bool Equals(Rect rect1, Rect rect2)
        {
            return rect1 == rect2;
        }
        public override string ToString()
        {
            return X + "," + Y + "," + Width + "," + Height;
        }
        public override bool Equals(object obj)
        {
            if (obj is Rect)
            {
                return this == (Rect)obj;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
