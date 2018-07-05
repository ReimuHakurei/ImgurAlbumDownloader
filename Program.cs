using System;
using ImgurAPIClient;

namespace ImgurAlbumDownloader {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Imgur Album Downloader");
			Console.WriteLine("Copyright (c) 2018 Alex Ingram");
			Console.WriteLine("");

			if (args.Length == 0) {
				Console.WriteLine("Usage: ImgurAlbumDownloader.exe url1 [url2 url3 ...]");
				Environment.Exit(0);
			}

			ImgurClient Client = new ImgurClient("6b33eb78c24c603");

			foreach (string s in args) {
				Client.Download(s);
			}
		}
	}
}
