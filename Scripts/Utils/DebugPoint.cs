using Godot;

namespace Client.Utilities
{
    public class DebugPoint : MeshInstance
    {
        public DebugPoint(Vector3 pos) : this(pos, Colors.AliceBlue)
        {

        }

        public DebugPoint(Vector3 pos, Color color)
        {
            // set mesh
            Mesh = new CubeMesh();

            var mat = new SpatialMaterial();
            mat.AlbedoColor = color;
            MaterialOverride = mat;

            // set position
            Translate(pos);

            // set scale
            var scale = 1f;
            Scale = new Vector3(scale, scale, scale);
        }
    }
}
