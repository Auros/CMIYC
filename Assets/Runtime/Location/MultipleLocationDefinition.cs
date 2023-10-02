using System;
using System.Collections.Generic;
using System.Linq;
using CMIYC.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CMIYC.Location
{
    public class MultipleLocationDefinition : MonoBehaviour
    {
        // low tech solution to avoid duplicates
        // TODO clear in unity editor when scene stopped
        private static Dictionary<string, int> _usedLocations = new();

        private static readonly List<string> _suffixes = new()
        {
            "_NEW",
            "old",
            " (1)",
            "_1",
            " (Copy)",
            "_backup",
            "(BACKUP)",
            " (2)",
            "_FINAL",
            " (FINAL)",
            "_LAST",
            "LASTBACKUPFORREAL",
            "_secret",
        };


        [SerializeField]
        private List<string> _locations = new List<string>();

        private string _location = null!;

        // idk if awake will always trigger before OnTriggerEnter.. but probably!!
        void Awake()
        {
            // pick a name
            PickBestName();

            Debug.Log(_location);
        }

        private void PickBestName()
        {
            if (_locations.Count == 0)
            {
                // idk do something
                Debug.LogWarning("No locations found in _locations!");
                _location = "";
                return;
            }

            Dictionary<string, int> localScores = new();
            foreach (var location in _locations)
            {
                if (_usedLocations.TryGetValue(location, out int count))
                {
                    localScores.TryAdd(location, count);
                }
                else
                {
                    localScores.TryAdd(location, 0);
                }
            }

            var lowest = localScores.Min(x => x.Value);
            var lowestScoreLocations = localScores.Where(x => x.Value == lowest).ToList(); //?

            var randomLocationAndScore = lowestScoreLocations[Random.Range(0, lowestScoreLocations.Count())];
            string randomLocation;
            if (randomLocationAndScore.Value > 0)
            {
                // add some flair
                randomLocation = AddSuffixesRecursive(randomLocationAndScore.Key);
            }
            else
            {
                randomLocation = randomLocationAndScore.Key;
            }

            // set
            if (_usedLocations.TryGetValue(randomLocation, out int randomCount))
            {
                _usedLocations[randomLocation] = randomCount + 1;
            }
            else
            {
                _usedLocations.Add(randomLocation, 0);
            }

            _location = randomLocation;
        }

        // TODO: Impl this so it'll ALWAYS chain instead of checking
        // eg: Documents | Documents (new) | Documents (new)_COPY
        // instead of Documents | Documents (new) | Documents (copy)
        private string AddSuffixesRecursive(string location)
        {
            var suffix = _suffixes[Random.Range(0, _suffixes.Count)];

            var suffixed = location + suffix;
            if (_usedLocations.TryGetValue(suffixed, out int count))
            {
                return AddSuffixesRecursive(suffixed);
            }

            return suffixed;
        }

        // idk if like a layermask needs to be set here or something
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                player.BroadcastMessage(nameof(LocationController.EnterLocation), _location, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                player.BroadcastMessage(nameof(LocationController.ExitLocation), SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
