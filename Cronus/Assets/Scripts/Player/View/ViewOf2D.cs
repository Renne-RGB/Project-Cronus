using System.Collections.Generic;
using System.Linq;
using Assets.Code;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ViewOf2D : MonoBehaviour
{
    private Collider2D selfCollider;
    public float radius = 20;
    public LayerMask blockLayerMask;

    public bool debug;
    public float detectionOffset = 0.02f;      //偏移量
    private ViewMeshCreater viewMeshCreater;

    public MeshFilter meshFilter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selfCollider = GetComponent<Collider2D>();
        viewMeshCreater = new ViewMeshCreater();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        List<Vector2> points = GetVisiblePointsOfColliders();
        List<HitInfo> hitInfos = points.ConvertAll(point => new HitInfo(point, false));

        Vector2[] rectPoint = new Vector2[] {
            (Vector2)transform.position + Vector2.up * radius,
            (Vector2)transform.position + Vector2.down * radius,
            (Vector2)transform.position + Vector2.left * radius,
            (Vector2)transform.position + Vector2.right * radius};

        foreach (Vector2 point in rectPoint)
        {
            if (IsVisiblePoint(point, false))
                points.Add(point);
        }

        //衝突している点を全部保存して度数順で並ぶ
        hitInfos = hitInfos.OrderBy(hitInfo => GetAngle360((hitInfo.basicPoint - (Vector2)transform.position).normalized, Vector2.right)).ToList();

        hitInfos.ForEach(hitInfo =>
        {
            CompleteHitInfo(hitInfo);
        });

        //メッシュを初期化
        viewMeshCreater.Clear();
        viewMeshCreater.SetCenter(transform.position);

        for (int i = 0; i < hitInfos.Count; i++)
        {
            int nextIndex = (i + 1) % hitInfos.Count;
            HitInfo hitInfo = hitInfos[i];
            HitInfo nextInfo = hitInfos[nextIndex];
            Vector2 point1 = GetHitRightFixedPoint(hitInfo);
            Vector2 point2 = GetHitLeftFixedPoint(nextInfo);
            viewMeshCreater.AddNewTriangle(point1 - (Vector2)transform.position, point2 - (Vector2)transform.position);
        }
        meshFilter.mesh = viewMeshCreater.GetMesh();
    }

    private List<Vector2> GetVisiblePointsOfColliders()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position, radius, blockLayerMask);
        List<Vector2> points = new List<Vector2>();

        foreach (Collider2D collider in colliders)
        {
            List<Vector2> pointsOfCollider = new List<Vector2>();
            if (collider is BoxCollider2D)
            {
                //boxColliderのローカル座標を保存する
                BoxCollider2D boxCollider = (BoxCollider2D)collider;
                pointsOfCollider.Add(new Vector2(-boxCollider.size.x, -boxCollider.size.y) * 0.5f);
                pointsOfCollider.Add(new Vector2(boxCollider.size.x, boxCollider.size.y) * 0.5f);
                pointsOfCollider.Add(new Vector2(-boxCollider.size.x, boxCollider.size.y) * 0.5f);
                pointsOfCollider.Add(new Vector2(boxCollider.size.x, -boxCollider.size.y) * 0.5f);
            }
            else if (collider is EdgeCollider2D)
            {
                //edgeColliderのローカル座標を保存する
                EdgeCollider2D edgeCollider = (EdgeCollider2D)collider;
                edgeCollider.GetPoints(pointsOfCollider);
            }
            else if (collider is PolygonCollider2D)
            {
                //polygonColliderのローカル座標を保存する
                PolygonCollider2D polygonCollider = (PolygonCollider2D)collider;
                pointsOfCollider.AddRange(polygonCollider.points);
            }

            Vector2 center = transform.position;
            foreach (Vector2 point in pointsOfCollider)
            {
                //ローカル座標をワールド座標に変換
                Vector2 worldPoint = collider.gameObject.transform.TransformPoint(point);
                if (IsVisiblePoint(worldPoint, true))
                    points.Add(worldPoint);
            }
        }
        return points;
    }

    private bool IsVisiblePoint(Vector2 point, bool offset)
    {
        Vector2 closePoint;
        if (offset)
        {
            //pointからの線にcollider自分に当たられないように 偏移量をつける
            Vector2 toCenterDirection = ((Vector2)transform.position - point).normalized;
            closePoint = point + toCenterDirection * detectionOffset;
        }
        else
        {
            closePoint = point;
        }
        //プレイヤーに向けて線を作る、プレイヤーに当たったら見えるポイントとする
        RaycastHit2D raycastHit = Physics2D.Linecast(closePoint, transform.position, blockLayerMask | (1 << gameObject.layer));
        return raycastHit && raycastHit.collider == selfCollider;
    }

    //時計回りの度数をとる
    private float GetAngle360(Vector2 dir1, Vector2 dir2)
    {
        float angle = Vector2.Angle(dir1, dir2);
        dir1 = Quaternion.AngleAxis(90, Vector3.forward) * dir1;

        if (Vector2.Dot(dir1, dir2) < 0)
            angle = 360 - angle;

        return angle;
    }

    //HitInfoの情報補完
    private void CompleteHitInfo(HitInfo hitInfo)
    {
        //中心から衝突点のベクトル
        hitInfo.toPointDirection = (hitInfo.basicPoint - (Vector2)transform.position).normalized;
        if (hitInfo.isAssistPoint)
            return;

        // RaycastHit2D pointRaycastHit = Physics2D.Raycast(transform.position, hitInfo.basicPoint, radius, blockLayerMask);

        // var hits = Physics.RaycastAll(transform.position, hitInfo.toPointDirection, radius, blockLayerMask);

        //RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, hitInfo.toPointDirection, radius, blockLayerMask);

        // if (hits.Length >= 2)
        // {
        //     Vector2 farPoint = hits[1].point;
        //     hitInfo.basicPoint = farPoint - (Vector2)transform.position;
        // }

        //そのベクトルの中心から垂直方向ベクトルを取る
        Vector2 perpendicular = Vector2.Perpendicular(hitInfo.toPointDirection);

        //衝突点偏移後の座標
        hitInfo.leftPoint = hitInfo.basicPoint + perpendicular * detectionOffset;
        hitInfo.rightPoint = hitInfo.basicPoint - perpendicular * detectionOffset;
        //偏移後のベクトル
        hitInfo.toLeftPointDirection = (hitInfo.leftPoint - (Vector2)transform.position).normalized;
        hitInfo.toRightPointDirection = (hitInfo.rightPoint - (Vector2)transform.position).normalized;

        RaycastHit2D leftPointRaycastHit = Physics2D.Raycast(transform.position, hitInfo.toLeftPointDirection, radius, blockLayerMask);
        RaycastHit2D rightPointRaycastHit = Physics2D.Raycast(transform.position, hitInfo.toRightPointDirection, radius, blockLayerMask);

        if (leftPointRaycastHit)
        {
            //ブロック面の法線
            hitInfo.leftHitNormal = leftPointRaycastHit.normal;
            hitInfo.leftHit = true;
            hitInfo.leftHitPoint = leftPointRaycastHit.point;
        }
        if (rightPointRaycastHit)
        {
            //ブロック面の法線
            hitInfo.rightHitNormal = rightPointRaycastHit.normal;
            hitInfo.rightHit = true;
            hitInfo.rightHitPoint = rightPointRaycastHit.point;
        }
        if (debug)
        {
            //Debug.DrawLine(transform.position, hitInfo.basicPoint, Color.yellow);
             Debug.DrawLine(transform.position, hitInfo.leftPoint, Color.yellow);

             Debug.DrawLine(transform.position, hitInfo.rightPoint, Color.red);
        }
    }

    //右のベクトル調整
    private Vector2 GetHitRightFixedPoint(HitInfo hitInfo)
    {
        Vector2 point = GetHitRightPoint(hitInfo);
        if (hitInfo.isAssistPoint)
            return point;
        float toPointDistance = Vector2.Distance(point, transform.position);
        Vector2 fixedPoint = (Vector2)transform.position + hitInfo.toPointDirection * toPointDistance;
        if (!hitInfo.rightHit)
            return fixedPoint;

        Vector2 point1Perpendicular = Vector2.Perpendicular(hitInfo.rightHitNormal);
        float pointA = Vector2.Angle(point1Perpendicular, -hitInfo.toPointDirection);
        if (pointA > 90)
            pointA = Vector2.Angle(-point1Perpendicular, -hitInfo.toPointDirection);
        if (pointA == 0)
            return fixedPoint;

        float pointa = PointToRayDistance(point, point1Perpendicular, fixedPoint);
        float point1c = pointa / Mathf.Sin(pointA * Mathf.Deg2Rad);

        if (Vector2.Dot((fixedPoint - point).normalized, hitInfo.rightHitNormal) < 0)
            return fixedPoint - hitInfo.toPointDirection * point1c;
        else
            return fixedPoint + hitInfo.toPointDirection * point1c;
    }

    //左のベクトル調整
    private Vector2 GetHitLeftFixedPoint(HitInfo hitInfo)
    {
        Vector2 point = GetHitLeftPoint(hitInfo);
        if (hitInfo.isAssistPoint)
            return point;
        float toPointDistance = Vector2.Distance(point, transform.position);
        Vector2 fixedPoint = (Vector2)transform.position + hitInfo.toPointDirection * toPointDistance;
        if (!hitInfo.leftHit)
            return fixedPoint;
        Vector2 point1Perpendicular = Vector2.Perpendicular(hitInfo.leftHitNormal);

        float pointA = Vector2.Angle(point1Perpendicular, -hitInfo.toPointDirection);
        if (pointA > 90)
            pointA = Vector2.Angle(-point1Perpendicular, -hitInfo.toPointDirection);
        if (pointA == 0)
        {
            return fixedPoint;
        }

        float pointa = PointToRayDistance(point, point1Perpendicular, fixedPoint);

        float point1c = pointa / Mathf.Sin(pointA * Mathf.Deg2Rad);

        if (Vector2.Dot((fixedPoint - point).normalized, hitInfo.leftHitNormal) < 0)
            return fixedPoint - hitInfo.toPointDirection * point1c;
        else
            return fixedPoint + hitInfo.toPointDirection * point1c;
    }

    private Vector2 GetHitRightPoint(HitInfo hitInfo)
    {
        if (hitInfo.isAssistPoint)
            return hitInfo.basicPoint;
        if (hitInfo.rightHit)
            return hitInfo.rightHitPoint;
        else
            //もしどこでも当たらなかったら その方向に半径までのベクトルを取る
            return (Vector2)transform.position + hitInfo.toRightPointDirection * radius;
    }
    private Vector2 GetHitLeftPoint(HitInfo hitInfo)
    {
        if (hitInfo.isAssistPoint)
            return hitInfo.basicPoint;
        if (hitInfo.leftHit)
            return hitInfo.leftHitPoint;
        else
            //もしどこでも当たらなかったら その方向に半径までのベクトルを取る
            return (Vector2)transform.position + hitInfo.toLeftPointDirection * radius;
    }

    private float PointToRayDistance(Vector3 start, Vector2 direction, Vector3 point)
    {
        Vector3 p1_target = point - start;
        return Mathf.Sin(Vector3.Angle(direction, p1_target) * Mathf.Deg2Rad) * p1_target.magnitude;
    }
}