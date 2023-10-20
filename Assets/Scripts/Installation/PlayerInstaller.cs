using FallGuys;
using UnityEngine;
using Zenject;

namespace FallGuys
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private UserInputController _userInputController;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private Lifes _lifes;

        public override void InstallBindings()
        {
            Container.BindInstance(_playerController).AsSingle();
            Container.BindInstance(_userInputController).AsSingle();
            Container.BindInstance(_cameraController).AsSingle();
            Container.BindInstance(_lifes).AsSingle();
        }
    }
}