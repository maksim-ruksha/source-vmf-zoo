using System;
using System.Collections.Generic;
using SourceVmfZoo.Log;
using SourceVmfZoo.Vmf;

namespace SourceVmfZoo.Util
{
    public class AutoOffset
    {
        public float Width { get; }
        public float Height { get; }

        private readonly struct DividerInfo
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
            float[] dimensions = GetOptimalDimensions(propsCount);
            Width = dimensions[0];
            Height = dimensions[1];
        }

        private static float[] GetOptimalDimensions(int propsCount)
        {
            
            int dividablePropsCount = propsCount;
            while (IsPrime(dividablePropsCount))
            {
                dividablePropsCount--;
            }

            LogManager.Log($"AutoOffset: {propsCount} props, dividable count: {dividablePropsCount}");

            bool additionalLine = dividablePropsCount < propsCount;

            int[] dividers = GetDividersOf(dividablePropsCount);
            List<DividerInfo> infos = new List<DividerInfo>();


            for (int w = dividers.Length - 1; w >= 0; w--)
            {
                for (int h = dividers.Length - 1; h >= 0; h--)
                {
                    int divider1 = Math.Max(dividers[w], dividers[h]);
                    int divider2 = Math.Min(dividers[w], dividers[h]);

                    while (divider1 * divider2 >= propsCount)
                    {
                        bool decreased = false;
                        if ((divider1 - 1) * divider2 >= propsCount)
                        {
                            divider1--;
                            decreased = true;
                        }

                        if (divider1 * (divider2 - 1) >= propsCount)
                        {
                            divider2--;
                            decreased = true;
                        }

                        if (!decreased)
                            break;
                    }

                    int delta = Math.Abs(divider1 - divider2);
                    int canFitProps = divider1 * divider2;

                    // little explanation about what happens in next line
                    // so we need to find "most square" rectangle (which sides are divider1 x divider2)
                    // this rectangle must be large enough to fit every prop and as small as possible
                    // because we don't want to leave empty space

                    // to calculate squareness of rectangle, we find delta of sides, and then divide 1 by delta + 0.1
                    // + 0.1 excludes case when delta = 0
                    // squareness multiplied by 1 divided by amount of empty places, which will remain after placing props
                    // also + 0.1 to exclude division by zero

                    // UPD: moved 1.0f / out of brackets
                    float weight = 1.0f / ((propsCount - canFitProps + 0.1f) *
                                           (delta + 0.1f));

                    // if rectangle with current sides can  fit all props, then add it to list
                    if (canFitProps >= dividablePropsCount)
                        infos.Add(new DividerInfo(
                            weight, w,
                            h));
                }
            }

            // sort and get optimal rectangle sizes
            infos.Sort((info1, info2) => (int) Math.Floor(info2.Weight - info1.Weight));
            DividerInfo best = infos[0];
            LogManager.Log("AutoOffset: Possible rectangles:");
            foreach (var i in infos)
            {
                LogManager.Log(
                    $"AutoOffset: {dividers[i.Index1]}x{dividers[i.Index2]}, Weight = {i.Weight / best.Weight}");
            }

            float offset1 = (float) Math.Floor(VmfZoo.MapSize / (dividers[best.Index1] + (additionalLine ? 1 : 0)));
            float offset2 = (float) Math.Floor(VmfZoo.MapSize / dividers[best.Index2]);
            LogManager.Log(
                $"AutoOffset: Optimal: {dividers[best.Index1] + (additionalLine ? 1 : 0)} {dividers[best.Index2]}");
            LogManager.Log($"AutoOffset: Offsets: {offset1} {offset2}");
            return new[]
            {
                offset1,
                offset2
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