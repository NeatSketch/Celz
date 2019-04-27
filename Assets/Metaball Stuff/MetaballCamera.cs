using UnityEngine;
using UnityEngine.UI;

public class MetaballCamera : MonoBehaviour
{
    public RawImage metaballOutputRawImage;

    private new Camera camera;

    private RenderTexture renderTexture;

    private int width = -1;
    private int height = -1;

    private void Start()
    {
        camera = this.GetComponent<Camera>();
    }

    private void Update()
    {
        if (Screen.width != width || Screen.height != height)
        {
            width = Screen.width;
            height = Screen.height;

            camera.targetTexture = null;
            metaballOutputRawImage.texture = null;

            if (renderTexture != null)
            {
                Destroy(renderTexture);
            }

            renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.R8);

            camera.targetTexture = renderTexture;
            metaballOutputRawImage.texture = renderTexture;
        }
    }
}
