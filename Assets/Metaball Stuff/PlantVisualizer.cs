using UnityEngine;
using UnityEngine.UI;

public class PlantVisualizer : MonoBehaviour
{
    public Vector2 distortionOffsetRate;

    private RawImage rawImage;
    private Vector2 distortionOffset;

    private void Awake()
    {
        rawImage = this.GetComponent<RawImage>();
    }
    
    private void Update()
    {
        distortionOffset = new Vector2
        (
            (distortionOffset.x + Time.deltaTime * distortionOffsetRate.x) % 1f,
            (distortionOffset.y + Time.deltaTime * distortionOffsetRate.y) % 1f
        );
        rawImage.material.SetTextureOffset("_DistortionTex", distortionOffset);
    }
}
