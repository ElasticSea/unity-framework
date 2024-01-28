using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public struct FasterBounds
    {
        public FasterBounds(float xmin, float ymin, float zmin, float xmax, float ymax, float zmax)
        {
            this.xmin = xmin;
            this.ymin = ymin;
            this.zmin = zmin;
            this.xmax = xmax;
            this.ymax = ymax;
            this.zmax = zmax;
        }

        public Bounds ToBounds()
        {
            var min = new Vector3(xmin, ymin, zmin);
            var max = new Vector3(xmax, ymax, zmax);
            return Utils.Bounds(min, max);
        }

        public float xmin;
        public float ymin;
        public float zmin;
        public float xmax;
        public float ymax;
        public float zmax;
    }
}