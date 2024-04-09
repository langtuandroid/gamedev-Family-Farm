using _Project.Scripts_dev.Language;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using _Project.Scripts_dev.Сamera;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private DataManager _dataManager;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private CamFollow _camFollow;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private EffectManager _effectManager;
        [SerializeField] private LanguageManager _languageManager;
        [SerializeField] private SoundManager _soundManager;
        public override void InstallBindings()
        {
            Container.Bind<DataManager>().FromInstance(_dataManager).AsSingle();
            Container.Bind<GameManager>().FromInstance(_gameManager).AsSingle();
            Container.Bind<CamFollow>().FromInstance(_camFollow).AsSingle();
            Container.Bind<UIManager>().FromInstance(_uiManager).AsSingle();
            Container.Bind<EffectManager>().FromInstance(_effectManager).AsSingle();
            Container.Bind<LanguageManager>().FromInstance(_languageManager).AsSingle();
            Container.Bind<SoundManager>().FromInstance(_soundManager).AsSingle();
        }
    }
    
}