using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Module.Detection.Samples
{
    public class Sample_TouchObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject _target;
        [SerializeField]
        private float _createDelay;
        [SerializeField]
        private float _destroyDelay;
        [SerializeField]
        private float _zPosition;
        [SerializeField]
        private float _touchRadius;

        private Camera _camera;
        private readonly Dictionary<GameObject, Coroutine> _crtMap = new Dictionary<GameObject, Coroutine>();

        private void Start()
        {
            _camera = Camera.main;
            
            // 뷰포트 좌표계 Rect를 얻어오기위한 메시지 콜백 등록
            Message.AddListener<CV.Output.ViewportRects>(ProcessViewportRects);

            // 타겟 생성 코루틴 시작
            StartCoroutine(InstantiateTargets());
        }

        private void OnDestroy()
        {
            Message.RemoveListener<CV.Output.ViewportRects>(ProcessViewportRects);
        }

        private IEnumerator InstantiateTargets()
        {
            while (true)
            {
                GameObject target = Instantiate(_target);
                Vector3 pos = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), _zPosition);
                pos = _camera.ViewportToWorldPoint(pos);
                pos.z = _zPosition;
                target.transform.position = pos;
                _crtMap.Add(target, StartCoroutine(Destroy(target, _destroyDelay)));

                yield return new WaitForSeconds(_createDelay);
            }
        }

        private void ProcessViewportRects(CV.Output.ViewportRects output)
        {
            List<UnityEngine.Rect> viewportRects = output.Value;
            foreach (var rect in viewportRects)
            {
                // 뷰포트 좌표를 월드 좌표로 변환
                Vector3 viewportPos = new Vector3(rect.center.x, rect.center.y, _zPosition - _camera.transform.position.z);
                Vector3 worldPos = _camera.ViewportToWorldPoint(viewportPos);
                worldPos.z = _zPosition;
                
                // 충돌 처리
                foreach (var collider in Physics.OverlapSphere(worldPos, _touchRadius))
                {
                    GameObject target = collider.gameObject;
                    collider.enabled = false;
                    target.GetComponentInChildren<Renderer>().material.color = new Color(1f, 0f, 0f);
                    
                    StopCoroutine(_crtMap[target]);
                    _crtMap.Remove(target);
                    StartCoroutine(Destroy(target, _destroyDelay));
                }
            }
        }

        private IEnumerator Destroy(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            GameObject.Destroy(obj);
        }
    }
}