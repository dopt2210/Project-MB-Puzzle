using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class FogOfWarMask : MonoBehaviour
{
    public RenderTexture fogMaskTexture;
    public int textureSize = 512;
    public float revealSize = 30f;
    [SerializeField] private Vector3 worldOrigin;
    [SerializeField] private Vector2 worldSize;

    [Header("Shader Material")]
    [SerializeField] private Material revealMaterial;
    private void Start()
    {
        if (fogMaskTexture == null)
        {
            fogMaskTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
            fogMaskTexture.Create();
        }

        RenderTexture.active = fogMaskTexture;
        GL.Clear(true, true, new Color(0, 0, 0, 1));
        RenderTexture.active = null;
    }
    public void SetUpSize(MazeSO mazeSO, float cellSize)
    {
        worldSize = new Vector2(mazeSO.Width, mazeSO.Depth) * cellSize;
    }
    public void Reveal(Vector3 worldPos)
    {
        // Chuyển worldPos sang UV (0~1)
        float u = Mathf.InverseLerp(worldOrigin.x, worldOrigin.x + worldSize.x, worldPos.x);
        float v = Mathf.InverseLerp(worldOrigin.z, worldOrigin.z + worldSize.y, worldPos.z);

        revealMaterial.SetVector("_RevealPos", new Vector4(u, v, 0, 0));
        revealMaterial.SetFloat("_Radius", revealSize / worldSize.x); // scale theo tỉ lệ UV

        RenderTexture temp = RenderTexture.GetTemporary(fogMaskTexture.width, fogMaskTexture.height, 0, fogMaskTexture.format);
        Graphics.Blit(fogMaskTexture, temp); // copy hiện tại
        Graphics.Blit(temp, fogMaskTexture, revealMaterial); // vẽ lên với reveal
        RenderTexture.ReleaseTemporary(temp);

    }
    public void Reveal(List<Cell> cells)
    {
        RenderTexture temp = RenderTexture.GetTemporary(fogMaskTexture.width, fogMaskTexture.height, 0, fogMaskTexture.format);
        Graphics.Blit(fogMaskTexture, temp); // copy hiện tại

        foreach (var cell in cells)
        {
            Vector3 centerPosition = cell.transform.Find("SpawnPoint").position;
            Vector3 worldPos = centerPosition; // bạn cần đảm bảo có thông tin này
            float u = Mathf.InverseLerp(worldOrigin.x, worldOrigin.x + worldSize.x, worldPos.x);
            float v = Mathf.InverseLerp(worldOrigin.z, worldOrigin.z + worldSize.y, worldPos.z);

            revealMaterial.SetVector("_RevealPos", new Vector4(u, v, 0, 0));
            revealMaterial.SetFloat("_Radius", revealSize / worldSize.x); // dùng 1 cell = bán kính sáng

            Graphics.Blit(temp, fogMaskTexture, revealMaterial); // cộng dồn ánh sáng
            Graphics.Blit(fogMaskTexture, temp); // tiếp tục vẽ chồng lên
        }

        RenderTexture.ReleaseTemporary(temp);
    }

    public void ResetFog()
    {
        RenderTexture.active = fogMaskTexture;
        GL.Clear(true, true, new Color(0, 0, 0, 1));
        RenderTexture.active = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Vẽ vùng worldSize từ worldOrigin (màu xanh dương)
        Gizmos.color = Color.cyan;
        Vector3 bottomLeft = new Vector3(worldOrigin.x, transform.position.y, worldOrigin.z);
        Vector3 topRight = bottomLeft + new Vector3(worldSize.x, 0, worldSize.y);
        Vector3 topLeft = new Vector3(bottomLeft.x, bottomLeft.y, topRight.z);
        Vector3 bottomRight = new Vector3(topRight.x, topRight.y, bottomLeft.z);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // Vẽ điểm origin (màu đỏ)
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(bottomLeft, 0.2f);

        // Nếu muốn test vẽ 1 worldPos demo (màu vàng)
        Vector3 sampleWorldPos = bottomLeft + new Vector3(worldSize.x * 0.5f, 0, worldSize.y * 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(sampleWorldPos, 0.15f);
    }
#endif

}
