using LibVLCSharp.Shared;
using Microsoft.Extensions.Hosting;
using SkiaSharp;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace BlazorRtsPlayer.BlazorApp
{
    public class RtspInMemoryService
    {
        private const uint Width = 1280;
        private const uint Height = 960;

        /// <summary>
        /// RGBA is used, so 4 byte per pixel, or 32 bits.
        /// </summary>
        private const uint BytePerPixel = 4;

        /// <summary>
        /// the number of bytes per "line"
        /// For performance reasons inside the core of VLC, it must be aligned to multiples of 32.
        /// </summary>
        private readonly uint Pitch;

        /// <summary>
        /// The number of lines in the buffer.
        /// For performance reasons inside the core of VLC, it must be aligned to multiples of 32.
        /// </summary>
        private readonly uint Lines;

        private SKBitmap CurrentBitmap;
        private long FrameCounter = 0;
        private readonly ConcurrentQueue<SKBitmap> FilesToProcess = new ConcurrentQueue<SKBitmap>();
        
        public delegate void SnapshotEventHandler(string bitmap);
        public event SnapshotEventHandler Snapshot;

        public RtspInMemoryService()
        {
            Pitch = Align(Width * BytePerPixel);
            Lines = Align(Height);

            uint Align(uint size)
            {
                if (size % 32 == 0)
                {
                    return size;
                }

                return ((size / 32) + 1) * 32;// Align on the next multiple of 32
            }
        }

        private async Task ProcessThumbnailsAsync(CancellationToken token)
        {
            var frameNumber = 0;
            var surface = SKSurface.Create(new SKImageInfo((int)Width, (int)Height));
            var canvas = surface.Canvas;
            while (!token.IsCancellationRequested)
            {
                if (FilesToProcess.TryDequeue(out var bitmap))
                {
                    canvas.DrawBitmap(bitmap, 0, 0); // Effectively crops the original bitmap to get only the visible area


                    using (var outputImage = surface.Snapshot())
                    using (var data = outputImage.Encode(SKEncodedImageFormat.Jpeg, 50))
                    {
                        var str = Convert.ToBase64String(data.ToArray());
                        Snapshot?.Invoke(str);
                    }
                    // https://base64.guru/converter/decode/image
                    bitmap.Dispose();
                    frameNumber++;
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), token);
                }
            }
        }

        private IntPtr Lock(IntPtr opaque, IntPtr planes)
        {
            CurrentBitmap = new SKBitmap(new SKImageInfo((int)(Pitch / BytePerPixel), (int)Lines, SKColorType.Bgra8888));
            Marshal.WriteIntPtr(planes, CurrentBitmap.GetPixels());
            return IntPtr.Zero;
        }

        private void Display(IntPtr opaque, IntPtr picture)
        {
            if (FrameCounter % 1 == 0 && CurrentBitmap != null) // take only every 100. image
            {
                FilesToProcess.Enqueue(CurrentBitmap);
                CurrentBitmap = null;
            }
            else
            {
                CurrentBitmap.Dispose();
                CurrentBitmap = null;
            }
            FrameCounter++;
        }

        public async Task ExecuteAsync()
        {

            // Load native libvlc library
            Core.Initialize();

            using (var libvlc = new LibVLC())
            using (var mediaPlayer = new MediaPlayer(libvlc))
            {
                // Listen to events
                var processingCancellationTokenSource = new CancellationTokenSource();
                mediaPlayer.Stopped += (s, e) => processingCancellationTokenSource.CancelAfter(1);

                // Create new media
                var media = new Media(libvlc, new Uri("rtsp://rtsp.stream/pattern"));
                media.AddOption(":no-audio");
                // Set the size and format of the video here.
                mediaPlayer.SetVideoFormat("RV32", Width, Height, Pitch);
                mediaPlayer.SetVideoCallbacks(Lock, null, Display);

                // Start recording
                mediaPlayer.Play(media);

                // Waits for the processing to stop
                try
                {
                    await ProcessThumbnailsAsync(processingCancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                { }
            }
            await Task.CompletedTask;
        }


    }
}
