using System.Collections;
using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.Car
{
    public class CarAnimation : MonoBehaviour
    {
        [Inject] private UIManager _uiManager;
        [Inject] private GameManager _gameManager;
        [SerializeField] SpinTransform wheel1, wheel2;
        [SerializeField] float scaleAmount=1.2f,duration=0.3f;
        [SerializeField] GameObject trailEffect;
        [SerializeField] GameObject truckCart;
        [SerializeField] GameObject truckBody;
        [SerializeField] Image fuelImage;
        [SerializeField] TextMeshProUGUI fuelText;
        [SerializeField] AudioSource engineSound;
        [SerializeField] AudioSource bimbimSound;
        private CharacterController characterController;
        private PlayerControl playerControl;
        private Vector3 originalScaleBody;
        private Vector3 originalScaleCart;
        public bool running;
        private bool posSet;
        private void Start()
        {
            characterController = transform.parent.GetComponent<CharacterController>();
            playerControl = transform.parent.GetComponent<PlayerControl>();
            StartCoroutine(BimBim());
        }
        private void Update()
        {
            if(_gameManager.truckTime<=0) engineSound.volume =0;
            if (!posSet&& playerControl.enabled==true)
            {
                originalScaleBody = truckBody.transform.localPosition;
                originalScaleCart = truckCart.transform.localPosition;
                posSet = true;
            }
            if (characterController.velocity != Vector3.zero&&!running && playerControl.enabled)
            {
                Debug.Log(characterController.enabled);
                StartCoroutine(DoCarAnim());
                wheel1.enabled = true;
                wheel2.enabled = true;
                engineSound.volume = SoundManager.instance.sound.volume;
            }
            if(characterController.velocity == Vector3.zero || !playerControl.enabled)
            {
                wheel1.enabled = false;
                wheel2.enabled = false;
                // StopAllCoroutines();
                DOTween.Kill(transform);
                if (playerControl.enabled)
                    engineSound.volume = 0.4f* SoundManager.instance.sound.volume;
                else
                {
                    engineSound.volume = 0;
                }
            }
            fuelImage.fillAmount = _gameManager.truckTime / 180f;
            fuelText.text = _uiManager.FormatTime(_gameManager.truckTime);
            if (_gameManager.truckTime <= 0) fuelImage.transform.parent.gameObject.SetActive(false);
            else fuelImage.transform.parent.gameObject.SetActive(true);
        }
        private IEnumerator BimBim()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(10f, 15f));
                bimbimSound.volume = SoundManager.instance.sound.volume;
                if (characterController.velocity != Vector3.zero) bimbimSound.Play();
          
            }
        }
        private IEnumerator DoCarAnim()
        {
            running = true;
            truckCart.transform.DOLocalMoveY(originalScaleCart.y + scaleAmount, duration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                truckCart.transform.DOLocalMoveY(originalScaleCart.y , duration).SetEase(Ease.OutQuad);
            }); truckBody.transform.DOLocalMoveY(originalScaleBody.y + scaleAmount, duration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                truckBody.transform.DOLocalMoveY(originalScaleBody.y , duration).SetEase(Ease.OutQuad);
            });
            if (playerControl.enabled&& _gameManager.truckTime > 1)
            {
                Instantiate(trailEffect, transform).transform.localPosition = new Vector3(0.05f, 0.02f, -0.15f);
                Instantiate(trailEffect, transform).transform.localPosition = new Vector3(-0.05f, 0.02f, -0.15f);
           
            }
            yield return new WaitForSeconds(duration);
            if (playerControl.enabled && _gameManager.truckTime > 1)
            {
                Instantiate(trailEffect, transform).transform.localPosition = new Vector3(0.05f, 0.02f, -0.15f);
                Instantiate(trailEffect, transform).transform.localPosition = new Vector3(-0.05f, 0.02f, -0.15f);
           
            }

            yield return new WaitForSeconds( duration );
            running = false;
        }
    }
}
