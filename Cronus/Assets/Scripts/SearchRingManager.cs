using UnityEngine;

public class SearchRingManager : MonoBehaviour
{
    public static SearchRingManager Instance { get; private set; }

    [Header("Search Ring Settings")]
    public GameObject searchRingPrefab;
    public float searchRingDuration = 5.0f; // 捜索リングの存在時間

    // プレイヤー消える寸前の座標 (原本在Enemy里的static变量)
    public Vector3 LastTargetPosition { get; set; }

    // 原本的 sharedSearchRing
    private GameObject currentSearchRing;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 共有検索赤い円の生成
    public void GenerateSearchRing(Vector3 position)
    {
        LastTargetPosition = position;

        if (currentSearchRing == null)
        {
            currentSearchRing = Instantiate(searchRingPrefab, position, Quaternion.identity);
            Destroy(currentSearchRing, searchRingDuration);
        }
    }

    public void DestroySearchRing()
    {
        if (currentSearchRing != null)
        {
            Destroy(currentSearchRing);
            currentSearchRing = null;
        }
    }

    public bool HasActiveRing()
    {
        return currentSearchRing != null;
    }
}