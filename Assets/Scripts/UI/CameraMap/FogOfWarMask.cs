using System.Collections.Generic;
using UnityEngine;

public class FogOfWarMask : MonoBehaviour
{
    [SerializeField] private RenderTexture _fogMaskTexture;
    [SerializeField] private float _revealSize = 10f;
    private int _textureSize = 512;
    private Vector3 _worldOrigin = Vector3.zero;
    private Vector2 _worldSize;

    [Header("Shader Material")]
    [SerializeField] private Material revealMaterial;
    private void Start()
    {
        if (_fogMaskTexture == null)
        {
            _fogMaskTexture = new RenderTexture(_textureSize, _textureSize, 0, RenderTextureFormat.ARGB32);
            _fogMaskTexture.Create();
        }

        RenderTexture.active = _fogMaskTexture;
        GL.Clear(true, true, new Color(0, 0, 0, 1));
        RenderTexture.active = null;
    }
    public void SetUpSize(MazeSO mazeSO, float cellSize)
    {
        _worldSize = new Vector2(mazeSO.Width, mazeSO.Depth) * cellSize;
    }
    public void Reveal(Vector3 worldPos)
    {
        // Chuyển worldPos sang UV (0~1)
        float u = Mathf.InverseLerp(_worldOrigin.x, _worldOrigin.x + _worldSize.x, worldPos.x);
        float v = Mathf.InverseLerp(_worldOrigin.z, _worldOrigin.z + _worldSize.y, worldPos.z);

        revealMaterial.SetVector("_RevealPos", new Vector4(u, v, 0, 0));
        revealMaterial.SetFloat("_Radius", _revealSize / _worldSize.x); // scale theo tỉ lệ UV

        RenderTexture temp = RenderTexture.GetTemporary(_fogMaskTexture.width, _fogMaskTexture.height, 0, _fogMaskTexture.format);
        Graphics.Blit(_fogMaskTexture, temp); // copy hiện tại
        Graphics.Blit(temp, _fogMaskTexture, revealMaterial); // vẽ lên với reveal
        RenderTexture.ReleaseTemporary(temp);

    }
    public void Reveal(List<Cell> cells)
    {
        RenderTexture temp = RenderTexture.GetTemporary(_fogMaskTexture.width, _fogMaskTexture.height, 0, _fogMaskTexture.format);
        Graphics.Blit(_fogMaskTexture, temp); // copy hiện tại

        foreach (var cell in cells)
        {
            Vector3 centerPosition = cell.transform.Find("SpawnPoint").position;
            Vector3 worldPos = centerPosition; // bạn cần đảm bảo có thông tin này
            float u = Mathf.InverseLerp(_worldOrigin.x, _worldOrigin.x + _worldSize.x, worldPos.x);
            float v = Mathf.InverseLerp(_worldOrigin.z, _worldOrigin.z + _worldSize.y, worldPos.z);

            revealMaterial.SetVector("_RevealPos", new Vector4(u, v, 0, 0));
            revealMaterial.SetFloat("_Radius", _revealSize / _worldSize.x); // dùng 1 cell = bán kính sáng

            Graphics.Blit(temp, _fogMaskTexture, revealMaterial); // cộng dồn ánh sáng
            Graphics.Blit(_fogMaskTexture, temp); // tiếp tục vẽ chồng lên
        }

        RenderTexture.ReleaseTemporary(temp);
    }
    public void ResetFog()
    {
        RenderTexture.active = _fogMaskTexture;
        GL.Clear(true, true, new Color(0, 0, 0, 1));
        RenderTexture.active = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Vẽ vùng _worldSize từ _worldOrigin (màu xanh dương)
        Gizmos.color = Color.cyan;
        Vector3 bottomLeft = new Vector3(_worldOrigin.x, transform.position.y, _worldOrigin.z);
        Vector3 topRight = bottomLeft + new Vector3(_worldSize.x, 0, _worldSize.y);
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
        Vector3 sampleWorldPos = bottomLeft + new Vector3(_worldSize.x * 0.5f, 0, _worldSize.y * 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(sampleWorldPos, 0.15f);
    }
#endif

}
