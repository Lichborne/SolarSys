//place this script in the Editor folder within Assets.
using UnityEditor;


//to be used on the command line:
//$ Unity -quit -batchmode -executeMethod WebGLBuilder.build

class WebGLBuilder {
    static void build() {
        string[] scenes = {"Assets/Scenes/SampleScene.unity"};
        BuildPipeline.BuildPlayer(scenes, "WebGL-Dist", BuildTarget.WebGL, BuildOptions.None);
    }
}
