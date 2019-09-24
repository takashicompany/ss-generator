namespace TakashiCompany.Unity.Editor.ScreenShot
{
	// ① フォルダを生成する
	// ② スクリーンショットを自動で撮る
	// ③ フォルダを指定する
	// ④ 画像を選択できる
	// ⑤ 指定したフォルダの下層にサイズ別でスクリーンショットを置ける場所を作る
	// ⑥ リサイズする

	using UnityEngine;
	using UnityEditor;
	using System.IO;

	[InitializeOnLoad]
	public static class SSGenerator
	{
		public enum Device
		{
			iPad,
			iPhone,
			iPhoneX,
		}


		public static Vector2Int GetPixel(this Device self)
		{
			switch(self)
			{
				case Device.iPad: return new Vector2Int(2048, 2732);
				case Device.iPhone: return new Vector2Int(1242, 2208);
				case Device.iPhoneX: return new Vector2Int(1242, 2688);
			}

			return new Vector2Int(1, 1);
		}

		private static string _capturePath;
		private static int _counter;

		static SSGenerator()
		{
			EditorApplication.update += OnEditorApplicationUpdate;
		}

		private static void OnEditorApplicationUpdate()
		{
			if (IsCapturing())
			{
				Capture();
			}
		}

		[MenuItem("SSGenerator/Start")]
		public static void StartCapture()
		{
			if (!Directory.Exists(GetRootPath()))
			{
				Directory.CreateDirectory(GetRootPath());
			}

			var now = System.DateTime.Now;
			_capturePath = now.ToString("yyyyMMdd_HHmm_ss");

			if (!Directory.Exists(GetCapturePath()))
			{
				Directory.CreateDirectory(GetCapturePath());
			}
		}

		[MenuItem("SSGenerator/Resize")]
		private static void Resize()
		{
			var directoryPath = EditorUtility.OpenFolderPanel("Select screenshot folder", "", "");

			if (string.IsNullOrEmpty(directoryPath))
			{
				return;
			}

			var iPadPath = GetResizedPath(directoryPath, Device.iPad);

			if (!Directory.Exists(iPadPath))
			{
				Directory.CreateDirectory(iPadPath);
			}

			var iPhonePath = GetResizedPath(directoryPath, Device.iPhone);

			if (!Directory.Exists(iPhonePath))
			{
				Directory.CreateDirectory(iPhonePath);
			}

			var iPhoneXPath = GetResizedPath(directoryPath, Device.iPhoneX);

			if (!Directory.Exists(iPhoneXPath))
			{
				Directory.CreateDirectory(iPhoneXPath);
			}

			var filePathes =  Directory.GetFiles(directoryPath);

			int index = 0;

			foreach (var filePath in filePathes)
			{
				if (!filePath.EndsWith(".png"))
				{
					continue;
				}

				ResizeProcess(directoryPath, filePath, index);

				index++;
			}
		}

		private static void ResizeProcess(string directoryPath, string filePath, int index)
		{
			var src = new Texture2D(1, 1, TextureFormat.RGB24, false);
			src.LoadImage(File.ReadAllBytes(filePath));
			
			var iPadTexture = Resize(src, Device.iPad);
			var iPhoneTexture = Trim(iPadTexture, Device.iPhone);
			var iPhoneXTexture = Trim(iPadTexture, Device.iPhone);

			SaveResized(iPadTexture, directoryPath, index, Device.iPad);
			SaveResized(iPhoneTexture, directoryPath, index, Device.iPhone);
			SaveResized(iPhoneXTexture, directoryPath, index, Device.iPhoneX);
		}

		private static void SaveResized(Texture2D texture, string directoryPath, int index, Device device)
		{
			var png = texture.EncodeToPNG();
			File.WriteAllBytes(GetResizedPath(directoryPath, device) + "/" + index + ".png", png);

		}

		private static Texture2D Resize(Texture2D src, Device device, bool verticalBased)
		{
			var size = device.GetPixel();
			return Resize(src, size.x, size.y, verticalBased);
		}

		private static Texture2D Resize(Texture2D src, int width, int height, bool varticalBased)
		{
			var dest = new Texture2D(width, height, TextureFormat.RGB24, false);

			var pixels = new Color[dest.width * dest.height];

			for (var y = 0; y < dest.height; y++)
			{
				for (var x = 0; x < dest.width; x++)
				{
					var normalizedX = (float)x / (float)(dest.width - 1);
					var normalizedY = (float)y / (float)(dest.height - 1);
					pixels[y * dest.width + x] = src.GetPixelBilinear(normalizedX, normalizedY);
				}
			}

			dest.SetPixels(pixels);
			dest.Apply();
 
			return dest;
		}

		private static Texture2D Trim(Texture2D src, Device ssType)
		{
			var size = ssType.GetPixel();
			return Trim(src, size.x, size.y);
		}

		private static Texture2D Trim(Texture2D src, int width, int height)
		{
			var offsetX = (src.width - width) / 2;
			var offsetY = (src.height - height) / 2;

			var dest = new Texture2D(width, height, TextureFormat.RGB24, false);

			var pixels = new Color[dest.width * dest.height];

			for (var y = 0; y < dest.height; y++)
			{
				for (var x = 0; x < dest.width; x++)
				{
					pixels[y * dest.width + x] = src.GetPixel(x + offsetX, y + offsetY);
				}
			}

			dest.SetPixels(pixels);
			dest.Apply();

			return dest;
		}

		private static void Capture()
		{
			_counter++;
			ScreenCapture.CaptureScreenshot(GetCapturePath() + "/" + string.Format("{0:00000000}.png", _counter));
		}

		private static string GetProjectPath()
		{
			return Application.dataPath + "/..";
		}

		private static string GetRootPath()
		{
			return GetProjectPath() + "/ss";
		}

		private static string GetCapturePath()
		{
			return GetRootPath() + "/" + _capturePath;
		}

		private static bool IsCapturing()
		{
			return !string.IsNullOrEmpty(_capturePath) && Application.isPlaying;
		}

		private static string GetResizedPath(string directoryPath, Device device)
		{
			return directoryPath + "/" + device;
		}


	}
}