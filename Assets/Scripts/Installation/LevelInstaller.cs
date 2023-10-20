using FallGuys;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace FallGuys
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private CharacterController _characterRef;
        [SerializeField] private Spawner _spawner;
        [SerializeField] private EndZone _endZone;
        [SerializeField] private StartLine _startLine;
        [SerializeField] private GameModeController _gameModeController;
        public override void InstallBindings()
        {
            Container.Bind<IFactory<CharacterController>>().FromInstance(new CharacterController.Factory(_characterRef, Container));

            Container.BindInstance(_spawner).AsSingle().NonLazy();
            Container.BindInstance(_endZone).AsSingle().NonLazy();
            Container.BindInstance(_gameModeController).AsSingle().NonLazy();
            Container.BindInstance(_startLine).AsSingle().NonLazy();
        }
    }
}