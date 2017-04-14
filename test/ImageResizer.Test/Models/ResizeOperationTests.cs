using ImageResizer.Properties;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using Xunit;

namespace ImageResizer.Models
{
    public class ResizeOperationTests
    {
        [Fact]
        public void Execute_copies_frame_metadata()
        {
            using (var directory = new TestDirectory())
            {
                var operation = new ResizeOperation(
                    "Test.jpg",
                    directory.Path,
                    new Settings
                    {
                        Sizes = new ObservableCollection<ResizeSize>
                            {
                                new ResizeSize
                                {
                                    Name = "Test",
                                    Width = 96,
                                    Height = 96
                                }
                            },
                        SelectedSizeIndex = 0
                    });

                operation.Execute();

                using (var stream = File.OpenRead(Path.Combine(directory.Path, "Test (Test).jpg")))
                {
                    var image = BitmapDecoder.Create(
                        stream,
                        BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.None);

                    Assert.Equal("Brice Lambson", ((BitmapMetadata)image.Frames[0].Metadata).Author[0]);
                }
            }
        }

        [Fact]
        public void Execute_keeps_date_modified()
        {
            using (var directory = new TestDirectory())
            {
                var operation = new ResizeOperation(
                    "Test.jpg",
                    directory.Path,
                    new Settings
                    {
                        Sizes = new ObservableCollection<ResizeSize>
                            {
                                new ResizeSize
                                {
                                    Name = "Test",
                                    Width = 96,
                                    Height = 96
                                }
                            },
                        SelectedSizeIndex = 0,
                        KeepDateModified = true
                    });

                operation.Execute();

                Assert.Equal(
                    File.GetLastWriteTimeUtc("Test.jpg"),
                    File.GetLastWriteTimeUtc(Path.Combine(directory.Path, "Test (Test).jpg")));
            }
        }

        [Fact]
        public void Execute_replaces_originals()
        {
            using (var directory = new TestDirectory())
            {
                var path = Path.Combine(directory.Path, "Test.jpg");
                File.Copy("Test.jpg", path);

                var operation = new ResizeOperation(
                    path,
                    null,
                    new Settings
                    {
                        Sizes = new ObservableCollection<ResizeSize>
                            {
                                new ResizeSize
                                {
                                    Name = "Test",
                                    Width = 96,
                                    Height = 96
                                }
                            },
                        SelectedSizeIndex = 0,
                        Replace = true
                    });

                operation.Execute();

                using (var stream = File.OpenRead(path))
                {
                    var image = BitmapDecoder.Create(
                        stream,
                        BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.None);

                    Assert.False(File.Exists(Path.Combine(directory.Path, "Test (Test).jpg")));
                    Assert.Equal(96, image.Frames[0].PixelWidth);
                }
            }
        }

        [Fact]
        public void Execute_uniquifies_output_filename()
        {
            using (var directory = new TestDirectory())
            {
                File.WriteAllBytes(Path.Combine(directory.Path, "Test (Test).jpg"), new byte[0]);

                var operation = new ResizeOperation(
                    "Test.jpg",
                    directory.Path,
                    new Settings
                    {
                        Sizes = new ObservableCollection<ResizeSize>
                            {
                                new ResizeSize
                                {
                                    Name = "Test",
                                    Width = 96,
                                    Height = 96
                                }
                            },
                        SelectedSizeIndex = 0,
                        KeepDateModified = true
                    });

                operation.Execute();

                Assert.True(File.Exists(Path.Combine(directory.Path, "Test (Test) (1).jpg")));
            }
        }

        [Fact]
        public void Execute_uniquifies_output_filename_again()
        {
            using (var directory = new TestDirectory())
            {
                File.WriteAllBytes(Path.Combine(directory.Path, "Test (Test).jpg"), new byte[0]);
                File.WriteAllBytes(Path.Combine(directory.Path, "Test (Test) (1).jpg"), new byte[0]);

                var operation = new ResizeOperation(
                    "Test.jpg",
                    directory.Path,
                    new Settings
                    {
                        Sizes = new ObservableCollection<ResizeSize>
                            {
                                new ResizeSize
                                {
                                    Name = "Test",
                                    Width = 96,
                                    Height = 96
                                }
                            },
                        SelectedSizeIndex = 0,
                        KeepDateModified = true
                    });

                operation.Execute();

                Assert.True(File.Exists(Path.Combine(directory.Path, "Test (Test) (2).jpg")));
            }
        }

        // TODO
        // names output using format
        // works with read-only codecs
        // transforms each frame
        // transforms
        //     ignores orientation
        //     converts units
        //     shrinks only
        //     uses fit
        //         crops when fill
    }
}
