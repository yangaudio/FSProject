
namespace ThGold.Feel
{
    public class FeedBackCustomName
    {

        /// <summary>
        /// lets you load a destination scene using various methods, either native or using MMTools’ loading screens
        /// 允许您使用各种方法加载目标场景，无论是原生方法还是使用 MMTools 的加载屏幕
        /// </summary>
        public const string FeedBack_LoadScene = "FeedBack_LoadScene";
        /// <summary>
        /// 卸载场景
        /// </summary>
        public const string FeedBack_UnLoadScene = "FeedBack_UnLoadScene";
        /// <summary>
        /// 让您在播放反馈时放大或缩小，无论是在指定的持续时间内，还是直到另行通知。
        /// 如果您使用的是普通相机，则需要一个 MMCameraZoom 组件。如果您使用的是 Cinemachine 虚拟摄像机，
        /// 则需要一个 MMCinemachineZoom 组件。此反馈还允许您使用相对值，添加到反馈播放时的当前缩放。
        ///  lets you zoom in or out when the feedback plays, either for a specified duration, or until further notice. 
        ///  If you’re using a regular camera, you will need a MMCameraZoom component on it.
        ///  If you’re using a Cinemachine virtual camera, you’ll need a MMCinemachineZoom component on it.
        /// </summary>
        public const string FeedBack_ChineZoom = "FeedBack_ChineZoom";
        /// <summary>
        /// flash an image on screen, or simply a color for a short duration. 
        /// You’ll need an element (or more) with a MMFlash component on it in your scene for this feedback to interact with. 
        /// </summary>
        public const string FeedBack_Bloom = "FeedBack_Bloom";
        /// <summary>
        /// ades an image in or out, useful for transitions. 
        /// This feedback requires an object with an MMFader component on it in your scene. 
        /// </summary>
        public const string FeedBack_Fade = "FeedBack_Fade";
        /// <summary>
        /// 改变景深
        /// control a camera’s field of view over time. Will require a MMCameraFieldOfViewShaker on your camera, 
        /// or a MMCinemachineFieldOfViewShaker on your virtual camera.
        /// </summary>
        public const string FeedBack_FieldOfView = "FeedBack_FieldOfView";
        /// <summary>
        /// lets you transition to another virtual camera, using the blend of your choice, and auto managing other camera’s priorities.
        /// You’ll need a MMCinemachinePriorityListener on each of your virtual cameras for this to work. 
        /// If you want more control, you can also add a MMCinemachinePriorityBrainListener on your Cinemachine brain. 
        /// This will let you specify the transition to use to override the default one straight from the feedback.
        /// </summary>
        public const string FeedBack_CineTransition = "FeedBack_CineTransition";
        /// <summary>
        /// 色差
        /// 随着时间的推移控制色差的力量。您的后期处理体积需要一个 MMChromaticAberrationShaker
        /// You’ll need a MMChromaticAberrationShaker on your post processing volume
        /// </summary>
        public const string FeedBack_ChromaticAberration = "FeedBack_ChromaticAberration";
        /// <summary>
        /// 景深
        /// lets you control depth of field focus distance, aperture and focal length over time. You’ll need a MMDepthOfFieldShaker on your post processing volume
        /// </summary>
        public const string FeedBack_DepthOfField = "FeedBack_DepthOfField";
        /// <summary>
        /// 镜头畸变
        /// lens distortion on demand. You’ll need a MMLensDistortionShaker on your post processing volume.
        /// </summary>
        public const string FeedBack_LensDistortion = "FeedBack_LensDistortion";
        /// <summary>
        /// 随着时间的推移控制晕影参数。您的后期处理卷上需要一个 MMVignetteShaker
        /// </summary>
        public const string FeedBack_Vignette = "FeedBack_Vignette";
        /// <summary>
        /// 生成粒子对象
        /// </summary>
        public const string FeedBack_PlayParticles = "FeedBack_PlayParticles";
    }
}
