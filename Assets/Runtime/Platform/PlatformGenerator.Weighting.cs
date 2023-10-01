using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CMIYC.Platform
{
    public partial class PlatformGenerator
    {
        private RoomDefinition[]? _weightedRoomOptions;

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private RoomDefinition[] GetWeightedRoomOptions()
        {
            if (_weightedRoomOptions is not null)
                return _weightedRoomOptions;

            var options = ListPool<RoomSpawnOption>.Get();
            foreach (var option in _roomSpawnOptions)
            {
                var (x, y) = option.Prefab.Size;

                // If the room is too large, eliminate it from valid options
                if (x > _motherboardSize.x || x > _motherboardSize.y || y > _motherboardSize.x || y > _motherboardSize.y)
                    continue;

                options.Add(option);
            }

            int totalRepresentations = 0;
            Span<int> representationIndicies = stackalloc int[options.Count];
            float weightMin = options.Count != 0 ? options[0].Weight : 0;

            foreach (var option in options)
                if (weightMin > option.Weight && option.Weight > 0)
                    weightMin = option.Weight;

            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                // Skip zero-weighted rooms
                if (0 >= option.Weight)
                    continue;

                var representation = Mathf.CeilToInt(weightMin * option.Weight);
                representationIndicies[i] = representation;
                totalRepresentations += representation;
            }

            _weightedRoomOptions = new RoomDefinition[totalRepresentations];
            for (int i = 0; i < representationIndicies.Length; i++)
                for (int c = 0; c < representationIndicies[i]; c++)
                    _weightedRoomOptions[i + c] = options[i].Prefab;

            ListPool<RoomSpawnOption>.Release(options);
            return _weightedRoomOptions;
        }
    }
}
