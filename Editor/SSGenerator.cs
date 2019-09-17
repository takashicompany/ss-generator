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
			var path = EditorUtility.OpenFolderPanel("Select screenshot folder", "", "");

			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			var files =  Directory.GetFiles(path);

			foreach (var file in files)
			{
				if (!file.EndsWith(".png"))
				{
					continue;
				}

				Resize(file);
				Debug.Log("hozon");
				break;
			}
		}

		private static Texture2D Resize(string path)
		{
			var src = new Texture2D(1, 1, TextureFormat.RGB24, false);
			src.LoadImage(File.ReadAllBytes(path));

			var dest = new Texture2D(2048,2732, TextureFormat.RGB24, false);

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

			var png = dest.EncodeToPNG();
			File.WriteAllBytes(path + "_" + "trim.png", png);
			return src;
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


	}
}