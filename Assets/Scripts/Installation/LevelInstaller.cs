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
        public override void InstallBindings()
        {
            Container.Bind<IFactory<CharacterController>>().FromInstance(new CharacterController.Factory(_characterRef, Container));

            Container.BindInstance(_spawner).AsSingle().NonLazy();
        }
    }
}