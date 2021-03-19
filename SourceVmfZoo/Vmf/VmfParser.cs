using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SourceVmfZoo.Util;

namespace SourceVmfZoo.Vmf
{
    public static class VmfParser
    {
        public static IEnumerable<VmfModel> Parse(string filePath, out List<string> corruptedModels)
        {
            string file = ReadFile(filePath);
            IEnumerable<VmfModel> models = FindModels(file, out corruptedModels);

            return models;
        }

        public static IEnumerable<Usage> GetUsages(IEnumerable<VmfModel> models)
        {
            List<VmfModel> vmfModels = models.ToList();
            HashSet<VmfModel> set = vmfModels.ToHashSet();
            List<Usage> usages = new List<Usage>();

            foreach (var setModel in set)
            {
                int count = vmfModels.Count(m => m.Equals(setModel));

                Usage usage = new Usage(setModel.Name, count);
                usages.Add(usage);
            }

            usages.Sort((s1, s2) => s2.Count - s1.Count);
            return usages;
        }

        private static IEnumerable<VmfModel> FindModels(string fileContent, out List<string> corruptedModels)
        {
            List<VmfModel> models = new List<VmfModel>();

            corruptedModels = new List<string>();

            const string regex = "(entity(\r|\n|\r\n|\n\r)|hidden)";

            MatchCollection matches = Regex.Matches(fileContent, regex, RegexOptions.None, TimeSpan.FromMinutes(5.0));

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                // if some entity found
                if (match.Value.Contains("entity"))
                {
                    // check if it is not hidden
                    if (i - 1 < 0 || !matches[i - 1].Value.Contains("hidden"))
                    {
                        // check if it static prop
                        int entityValueStartIndex = match.Index;
                        int entityValueEndIndex =
                            i + 1 >= matches.Count ? fileContent.Length - 1 : matches[i + 1].Index;
                        string entityContent = fileContent.Substring(entityValueStartIndex,
                            entityValueEndIndex - entityValueStartIndex);
                        try
                        {
                            if (entityContent.Contains("prop_static"))
                            {
                                // yep this is static prop
                                VmfModel model = new VmfModel(entityContent);
                                models.Add(model);
                            }
                        }
                        catch (Exception)
                        {
                            corruptedModels.Add(entityContent);
                        }
                    }
                }
            }

            return models;
        }

        private static string ReadFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            StringBuilder stringBuilder = new StringBuilder();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                stringBuilder.Append(line + "\n");
            }

            return stringBuilder.ToString();
        }
    }
}