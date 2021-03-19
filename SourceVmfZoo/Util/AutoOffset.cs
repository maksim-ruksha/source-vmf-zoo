using System;
using System.Collections.Generic;
using SourceVmfZoo.Vmf;

namespace SourceVmfZoo.Util
{
    public class AutoOffset
    {
        public float Width { get; }
        public float Height { get; }

        private struct DividerInfo
        {
            public readonly float Weight;
            public readonly int Index1;
            public readonly int Index2;

            public DividerInfo(float weight, int index1, int index2)
            {
                Weight = weight;
                Index1 = index1;
                Index2 = index2;
            }
        }

        public AutoOffset(int propsCount)
        {
            int dividableCount = propsCount;
            while (IsPrime(dividableCount))
            {
                dividableCount--;
            }

            bool additionalLine = propsCount - dividableCount > 0;
            float[] dimensions = GetOptimalDimensions(dividableCount, additionalLine);
            Width = dimensions[0];
            Height = dimensions[1];
        }

        private static float[] GetOptimalDimensions(int propsCount, bool additionalLine)
        {
            int dividablePropsCount = propsCount;
            while (IsPrime(dividablePropsCount))
            {
                dividablePropsCount--;
            }

            int[] dividers = GetDividersOf(dividablePropsCount);
            List<DividerInfo> infos = new List<DividerInfo>();
            

            for (int w = dividers.Length - 1; w >= 0; w--)
            {
                for (int h = dividers.Length - 1; h >= 0; h--)
                {
                    int divider1 = dividers[w] + (additionalLine ? 1 : 0);
                    int divider2 = dividers[h];
                    int delta = Math.Abs(divider1 - divider2);
                    int canFitProps = divider1 * divider2;

                    // little explanation about what happens in next line
                    // so we need to find "most square" rectangle (which sides are divider1 x divider2)
                    // this rectangle must be large enough to fit every prop and as small as possible
                    // because we don't want to leave empty space
                    
                    // to calculate squareness of rectangle, we find delta of sides, and then divide 1 by delta + 0.1
                    // + 0.1 excludes case when delta = 0
                    // squareness multiplied 1 divide by amount of empty places, which will remain after placing props
                    // also + 0.1 to exclude division by zero;
                    
                    // UPD: moved 1.0f / out of brackets
                    float weight = 1.0f / ((propsCount - canFitProps + 0.1f) * (delta + 0.1f));
                    
                    // if rectangle with current sides can  fit all props, then add it to list
                    if (canFitProps >= dividablePropsCount)
                        infos.Add(new DividerInfo(
                            weight, w,
                            h));
                }
            }

            // sort and get optimal rectangle sizes
            infos.Sort((info1, info2) => (int) Math.Floor(info2.Weight - info1.Weight));
            DividerInfo info = infos[0];
            
            return new[]
            {
                (float) Math.Floor(VmfZoo.MapSize / dividers[info.Index1]),
                (float) Math.Floor(VmfZoo.MapSize / dividers[info.Index2])
            };
        }


        private static int[] GetDividersOf(int number)
        {
            int half = number / 2;
            List<int> result = new List<int>();
            for (int i = 1; i < half; i++)
            {
                if (number % i == 0)
                    result.Add(i);
            }

            return result.ToArray();
        }

        private static bool IsPrime(int number)
        {
            int sqrt = (int) Math.Sqrt(number);
            for (int i = 2; i < sqrt; i++)
            {
                if (number % i == 0)
                    return false;
            }

            return true;
        }
    }
}