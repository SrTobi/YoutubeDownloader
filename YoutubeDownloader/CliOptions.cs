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
		[ValueList(typeof(List<string>))]
		public IList<String> YouTubeUrls { get; set; }

		[Option('v')]
		public bool Verbose { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return
				"YoutubeDownloader (-v) [urls]\n";
		}
	}
}
