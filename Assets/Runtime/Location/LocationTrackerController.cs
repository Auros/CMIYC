using System;
using CMIYC.Enemy;
using CMIYC.Enemy.Behaviour;
using UnityEngine;

namespace CMIYC.Location
{
    public class LocationTrackerController : MonoBehaviour
    {
        public LocationNode RootNode { get; private set; }

        [SerializeField]
        private LocationController _locationController = null!;

        [SerializeField]
        private EnemyController _enemyController = null!;

        [SerializeField]
        private Texture2D _driveSprite = null!;
        [SerializeField]
        private Texture2D _folderSprite = null!;
        [SerializeField]
        private Texture2D _txtSprite = null!;

        private LocationNode _workingNode;

        private void Start()
        {
            RootNode = _workingNode = new LocationNode(null, string.Empty, null, null);

            _enemyController.OnEnemyDeath += OnEnemyDeath;
            _locationController.OnLocationEnter += OnLocationEnter;
            _locationController.OnLocationExit += OnLocationExit;

            OnLocationEnter(_locationController.GetFullLocation()[0..^1]);
        }



        private void OnLocationEnter(string obj)
        {
            var node = new LocationNode(_workingNode, obj, _workingNode == RootNode ? _driveSprite : _folderSprite, null);
            _workingNode.AddChildNode(node);
            _workingNode = node;
        }

        private void OnLocationExit(string obj)
        {
            _workingNode = _workingNode.parent ?? RootNode;
        }

        private void OnEnemyDeath(EnemyBehaviour obj)
        {
            var name = obj switch
            {
                TxtBehaviour txt => txt.AssignedMetadata.NameTag,
                PngBehaviour png => png.AssignedMetadata.NameTag,
                JpgBehaviour jpg => jpg.AssignedMetadata.NameTag,
                FbxBehaviour fbx => fbx.AssignedMetadata.NameTag,
                _ => throw new InvalidOperationException("bro forgot to fill out information here LMAOOO")
            };

            var texture = obj switch
            {
                TxtBehaviour => _txtSprite,
                PngBehaviour png => png.AssignedMetadata.Texture,
                JpgBehaviour jpg => jpg.AssignedMetadata.Texture,
                FbxBehaviour fbx => fbx.AssignedMetadata.Texture,
                _ => null
            };

            var size = 0; // TODO calculate points at the enemy level, shared between here and ScoreController

            var enemyNode = new LocationNode(_workingNode, name, texture, size);
            _workingNode.AddChildNode(enemyNode);
        }

        private void OnDestroy()
        {
            _enemyController.OnEnemyDeath -= OnEnemyDeath;
            _locationController.OnLocationEnter -= OnLocationEnter;
            _locationController.OnLocationExit -= OnLocationExit;
        }
    }
}
