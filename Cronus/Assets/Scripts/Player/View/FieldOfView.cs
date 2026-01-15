using System;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Settings")]
    public int rayCount = 50;
    public float viewDistance = 50f;
    public float fov = 90f;
    [SerializeField] private LayerMask layerMask;

    private Mesh mesh;
    private float startingAngle;
    private Vector3 origin = Vector3.zero; // ローカル原点 (Local Origin)
    private MeshFilter meshFilter;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    private void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // 描画順序を調整する
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 1;
        }

        //配列の初期化
        vertices = new Vector3[rayCount + 1 + 1];
        uv = new Vector2[vertices.Length];
        triangles = new int[rayCount * 3];
    }

    private void LateUpdate()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        //レイキャストの発射位置（ワールド座標）
        Vector3 raycastOrigin = transform.position;

        //メッシュの原点（ローカル座標：通常は0,0,0）
        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i < rayCount; i++)
        {
            Vector3 vertex;

            //角度からベクトルを取得
            Vector3 dir = GetVectorFromAngle(angle);

            RaycastHit2D raycastHit2D = Physics2D.Raycast(raycastOrigin, dir, viewDistance, layerMask);

            if (raycastHit2D.collider == null)
            {
                //衝突なし：最大距離まで伸ばす
                vertex = origin + dir * viewDistance;
            }
            else
            {
                //衝突あり：衝突点を使用
                //ワールド座標の衝突点 (hit.point) を ローカル座標に変換する
                vertex = raycastHit2D.point - (Vector2)raycastOrigin;
            }

            vertices[vertexIndex] = vertex;

            // 三角形のインデックスを構築
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;             // 原点
                triangles[triangleIndex + 1] = vertexIndex - 1; // 前の頂点
                triangles[triangleIndex + 2] = vertexIndex;     // 現在の頂点

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.bounds = new Bounds(origin, Vector3.one * viewDistance * 2f);
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        //角度(0-360)をラジアンに変換してベクトルを生成
        float angleRad = angle * (MathF.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        //向きに合わせて開始角度を調整
        float baseAngle = GetAngleFromVectorFloat(aimDirection);
        startingAngle = baseAngle + fov / 2f;
    }

    public void SetFov(float fov)
    {
        this.fov = fov;
    }

    public void SetViewDistance(float distance)
    {
        this.viewDistance = distance;
    }
}