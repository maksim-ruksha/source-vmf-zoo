using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceVmfZoo.Log;
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
            {
                LogManager.Log("VmfZoo: Can not fit models with specified offsets");
                throw new Exception($"Can't fit models with current offsets! {_inLineOffset} {_betweenLineOffset}");
            }

            List<VmfModel> list = _vmfModels.ToList();
            StringBuilder stringBuilder = new StringBuilder();

            // in Source Z axis is up, so Y and Z are swapped
            Vector3 position = new Vector3(MinMapDimensionValue, MinMapDimensionValue, 0);
            int index = 0;
            bool passedHalfY = false;
            bool passedHalfX = false;
            LogManager.Log("VmfZoo: Placing models");
            for (position.Y = MinMapDimensionValue; position.Y <= MaxMapDimensionValue; position.Y += _betweenLineOffset)
            {
                if (!passedHalfY && position.Y > 0)
                    passedHalfY = true;
                for (position.X = MinMapDimensionValue; position.X <= MaxMapDimensionValue; position.X += _inLineOffset)
                {
                    if (!passedHalfX && position.X > 0)
                        passedHalfX = true;
                    if (index >= list.Count)
                    {
                        if(!passedHalfX)
                            LogManager.Log("VmfZoo: Warning! Did not passed half of X axis (pretty normal for user offset)!");
                        if(!passedHalfY)
                            LogManager.Log("VmfZoo: Warning! Did not passed half of Y axis (pretty normal for user offset)!");
                        return stringBuilder.ToString();
                    }

                    LogManager.Log($"VmfZoo: Prop #{index} {position.X} {position.Y} {position.Z}");
                    VmfModel model = list[index];
                    stringBuilder.Append(model.ToStringWithOriginAndNumber(position, index));
                    index++;
                    
                }
            }

            // wtf
            LogManager.Log($"VmfZoo: Warning! Prop placing loops ended by themselves");
            Console.WriteLine(
                "Something strange happened: writing loops ended by themselves, not by \"return\" condition. Result might be corrupted or not full.");
            return stringBuilder.ToString();
        }


        private bool CanFitModels()
        {
            int modelsPerLine = (int) Math.Floor(MapSize / _inLineOffset + 1);
            int lineCount = (int) Math.Floor(MapSize / _betweenLineOffset);
            int canFitCount = lineCount * modelsPerLine;
            return canFitCount >= _vmfModels.Count();
        }
    }
}