using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Veldrid.PBR
{
    [TestFixture]
    public class GltfSampleFiles
    {
        public static IEnumerable<string> GltfFiles
        {
            get
            {
                var roolFolder = Path.GetFullPath(Path.Combine(typeof(GltfSampleFiles).Assembly.Location,
                    @"..\..\..\..\..\..\modules\glTF-Sample-Models\2.0\"));
                foreach (var file in Directory.GetFiles(roolFolder, "*.gltf", SearchOption.AllDirectories))
                    yield return file;
                foreach (var file in Directory.GetFiles(roolFolder, "*.glb", SearchOption.AllDirectories))
                    yield return file;
            }
        }

        [Test]
        [TestCaseSource(nameof(GltfFiles))]
        public void ConvertFile(string fileName)
        {
            var content = GltfConverter.ReadGtlf(fileName);
        }
    }
}