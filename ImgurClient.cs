// Imgur client class.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImgurAPIClient {
	public class ImgurClient {
		private string ClientId;
		private HttpClient Client;

		public ImgurClient(string ClientId) {
			this.ClientId = ClientId;
			this.Client = new HttpClient();
			Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", ClientId);
		}

		public void Download(string AlbumUrl) {
			DownloadAsync(AlbumUrl).GetAwaiter().GetResult();
		}

		public async Task DownloadAsync(string AlbumUrl) {
			// Extract album hash from URL.
			string[] AlbumUrlSplit = AlbumUrl.Split('/');
			string AlbumId = AlbumUrlSplit[AlbumUrlSplit.Length - 1];

			HttpResponseMessage AlbumJsonRequest = await Client.GetAsync(String.Concat("https://api.imgur.com/3/album/", AlbumId));

			if (!AlbumJsonRequest.IsSuccessStatusCode) {
				Console.WriteLine("HTTP error.");
				Environment.Exit(1);
				//throw new Exception();
			}

			string AlbumJson = await AlbumJsonRequest.Content.ReadAsStringAsync();
			dynamic AlbumData = JObject.Parse(AlbumJson);

			string DirectoryName;
			try {
				DirectoryName = AlbumData.data.title;
				foreach (char c in System.IO.Path.GetInvalidFileNameChars()) {
					DirectoryName = DirectoryName.Replace(c, '_');
				}
			} catch {
				DirectoryName = AlbumId;
			}

			Directory.CreateDirectory(DirectoryName);
			Directory.SetCurrentDirectory(DirectoryName);

			Console.WriteLine("Album title: " + AlbumData.data.title);
			Console.WriteLine("Downloading...");

			int i = 0;
			foreach(dynamic d in AlbumData.data.images) {
				i++;

				String ImageUrl = d.link;

				Console.WriteLine("Downloading " + ImageUrl);

				HttpResponseMessage ImageDownload = await Client.GetAsync(ImageUrl);

				if (!AlbumJsonRequest.IsSuccessStatusCode) {
					Console.WriteLine("HTTP error.");
					Environment.Exit(1);
					//throw new Exception();
				}

				Stream ImageData = await ImageDownload.Content.ReadAsStreamAsync();

				string[] ImageUrlSplit = ImageUrl.Split('.');
				string ImageExtension = ImageUrlSplit[ImageUrlSplit.Length - 1];

				StreamWriter WriteImage = new StreamWriter(String.Format("{0:000}.{1}", i, ImageExtension));
				ImageData.CopyTo(WriteImage.BaseStream);
			}

			Directory.SetCurrentDirectory("..");
		}
	}
}