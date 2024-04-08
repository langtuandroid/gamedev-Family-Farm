using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCamera : MonoBehaviour
{
    public static FocusCamera instance;
    public CinemachineVirtualCamera virtualCamera;
    public float temporaryFocusDuration = 3f;
    public Transform camfollow;
    private CinemachineTransposer cameraTransposer;

    private float originalFOV;
    private Coroutine focusCoroutine;
    private void Start()
    {
        instance = this;
        cameraTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        originalFOV = virtualCamera.m_Lens.FieldOfView;
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
        while (GameManager.instance.gainExp == false)
        {
            virtualCamera.gameObject.SetActive(false);
            yield return null;
        }
        virtualCamera.gameObject.SetActive(true);
        // GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        //  Vector3 originalCameraPosition = cameraTransposer.m_FollowOffset;
        // cameraTransposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;

        //     cameraTransposer.m_FollowOffset =new Vector3(0,10,10);
        GameObject go = Instantiate(new GameObject(), targetObject.transform.position + new Vector3(0, 0, 0), targetObject.transform.rotation);
        virtualCamera.Follow = go.transform;
      //  virtualCamera.m_Lens.FieldOfView = originalFOV / 1.3f;

       yield return new WaitForSeconds(temporaryFocusDuration);

       // virtualCamera.Follow = camfollow;
       // virtualCamera.m_Lens.FieldOfView = originalFOV;
       // cameraTransposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;

       // cameraTransposer.m_FollowOffset = originalCameraPosition;

        focusCoroutine = null;
        virtualCamera.gameObject.SetActive(false);
    }
}
