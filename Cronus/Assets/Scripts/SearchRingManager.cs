using UnityEngine;
using System.Collections;

public class SearchRingManager : MonoBehaviour
{
    public static SearchRingManager Instance { get; private set; }

    [Header("Search Ring Settings")]
    public GameObject searchRingPrefab;
    public float searchRingDuration = 10.0f;

    public Vector3 LastTargetPosition { get; set; }
    private GameObject currentSearchRing;
    private Coroutine destroyCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GenerateSearchRing(Vector3 position)
    {
        LastTargetPosition = position;

        if (currentSearchRing == null)
        {
            currentSearchRing = Instantiate(searchRingPrefab, position, Quaternion.identity);
        }
        else
        {
            //もし既に存在している 最新の位置に更新
            currentSearchRing.transform.position = position;
            if (destroyCoroutine != null)
                StopCoroutine(destroyCoroutine);
        }

        destroyCoroutine = StartCoroutine(RingTimer(searchRingDuration));
    }

    private IEnumerator RingTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroySearchRing();
    }

    public void DestroySearchRing()
    {
        if (currentSearchRing != null)
        {
            Destroy(currentSearchRing);
            currentSearchRing = null;
        }
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
    }

    public bool HasActiveRing() => currentSearchRing != null;
}