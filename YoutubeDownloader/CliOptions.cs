using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader
{
	class CliOptions
	{
		[ValueOption(0)]
		public string YouTubeUrl { get; set; }

		[ValueOption(1)]
		public string DestFile { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return
				"YoutubeDownloader <url> <dest>\n";
		}
	}
}
