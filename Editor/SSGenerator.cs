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

	public static class SSGenerator
	{
		private static string _capturePath;

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


	}
}