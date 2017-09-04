using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FmdlTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) {
                string initalPath = args[0];
                string[] files = { args[0] };
                if (Directory.Exists(initalPath))
                {
                    files = Directory.GetFiles(initalPath, "*.fmdl", SearchOption.AllDirectories);
                }

                bool outputHashes = false;
                string dictionaryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\fmdl_dictionary.txt";

                if (args.Length > 1) {
                    for (int i = 1; i < args.Length; i++) {
                        string arg = args[i];
                        string argL = args[i].ToLower();
                        if (argL == "-outputhashes" || argL == "-o") {
                            outputHashes = true;
                        } else {
                            if (argL == "-dictionary" || argL == "-d") {
                                if (i + 1 < args.Length) {
                                    dictionaryPath = args[i + 1];
                                }
                            }
                        }
                    }
                }

                Hashing.ReadDictionary(dictionaryPath);

                //Parallel.ForEach(files, (path) =>
                foreach (string path in files)
                {
                    Console.WriteLine(path);
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        Fmdl file = new Fmdl();
                        file.Read(stream);
                        if (!outputHashes) {
                            file.OutputSection0Block0Info();
                            file.OutputSection0Block2Info();
                            file.OutputSection0Block3Info();
                            file.OutputSection0Block5Info();
                            file.OutputSection0Block7Info();
                            file.OutputSection0Block8Info();
                            file.OutputSection0BlockDInfo();
                            file.OutputObjectInfo2();
                            file.OutputStringInfo();
                        }
                        if (outputHashes)
                        {
                            string fileDirectory = Path.GetDirectoryName(path);

                            var str32Hashes = file.OutputHashSection();
                            if (str32Hashes != null)
                            {
                                string outputPath = Path.Combine(fileDirectory, string.Format("{0}_str64Hashes.txt", Path.GetFileName(path)));
                                File.WriteAllLines(outputPath, str32Hashes);
                            }

                            var textureHashes = file.OutputTextureHashes();
                            if (textureHashes != null)
                            {
                                string outputPath = Path.Combine(fileDirectory, string.Format("{0}_textureHashes.txt", Path.GetFileName(path)));
                                File.WriteAllLines(outputPath, textureHashes);
                            }

                            //tex strings from GZ format fmdls
                            var strings = file.OutputStrings();
                            if (strings != null)
                            {
                                string outputPath = Path.Combine(fileDirectory, string.Format("{0}_strings.txt", Path.GetFileName(path)));
                                File.WriteAllLines(outputPath, strings);
                            }

                            //tex more fine grained division of str32Hashes 

                            var boneNameHashes = file.OutputBoneNameHashes();
                            if (boneNameHashes != null)
                            {
                                string outputPath = Path.Combine(fileDirectory, string.Format("{0}_boneNameHashes.txt", Path.GetFileName(path)));
                                File.WriteAllLines(outputPath, boneNameHashes);
                            }

                            //tex indidual hashes currently dont tally up with full hash section
                            //not going to find any bond parents not in the bone names though so no point dumping
                            //var boneParentHashes = file.OutputBoneNameHashes();
                            //if (boneParentHashes != null)
                            //{
                            //    string outputPath = Path.Combine(fileDirectory, string.Format("{0}_boneParentHashes.txt", Path.GetFileName(path)));
                            //    File.WriteAllLines(outputPath, boneParentHashes);
                            //}

                            var meshGroupHashes = file.OutputMeshGroupHashes();
                            if (meshGroupHashes != null)
                            {
                                string outputPath = Path.Combine(fileDirectory, string.Format("{0}_meshGroupHashes.txt", Path.GetFileName(path)));
                                File.WriteAllLines(outputPath, meshGroupHashes);
                            }

                            var textureTypeHashes = file.OutputTextureTypeHashes();
                            if (textureTypeHashes != null)
                            {
                                string outputPath = Path.Combine(fileDirectory, string.Format("{0}_textureTypeHashes.txt", Path.GetFileName(path)));
                                File.WriteAllLines(outputPath, textureTypeHashes);
                            }

                            var unknownHashes = file.OutputUnknownHashes();
                            if (unknownHashes != null)
                            {
                                string outputPath = Path.Combine(fileDirectory, string.Format("{0}_unknownHashes.txt", Path.GetFileName(path)));
                                File.WriteAllLines(outputPath, unknownHashes);
                            }

                            var materialHashes = file.OutputMaterialHashes();
                            if (materialHashes != null)
                            {
                                string outputPath = Path.Combine(fileDirectory, string.Format("{0}_materialHashes.txt", Path.GetFileName(path)));
                                File.WriteAllLines(outputPath, materialHashes);
                            }
                        }//if outputhashes
                        stream.Close();
                    } //using
                }//for files
                //);
                if (!outputHashes) {
                    Console.WriteLine("done");
                    Console.ReadKey();
                }
            }
        } //Main
    } //class
} //namespace
