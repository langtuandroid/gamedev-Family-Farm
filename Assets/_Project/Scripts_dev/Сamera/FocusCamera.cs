using System.Collections;
using _Project.Scripts_dev.Managers;
using Cinemachine;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Сamera
{
    public class FocusCamera : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        public static FocusCamera instance;
        public CinemachineVirtualCamera virtualCamera;
        public float temporaryFocusDuration = 3f;

        private Coroutine focusCoroutine;
        private void Start()
        {
            instance = this;
        }
        public void SetCameraFocus(GameObject targetObject)
        {
            if (virtualCamera != null)
            {
                virtualCamera.gameObject.SetActive(true);
                if (focusCoroutine != null)
                {
                    StopCoroutine(focusCoroutine);
                }

                focusCoroutine = StartCoroutine(TemporaryFocusCoroutine(targetObject));
            }
        }

        private IEnumerator TemporaryFocusCoroutine(GameObject targetObject)
        {
            while (_gameManager.IsExpGet == false)
            {
                virtualCamera.gameObject.SetActive(false);
                yield return null;
            }
            virtualCamera.gameObject.SetActive(true);
            GameObject go = Instantiate(new GameObject(), targetObject.transform.position + new Vector3(0, 0, 0), targetObject.transform.rotation);
            virtualCamera.Follow = go.transform;
            yield return new WaitForSeconds(temporaryFocusDuration);
            focusCoroutine = null;
            virtualCamera.gameObject.SetActive(false);
        }
    }
}
