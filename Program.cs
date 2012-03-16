//
// Copyright 2012 ThoughtWorks, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at:
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
//

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace grayscale
{
    class Program
    {
        static void Main(string[] args)
        {
            var directory = args.Length == 0 ? "." : args[0];
            directory = directory.EndsWith("\\") ? directory : directory + "\\";
            TextWriterTraceListener log = null;
            try
            {
                using (var filestream = new FileStream(directory + "log.csv", FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    log = new TextWriterTraceListener(filestream);
                    foreach (var file in new DirectoryInfo(directory).GetFiles().Select(file => file.Name))
                    {
                        var begin = DateTime.Now;

                        using (var source = new Bitmap(Image.FromFile(file)))
                        {
                            using (var bm = ConvertToGrayscale(source))
                            {
                                bm.Save(string.Format("{0}{1}.{2}", directory, "gray", file));
                            }
                        }
                        var end = DateTime.Now;
                        log.WriteLine(string.Format("{0},gray.{0},{1}", file, end - begin));
                        log.Flush();
                    }
                }

            }
            catch (Exception ex)
            {
                log.WriteLine(ex.Message);
                log.WriteLine(ex.StackTrace);
            }
        }

        internal static Bitmap ConvertToGrayscale(Bitmap source)
        {
            var bm = new Bitmap(source.Width, source.Height);
            for (var y = 0; y < bm.Height; y++)
            {
                for (var x = 0; x < bm.Width; x++)
                {
                    var c = source.GetPixel(x, y);
                    var luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                }
            }
            return bm;
        }
    }
}
