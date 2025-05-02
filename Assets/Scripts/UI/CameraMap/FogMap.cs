using UnityEngine;

public class FogMap : MonoBehaviour
{
    public Texture2D fogMask;
    public int maskWidth = 256;
    public int maskHeight = 256;

    void Start()
    {
        fogMask = new Texture2D(maskWidth, maskHeight, TextureFormat.RGBA32, false);
        ClearMask(); 
    }

    void ClearMask()
    {
        Color[] blackPixels = new Color[maskWidth * maskHeight];
        for (int i = 0; i < blackPixels.Length; i++) blackPixels[i] = Color.black;
        fogMask.SetPixels(blackPixels);
        fogMask.Apply();
    }
    public void RevealFogAt(int cellX, int cellZ)
    {
        int texX = Mathf.Clamp(cellX, 0, fogMask.width - 1);
        int texY = Mathf.Clamp(cellZ, 0, fogMask.height - 1);

        fogMask.SetPixel(texX, texY, Color.white);
        fogMask.Apply();
    }
}
