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
            
            ReplaceParameter("origin", origin.ToString());
            ReplaceParameter("angles", Vector3.Zero.ToString());

            return FullContent;
        }
        public string ToStringWithOriginAndNumber(Vector3 origin, int number)
        {
            FullContent = $"// model #{number}\n" + FullContent;
            ReplaceParameter("origin", origin.ToString());
            ReplaceParameter("angles", Vector3.Zero.ToString());

            return FullContent;
        }

        private void ReplaceParameter(string parameterName, string parameterValue)
        {
            parameterName = "\"" + parameterName + "\"";
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