using System.Collections;
using _Project.Scripts_dev.Managers;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Сamera
{
    public class CameraFocus : MonoBehaviour
    {
        public static CameraFocus instance;
        [Inject] private GameManager _gameManager;
        [SerializeField] private CinemachineVirtualCamera _camera;
        private readonly float _focusDuration = 3f;
        private Coroutine _focusCoroutine;
        private void Start()
        {
            instance = this;
        }
        public void StartFocus(GameObject targetObject)
        {
            if (_camera != null)
            {
                _camera.gameObject.SetActive(true);
                if (_focusCoroutine != null)
                {
                    StopCoroutine(_focusCoroutine);
                }

                _focusCoroutine = StartCoroutine(FocusRoutine(targetObject));
            }
        }

        private IEnumerator FocusRoutine(GameObject targetObject)
        {
            while (_gameManager.IsExpGet == false)
            {
                _camera.gameObject.SetActive(false);
                yield return null;
            }
            _camera.gameObject.SetActive(true);
            GameObject go = Instantiate(new GameObject(), targetObject.transform.position + new Vector3(0, 0, 0), targetObject.transform.rotation);
            _camera.Follow = go.transform;
            yield return new WaitForSeconds(_focusDuration);
            _focusCoroutine = null;
            _camera.gameObject.SetActive(false);
        }
    }
}
