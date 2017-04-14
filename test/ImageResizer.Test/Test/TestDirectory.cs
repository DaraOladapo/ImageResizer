using System;
using System.IO;
using IOPath = System.IO.Path;

namespace ImageResizer
{
    public class TestDirectory : IDisposable
    {
        public TestDirectory()
        {
            Path = IOPath.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                IOPath.GetRandomFileName());
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
            => Directory.Delete(Path, recursive: true);
    }
}