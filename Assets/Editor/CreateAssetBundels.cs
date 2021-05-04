using UnityEditor;

public class CreateAssetBundels  
{
    [MenuItem("Assets/Build AssetBundles for Android")]
    static void BuildAllAssetBundlesForAndroid()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    [MenuItem("Assets/Build AssetBundles for iOS")]
    static void BuildAllAssetBundlesForiOS()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.iOS);
    }

}
