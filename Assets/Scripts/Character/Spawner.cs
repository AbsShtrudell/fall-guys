using System;
using UnityEngine;
using Zenject;

namespace FallGuys
{
    public class Spawner : MonoBehaviour
    {
        [Inject]
        private IFactory<CharacterController> _characterFactory;

        public event Action<CharacterController> Spawned;

        public CharacterController Spawn()
        {
            var character = _characterFactory.Create();
            character.transform.position = transform.position;

            Spawned?.Invoke(character);

            return character;
        }
    }
}