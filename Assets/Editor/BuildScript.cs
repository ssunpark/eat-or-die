using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
	public static void PerformBuild()
	{
		string[] scenes = { "Assets/01.Scenes/PrototypeDemoScene.unity" };
		string buildPath = "Build/eatOrDie.exe";

		Debug.Log("🛠 Starting build process...");
		Debug.Log($"📁 Target path: {buildPath}");

		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = buildPath,
			target = BuildTarget.StandaloneWindows64,
			options = BuildOptions.None
		};

		BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
		BuildSummary summary = report.summary;

		Debug.Log($"📦 Build Result: {summary.result}");
		Debug.Log($"📦 Output Path: {summary.outputPath}");
		Debug.Log($"📦 Total Warnings: {summary.totalWarnings}, Errors: {summary.totalErrors}");

		foreach (var step in report.steps)
		{
			Debug.Log($"🧩 Step: {step.name} - Duration: {step.duration.TotalSeconds}s");
		}

		if (summary.result == BuildResult.Succeeded)
		{
			Debug.Log($"✅ Build completed successfully. Size: {summary.totalSize} bytes");
		}
		else
		{
			Debug.LogError($"❌ Build failed with result: {summary.result}");
			EditorApplication.Exit(1);
		}
	}
}
