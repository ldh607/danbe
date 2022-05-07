using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Output = CellBig.Module.Detection.CV.Output;

public class Sample_Waterplay : MonoBehaviour
{
    [SerializeField]
    private bool _showColliders;
    [SerializeField]
    private float _zPosition = 10f;

    [Space]

    [SerializeField]
    private float _polygonDestroyDelay = 5f;
    [SerializeField]
    private int _maxPolygonCount;

    private Camera _camera;
    private List<GameObject> _polygonPool;
    private int _polygonCount;

    private void Awake()
    {
        _camera = Camera.main;
        _polygonPool = new List<GameObject>();
    }

    private void Start()
    {
        Message.AddListener<Output.ViewportContours>(ProcessContours);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<Output.ViewportContours>(ProcessContours);
    }

    private void Update()
    {

    }

    private void ProcessContours(Output.ViewportContours output)
    {
        foreach (var contour in output.Value)
        {
            if (contour.Count == 0)
                continue;
            if (_polygonCount >= _maxPolygonCount)
                return;

            GameObject inst = null;
            if (_polygonPool.Count > 0)
            {
                inst = _polygonPool[_polygonPool.Count - 1];
                _polygonPool.RemoveAt(_polygonPool.Count - 1);
            }
            else
            {
                //inst = Instantiate(_polygonObject);
                inst = new GameObject("Polygon", typeof(EdgeCollider2D));
            }
            _polygonCount++;

            List<Vector2> points = new List<Vector2>();// contour.ToArray();
            Vector2 center = new Vector2();
            foreach (Vector2 viewportPoint in contour)
                center += viewportPoint;
            center /= contour.Count;

            Vector3 position = _camera.ViewportToWorldPoint(new Vector3(center.x, center.y, _zPosition - _camera.transform.position.z));
            position.z = _zPosition;

            inst.transform.position = position;
            inst.SetActive(true);

            int step = Mathf.Max(1, 1);
            for (int i = 0; i < contour.Count; i += step)
            {
                Vector2 point = contour[i];
                points.Add(_camera.ViewportToWorldPoint(new Vector3(point.x, point.y, _zPosition - _camera.transform.position.z)) - position);
            }

            //var polygonCollider = inst.GetComponent<PolygonCollider2D>();
            //if (polygonCollider != null) { polygonCollider.points = points.ToArray(); }

            var edgeCollider = inst.GetComponent<EdgeCollider2D>();
            if (edgeCollider != null)
            {
                edgeCollider.points = points.ToArray();
                var obiCollider = inst.AddComponent<Obi.ObiCollider2D>();
                obiCollider.SourceCollider = edgeCollider;
            }



#if UNITY_EDITOR
            if (_showColliders)
            {
                Object[] objects = UnityEditor.Selection.objects;
                Object[] newObjects = new Object[objects.Length + 1];
                System.Array.Copy(objects, newObjects, objects.Length);
                newObjects[newObjects.Length - 1] = inst;
                UnityEditor.Selection.objects = newObjects;
            }
#endif


            StartCoroutine(DestroyRoutine(inst, _polygonDestroyDelay, _polygonPool));
        }
    }

    private IEnumerator DestroyRoutine(GameObject obj, float delay, List<GameObject> pool)
    {
        yield return new WaitForSeconds(delay);
        var obiCollider = obj.GetComponent<Obi.ObiCollider2D>();
        if (obiCollider != null) { Destroy(obiCollider); }
        obj.SetActive(false);
        pool.Add(obj);

        if (pool == _polygonPool)
            _polygonCount--;
    }
}
