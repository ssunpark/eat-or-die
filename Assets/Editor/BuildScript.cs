using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
	public static void PerformBuild()
	{
		string[] scenes = { "Assets/01.Scenes/PopupTestScene.unity" };
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = "Build/eatOrDie.exe",
			target = BuildTarget.StandaloneWindows64,
			options = BuildOptions.None
		};

		BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
		BuildSummary summary = report.summary;

		if (summary.result == BuildResult.Succeeded)
		{
			Debug.Log($"✅ Build completed successfully. {summary.totalSize} bytes");
		}
		else
		{
			Debug.LogError($"❌ Build failed with result: {summary.result}");
			EditorApplication.Exit(1);
		}
	}
}
