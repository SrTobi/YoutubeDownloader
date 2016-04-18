using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace YoutubeDownloader
{
	class Program
	{
		static void Main(string[] args)
		{
			var opts = new CliOptions();
			if(CommandLine.Parser.Default.ParseArguments(args, opts))
			{
				Console.WriteLine("Download from " + opts.YouTubeUrl);

				var videoInfos = DownloadUrlResolver.GetDownloadUrls(opts.YouTubeUrl).OrderByDescending(info => info.Resolution);

				Console.WriteLine("\nVideos:");
				foreach(var info in videoInfos)
				{
					Console.WriteLine(info);
				}

				var audioInfos = videoInfos
					.Where(info => info.CanExtractAudio)
					.OrderByDescending(info => info.AudioBitrate);

				Console.WriteLine("\nAudio:");
				foreach (var info in audioInfos)
				{
					Console.WriteLine(info);
				}

				VideoInfo video = audioInfos.First();

				Download(video);
			}
		}

		static void Download(VideoInfo video)
		{
			/*
			* If the video has a decrypted signature, decipher it
			*/
			if (video.RequiresDecryption)
			{
				DownloadUrlResolver.DecryptDownloadUrl(video);
			}

			/*
			 * Create the audio downloader.
			 * The first argument is the video where the audio should be extracted from.
			 * The second argument is the path to save the audio file.
			 */
			var audioDownloader = new AudioDownloader(video, video.Title + video.AudioExtension);

			// Register the progress events. We treat the download progress as 85% of the progress and the extraction progress only as 15% of the progress,
			// because the download will take much longer than the audio extraction.
			audioDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage * 0.85);
			audioDownloader.AudioExtractionProgressChanged += (sender, args) => Console.WriteLine(85 + args.ProgressPercentage * 0.15);

			/*
			 * Execute the audio downloader.
			 * For GUI applications note, that this method runs synchronously.
			 */
			audioDownloader.Execute();
		}
	}
}
