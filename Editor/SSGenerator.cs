namespace takashicompany.Unity.Editor.ScreenShot
{
	// ストア向けのスクリーンショットの撮影を自動で行い、リサイズを半自動で行うエディター拡張
	// 詳細な使い方はこちら
	// https://github.com/takashicompany/ss-generator/blob/master/README.md

	using UnityEngine;
	using UnityEditor;
	using System.IO;

	[InitializeOnLoad]
	public static class SSGenerator
	{
		private enum Device
		{
			iPad,
			iPhone,
			iPhoneX,
		}

		private enum ResizeOption
		{
			Portrait,
			Landscape,
		}

#region サイズ関係

		private static Vector2Int GetPortraitPixel(this Device self)
		{
			switch(self)
			{
				case Device.iPad: return new Vector2Int(2048, 2732);
				case Device.iPhone: return new Vector2Int(1242, 2208);
				case Device.iPhoneX: return new Vector2Int(1242, 2688);
			}

			return new Vector2Int(1, 1);
		}

		private static Vector2Int GetPortraitPixelForResize(this Device self)
		{
			var pixel = GetPortraitPixel(self);
			return new Vector2Int(-1, pixel.y);
		}
#endregion

#region 定数
		private const float _captureInterval = 1f;	// TODO 個々を動的に変えられるようにしたい
#endregion

#region 変数
		private static string _capturePath;
		private static int _counter;
		private static float _remainNextCapture = 0;
#endregion

		static SSGenerator()
		{
			EditorApplication.update += OnEditorApplicationUpdate;
		}

		private static int GetDevices()
		{
			return System.Enum.GetNames(typeof(Device)).Length;
		}

#region キャプチャ

		private static bool IsCapturing()
		{
			return !string.IsNullOrEmpty(_capturePath) && Application.isPlaying;
		}

		/// <summary>
		/// 撮影時はアスペクト比 横3:縦4を推奨している
		/// </summary>
		/// <returns></returns>
		private static bool IsValidAspectRatio()
		{
			return (float)Screen.width / (float)Screen.height > 0.7f;
		}

		private static void OnEditorApplicationUpdate()
		{
			if (IsCapturing())
			{
				_remainNextCapture -= Time.unscaledDeltaTime;

				if (_remainNextCapture <= 0)
				{
					_remainNextCapture = _captureInterval;
					Capture();
				}
			}
		}

		[MenuItem("スクショ自動撮影/撮影を開始")]
		public static void StartCapture()
		{
			if (!Application.isPlaying)
			{
				NotificationAll("ERROR\n再生中に実行してください。");
				return;
			}

			if (!IsValidAspectRatio())
			{
				var r = (float)Screen.height / 4f;
				var w = (float)Screen.width / r;
				NotificationGameView("WARNING\n画面の比率は3:4に近いと良いです。\n現在は (" + w.ToString("F1") + " : 4) です。");
			}
			else
			{
				NotificationGameView("撮影を開始します");
			}

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

		private static void Capture()
		{
			_counter++;
			ScreenCapture.CaptureScreenshot(GetCapturePath() + "/" + string.Format("{0:00000000}.png", _counter));
		}
#endregion

#region リサイズのタスク
		[MenuItem("スクショ自動撮影/フォルダ内の画像をストア用にリサイズ")]
		private static void Resize()
		{
			var directoryPath = EditorUtility.OpenFolderPanel("自動でリサイズしたい画像が入ったフォルダを選択してください。", "", "");

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

			var max = System.Enum.GetNames(typeof(Device)).Length * filePathes.Length;

			foreach (var filePath in filePathes)
			{
				if (!filePath.EndsWith(".png"))
				{
					continue;
				}

				if (!ResizeProcess(directoryPath, filePath, index, max))
				{
					Debug.LogError("リサイズを中断しました。");
					return;
				}

				index++;
			}

			 EditorUtility.ClearProgressBar();

			 var okList = new string[]	// 特に意味は無いです。
			 {
				"ありがとう！",
				"おつかれ！",
				"やるじゃん！",
				"ご苦労！",
				"いいね！",
				"よくやった！",
				"上出来だ！",
				"ええやん！",
				"さすが！",
			 };

			 EditorUtility.DisplayDialog("ストア用にリサイズ", "リサイズが完了しました！", okList[Random.Range(0, okList.Length)]);
		}

		private static bool ResizeProcess(string directoryPath, string filePath, int index, int max)
		{
			var count = 0;

			var src = new Texture2D(1, 1, TextureFormat.RGB24, false);
			src.LoadImage(File.ReadAllBytes(filePath));

			foreach (Device device in System.Enum.GetValues(typeof(Device)))
			{
				if (DisplayResizeProgress(index * GetDevices() + count, max))
				{
					return false;
				}

				var texture = src.Resize(device).Trim(device);
				SaveResized(texture, directoryPath, index, device);

				count++;
			}

			// var iPadTexture = src.Resize(Device.iPad).Trim(Device.iPad);
			// SaveResized(iPadTexture, directoryPath, index, Device.iPad);

			// var iPhoneTexture = src.Resize(Device.iPhone).Trim(Device.iPhone);
			// SaveResized(iPhoneTexture, directoryPath, index, Device.iPhone);
			
			// var iPhoneXTexture = src.Resize(Device.iPhoneX).Trim(Device.iPhoneX);
			// SaveResized(iPhoneXTexture, directoryPath, index, Device.iPhoneX);

			return true;
		}

		private static bool DisplayResizeProgress(int current, int max)
		{
			if (EditorUtility.DisplayCancelableProgressBar("リサイズ中...", current + "/" + max, (float)current / (float)max))
			{
				return true;
			}

			return false;
		}
#endregion

#region リサイズの処理
		private static Texture2D Resize(this Texture2D src, Device device)
		{
			var size = device.GetPortraitPixelForResize();
			return ResizeInternal(src, size.x, size.y);
		}

		/// <summary>
		/// 画像をリサイズして新しいインスタンスを返す
		/// </summary>
		/// <param name="src">元の画像</param>
		/// <param name="width">幅。負数を指定すると高さを基準にアスペクト比を維持したサイズが指定される</param>
		/// <param name="height">高さ。負数を指定すると幅を基準にアスペクト比を維持したサイズが指定される</param>
		/// <returns></returns>
		private static Texture2D ResizeInternal(this Texture2D src, int width, int height)
		// Texture2DクラスにResize(bool)があるから関数名を変えてる
		{
			if (width < 0 && height < 0)
			{
				Debug.LogError("widthもheightも負数が指定されてます");
				return null;
			}
			else if (width < 0)
			{
				var ratio = (float)height / src.height;
				width = (int)(src.width * ratio);
			}
			else if (height < 0)
			{
				var ratio = (float)width / src.width;
				height = (int)(src.width * ratio);
			}

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

		private static Texture2D Trim(this Texture2D src, Device ssType)
		{
			var size = ssType.GetPortraitPixel();
			return Trim(src, size.x, size.y);
		}

		private static Texture2D Trim(this Texture2D src, int width, int height)
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
#endregion

#region 加工した後の保存処理
		
		private static void SaveResized(Texture2D texture, string directoryPath, int index, Device device)
		{
			var png = texture.EncodeToPNG();
			File.WriteAllBytes(GetResizedPath(directoryPath, device) + "/" + index + ".png", png);
		}

#endregion

#region パス
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

		private static string GetResizedPath(string directoryPath, Device device)
		{
			return directoryPath + "/" + device;
		}
#endregion

#region オーバーレイメッセージ
		// https://kan-kikuchi.hatenablog.com/entry/ShowNotificationを参考にした

		private static void NotificationGameView(string message)
		{
			var assembly = typeof(UnityEditor.EditorWindow).Assembly;
			var type = assembly.GetType("UnityEditor.GameView");
			EditorWindow.GetWindow(type).ShowNotification(new GUIContent(message));
		}

		private static void NotificationSceneView(string message)
		{
			var assembly = typeof(UnityEditor.EditorWindow).Assembly;
			var type = assembly.GetType("UnityEditor.SceneView");
			EditorWindow.GetWindow(type).ShowNotification(new GUIContent(message));
		}

		private static void NotificationConsoleWindow(string message)
		{
			var assembly = typeof(UnityEditor.EditorWindow).Assembly;
			var type = assembly.GetType("UnityEditor.ConsoleWindow");
			EditorWindow.GetWindow(type).ShowNotification(new GUIContent(message));
		}

		private static void NotificationAll(string message)
		{
			NotificationGameView(message);
			NotificationSceneView(message);
			NotificationConsoleWindow(message);
		}
#endregion

	}
}
