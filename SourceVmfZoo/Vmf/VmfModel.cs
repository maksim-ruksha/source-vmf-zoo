using System;
using SourceVmfZoo.Util;

namespace SourceVmfZoo.Vmf
{
    public class VmfModel
    {
        public string Name { get; }
        private string FullContent { get; set; }

        public VmfModel(string fullContent)
        {
            FullContent = fullContent;
            Name = GetName();
        }

        private string GetName()
        {
            string nameString = "\"model\"";
            int nameIndex = FullContent.IndexOf(nameString, StringComparison.Ordinal);

            // + 1 for space
            int nameValueStartIndex = nameIndex + nameString.Length + 1;
            int nameValueEndIndex = FullContent.IndexOf("\n", nameValueStartIndex, StringComparison.Ordinal);

            string nameWithQuotes = FullContent.Substring(nameValueStartIndex, nameValueEndIndex - nameValueStartIndex);
            // - 1 because starting from 1
            // - 1 for bracket
            string name = nameWithQuotes.Substring(1, nameWithQuotes.Length - 1 - 1);

            return name;
        }

        public string ToStringWithOrigin(Vector3 origin)
        {
            /*string originString = "\"origin\"";
            int originIndex = FullContent.IndexOf(originString, StringComparison.Ordinal);
            
            // +1 for space
            int originValueStartIndex = originIndex + originString.Length + 1;
            int originValueEndIndex = FullContent.IndexOf("\n", originValueStartIndex, StringComparison.Ordinal);
            
            // also must reset
            string anglesString = "\"angles\"";
            int anglesIndex = FullContent.IndexOf(anglesString, StringComparison.Ordinal);

            int anglesValueStartIndex = anglesIndex + anglesString.Length + 1;
            int anglesValueEndIndex = FullContent.IndexOf("\n", anglesValueStartIndex, StringComparison.Ordinal);

            string newContent =
                // replace position
                FullContent.Remove(originValueStartIndex, originValueEndIndex - originValueStartIndex)
                    .Insert(originValueStartIndex, origin.ToString())
                    // reset angles
                    .Remove(anglesValueStartIndex, anglesValueEndIndex - anglesValueStartIndex)
                    .Insert(anglesValueStartIndex, Vector3.Zero.ToString());*/
            ReplaceParameter("origin", origin.ToString());
            ReplaceParameter("angles", Vector3.Zero.ToString());

            return FullContent;
        }

        private void ReplaceParameter(string parameterName, string parameterValue)
        {
            int parameterIndex = FullContent.IndexOf(parameterName, StringComparison.Ordinal);

            // +1 for space
            int parameterValueStartIndex = parameterIndex + parameterName.Length + 1;
            int parameterValueEndIndex = FullContent.IndexOf("\n", parameterValueStartIndex, StringComparison.Ordinal);

            FullContent = FullContent
                .Remove(parameterValueStartIndex, parameterValueEndIndex - parameterValueStartIndex)
                .Insert(parameterValueStartIndex, parameterValue);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            // ReSharper disable once PossibleNullReferenceException
            return Name.Equals((obj as VmfModel).Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name != null ? Name.GetHashCode() : 0) * 397;
            }
        }

        protected bool Equals(VmfModel other)
        {
            return Name == other.Name;
        }
    }
}