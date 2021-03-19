using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceVmfZoo.Util;

namespace SourceVmfZoo.Vmf
{
    public class VmfZoo
    {
        private const float MaxMapDimensionValue = 15360.0f;
        private const float MinMapDimensionValue = -15360.0f;
        public const float MapSize = 30720.0f;

        private readonly IEnumerable<VmfModel> _vmfModels;
        private readonly float _inLineOffset;
        private readonly float _betweenLineOffset;

        public VmfZoo(IEnumerable<VmfModel> vmfModels, float inLineOffset,
            float betweenLineOffset)
        {
            _vmfModels = vmfModels;
            _inLineOffset = inLineOffset;
            _betweenLineOffset = betweenLineOffset;
        }

        public string GetPlacedModels()
        {
            if (!CanFitModels())
                throw new Exception($"Can't fit models with current offsets! {_inLineOffset} {_betweenLineOffset}");

            List<VmfModel> list = _vmfModels.ToList();
            StringBuilder stringBuilder = new StringBuilder();

            // in Source Z axis is up, so Y and Z are swapped
            Vector3 position = new Vector3(MinMapDimensionValue, MinMapDimensionValue, 0);
            int index = 0;
            //Console.WriteLine($"Offsets: inLine: {_inLineOffset} betweenLine: {_betweenLineOffset}");
            for (position.Y = MinMapDimensionValue; position.Y <= MaxMapDimensionValue; position.Y += _betweenLineOffset)
            {
                for (position.X = MinMapDimensionValue; position.X <= MaxMapDimensionValue; position.X += _inLineOffset)
                {
                    //Console.WriteLine(position);
                    if (index >= list.Count)
                        return stringBuilder.ToString();

                    VmfModel model = list[index];
                    index++;
                    stringBuilder.Append(model.ToStringWithOrigin(position));
                }
            }

            // wtf
            Console.WriteLine(
                "Something strange happened: writing cycles ended not by \"return\" condition. Result might be corrupted or not full.");
            return stringBuilder.ToString();
        }

        public static bool CanFitModels(int propsCount, float inLineOffset, float betweenLineOffset)
        {
            int modelsPerLine = (int) Math.Floor(MapSize / inLineOffset);
            int lineCount = (int) Math.Floor(MapSize / betweenLineOffset);
            int canFitCount = lineCount * modelsPerLine;
            Console.WriteLine($"Can fit count {canFitCount}");
            return canFitCount >= propsCount;
        }
        public bool CanFitModels()
        {
            int modelsPerLine = (int) Math.Floor(MapSize / _inLineOffset + 1);
            int lineCount = (int) Math.Floor(MapSize / _betweenLineOffset);
            int canFitCount = lineCount * modelsPerLine;
            return canFitCount >= _vmfModels.Count();
        }
    }
}