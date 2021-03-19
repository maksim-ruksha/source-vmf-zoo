using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using SourceVmfZoo.Util;
using SourceVmfZoo.Vmf;

namespace SourceVmfZoo
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string entities = ConfigurationManager.AppSettings["InsertionString"];

                string currentDirectory = Directory.GetCurrentDirectory();

                string templatePath = Directory.GetCurrentDirectory() + "\\" +
                                      ConfigurationManager.AppSettings["TemplateFileName"];

                string usagesPath;
                string inputPath;
                string outputPath;
                if (args.Length == 0)
                {
                    // use input.vmf file
                    inputPath = currentDirectory + "\\" + ConfigurationManager.AppSettings["DefaultInputFileName"];
                    outputPath = currentDirectory + "\\" + ConfigurationManager.AppSettings["DefaultOutputFileName"];
                    usagesPath = currentDirectory + "\\" + ConfigurationManager.AppSettings["UsagesFileName"];
                }
                else
                {
                    // use provided path
                    // still using template.txt next to exe tho
                    if (args.Length > 1)
                        Console.WriteLine("Some redundant parameters were specified. Will pretend I did not see them.");
                    inputPath = args[0];
                    FileInfo fileInfo = new FileInfo(inputPath);
                    string fileName = fileInfo.Name;
                    string outputFileName = fileName.Insert(fileName.LastIndexOf('.'),
                        ConfigurationManager.AppSettings["CustomOutputPostfix"]);
                    string usagesFileName =
                        fileName.Replace(".vmf", $"_{ConfigurationManager.AppSettings["UsagesFileName"]}");
                    outputPath = fileInfo.DirectoryName + "\\" + outputFileName;
                    usagesPath = fileInfo.DirectoryName + "\\" + usagesFileName;
                }


                Console.WriteLine("Reading...");
                string templateContent = File.ReadAllText(templatePath);
                List<string> corruptedModels;

                List<VmfModel> models = VmfParser.Parse(inputPath, out corruptedModels).ToList();
                List<Usage> usages = VmfParser.GetUsages(models).ToList();

                ReadOffsets(out double inLineOffset, out double betweenLineOffset, usages.Count);

                Console.WriteLine("Processing...");
                VmfZoo zoo = new VmfZoo(models.ToHashSet(), (float) inLineOffset, (float) betweenLineOffset);

                templateContent = templateContent.Replace(entities, zoo.GetPlacedModels());
                Console.WriteLine("Writing...");
                File.WriteAllText(outputPath, templateContent);

                // write stats to file

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append($"CORRUPTED ENTITIES ({corruptedModels.Count}):\n");
                for (int i = 0; i < corruptedModels.Count; i++)
                {
                    stringBuilder.Append($"{i + 1}:\n{corruptedModels[i]}");
                }

                stringBuilder.Append($"USAGES ({usages.Count}):\n");
                for (int i = 0; i < usages.Count; i++)
                {
                    Usage usage = usages[i];
                    stringBuilder.Append($"{i + 1}: {usage}\n");
                }

                File.WriteAllText(usagesPath, stringBuilder.ToString());

                Console.WriteLine(
                    $"Done. Enjoy your prop zoo! You can open {usagesPath} to check every prop usages count.");
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Some shit happened: {e.Message}");
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
        }

        private static void ReadOffsets(out double inLineOffset, out double betweenLineOffset, int propsCount)
        {
            Console.WriteLine("Specify offset in line (or just press enter for automatic offsets): ");
            inLineOffset = ReadUntilValid();
            if (inLineOffset < 0)
            {
                AutoOffset autoOffset = new AutoOffset(propsCount);
                inLineOffset = autoOffset.Height;
                betweenLineOffset = autoOffset.Width;
                Console.WriteLine($"Automatic offsets: {inLineOffset} {betweenLineOffset}");
            }
            else
            {
                Console.WriteLine("Specify offset between lines:");
                betweenLineOffset = ReadUntilValid();
            }
        }

        private static double ReadUntilValid()
        {
            double value = -1.0;
            string s = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(s) && !Double.TryParse(s, out value))
            {
                Console.WriteLine("Invalid value.");
            }

            return value;
        }
    }
}