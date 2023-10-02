using System.Collections.Generic;
using UnityEngine;

namespace CMIYC.Location
{
#nullable enable
    public class LocationNode
    {
        public Texture2D? texture2D;
        public int? size;

        public string location;
        public List<LocationNode> children;
        public LocationNode? parent;

        public LocationNode(LocationNode? parent, string location, Texture2D? texture, int? size)
        {
            this.parent = parent;
            this.location = location;
            this.texture2D = texture;
            this.size = size;
            children = new();
        }

        public void AddChildNode(LocationNode node)
            => children.Add(node);
    }
#nullable restore
}
