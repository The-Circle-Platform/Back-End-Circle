using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace TheCircleBackend.Hubs
{
    public class LivestreamHub : Hub
    {
        #region snippet1
        public ChannelReader<int> Counter(
            int count,
            int delay,
            CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<int>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);

            Console.WriteLine("testinggg");

            return channel.Reader;
        }




        private async Task WriteItemsAsync(
            ChannelWriter<int> writer,
            int count,
            int delay,
            CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    await writer.WriteAsync(i, cancellationToken);

                    // Use the cancellationToken in other APIs that accept cancellation
                    // tokens so the cancellation can flow down to them.
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                localException = ex;
            }
            finally
            {
                writer.Complete(localException);
            }
        }
        #endregion

        #region snippet2
        public async Task UploadStream(ChannelReader<string> stream)
        {
            while (await stream.WaitToReadAsync())
            {
                while (stream.TryRead(out var item))
                {
                    // do something with the stream item
                    Console.WriteLine(item);
                    Console.WriteLine("testinggg22");
                }
            }
        }
        #endregion

        public async Task SendBlobs(Blob blob)
        {

            try
            {
                string ffmpegCommand = "ffmpeg -i input.mp4 -c:v libx264 -preset medium -crf 23 -c:a aac -b:a 128k output.mp4"; // Vervang dit door het gewenste FFmpeg-commando

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "captured-video",
                    Arguments = ffmpegCommand,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process
                {
                    StartInfo = startInfo
                };

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                Console.WriteLine("result:", output);

            } catch (Exception ex)
            {
                Console.WriteLine("Error ffmpeg:", ex);
            }


            byte[] bytes = null;
            Console.WriteLine("Blobs received");
            try
            {
                Console.WriteLine("Bytes:", blob);
                Console.WriteLine("Length:", blob.Length);




                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Download the Blob's content to the MemoryStream
                    //foreach (Blob item in blob)
                    //{
                    //item.GetBytes().CopyTo(bytes);
                    //}
                    // Perform operations on the downloaded content
                    // For example, read the contents as a string
                    string blobContent = Encoding.UTF8.GetString(memoryStream.ToArray());
                    string byteContent = Encoding.UTF8.GetString(bytes.ToArray());

                    Console.WriteLine(blobContent);
                    Console.WriteLine(byteContent);

                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Error found:", err);
            }
        }

        public async Task ReceiveVideoBlob(Blob videoData)
        {
            byte[] bytes= null;
            Console.WriteLine("ReceiveVideoBlob called");
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    videoData.
                    bytes= videoData.GetBytes().ToArray();

                    
                    memoryStream.ToArray();
                }
                foreach (var video in bytes)
                {
                    Console.WriteLine($"{video}");
                    Console.WriteLine(video.ToString());
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Error in ReceiveVideoBlob:", ex.Message);
                Console.WriteLine(ex.Source);
            }
        }

        public void DownloadVideo(Blob videoBlob)
        {
            Response.Clear();
            Response.ContentType = videoBlob.ContentType;
            Response.Headers.Add("Content-Disposition", "attachment; filename=video.mp4");

            using (var stream = videoBlob.OpenRead())
            {
                stream.CopyTo(Response.Body);
                Response.Body.Flush();
            }
        }
    }
}
