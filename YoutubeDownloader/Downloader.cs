using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace YoutubeDownloader
{
	class Downloader
	{
		static void Main(string[] args)
		{
			var opts = new CliOptions();
			if(CommandLine.Parser.Default.ParseArguments(args, opts))
			{
				var downloads = opts.YouTubeUrls.Select(url => new Downloader(url, opts.Verbose)).ToList();

				foreach(var d in downloads)
				{
					d.Resolve();
				}

				Console.WriteLine("\n\nDownload...");

				foreach(var d in downloads)
				{
					d.Download();
				}
			}
		}

		private String url;
		private VideoInfo video;
		private ProgressBar bar;
		private bool verbose;

		public Downloader(String url, bool verbose)
		{
			this.url = url;
			this.verbose = verbose;
		}

		private void WriteVerbose(String msg)
		{
			if(verbose)
				Console.WriteLine(msg);
		}

		public void Resolve()
		{
			try {
				WriteVerbose("Examine Video at <" + url + ">");
				var videoInfos = DownloadUrlResolver.GetDownloadUrls(url).OrderByDescending(info => info.Resolution);

				WriteVerbose("Videos:");
				foreach (var info in videoInfos)
				{
					WriteVerbose(info.ToString());
				}

				var audioInfos = videoInfos
					.Where(info => info.CanExtractAudio)
					.OrderByDescending(info => info.AudioBitrate);

				WriteVerbose("\nAudio:");
				foreach (var info in audioInfos)
				{
					WriteVerbose(info.ToString());
				}

				video = audioInfos.First();
			} catch (Exception e)
			{
				Console.WriteLine("ERR: Failed to fetch video from <" + url + ">");
				Console.WriteLine("=====> " + e.Message);
			}
		}
		 
		void Download()
		{
			if (video == null)
			{
				WriteVerbose("Skip <" + url + ">");
				return;
			}

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

			Console.WriteLine(video.Title);
			bar = new ProgressBar();
			// Register the progress events. We treat the download progress as 85% of the progress and the extraction progress only as 15% of the progress,
			// because the download will take much longer than the audio extraction.
			audioDownloader.DownloadProgressChanged += (sender, args) => bar.Report(args.ProgressPercentage * 0.85 * 0.01);
			audioDownloader.AudioExtractionProgressChanged += (sender, args) => bar.Report(85 + args.ProgressPercentage * 0.15 * 0.01);

			/*
			 * Execute the audio downloader.
			 * For GUI applications note, that this method runs synchronously.
			 */
			audioDownloader.Execute();
		}
	}
}
