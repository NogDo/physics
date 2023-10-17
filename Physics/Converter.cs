using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace Personal_Project_Game.Physics
{
    public class Converter
    {
        /// <summary>
        /// Vector값을 PointF로 바꾸는 함수.. Vector는 내가 임의로 만든 구조체기때문에 그래픽상에서 좌표값을 전달할때는 Graphic의 Point값으로 바꿔서 전달해줘야한다.
        /// </summary>
        /// <param name="v">바꿀 Vector값</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF toPointF(Vector2 v)
        {
            return new PointF(v.x, v.y);
        }

        /// <summary>
        /// Vector배열을 PointF배열로 바꾸는 함수.. Vector는 내가 임의로 만든 구조체기때문에 그래픽상에서 좌표값을 전달할때는 Graphic의 Point값으로 바꿔서 전달해줘야한다.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF[] toPointFArray(Vector2[] src, ref Vector2[] dst)
        {
            PointF[] pointF = new PointF[src.Length];

            if(dst is null || src.Length != dst.Length)
            {
                dst = new Vector2[src.Length];
            }

            for(int i = 0; i < src.Length; i++)
            {
                dst[i] = src[i];
                pointF[i] = toPointF(dst[i]);
            }

            return pointF;
        }
    }
}
