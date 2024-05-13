using System;
using System.Collections;
using Integration;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Managers
{
    public class ShowInterstitial : MonoBehaviour
    {
        [Inject] private AdMobController _adMobController;
        private static int _timePassed;
        private void Awake()
        {
            StartCoroutine(RewardRoutine());
        }

        private IEnumerator RewardRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                _timePassed++;
                if (_timePassed >= 120)
                {
                    _timePassed -= 120;
                    ShowAdd();
                }
            }
        }

        private void ShowAdd()
        {
            _adMobController.ShowInterstitialAd();
        }
    }
}