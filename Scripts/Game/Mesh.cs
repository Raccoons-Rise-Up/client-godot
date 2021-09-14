using Godot;
using System;

public class Mesh : MeshInstance2D
{
    public override void _Ready()
    {
        var st = new SurfaceTool();
        st.Begin(Godot.Mesh.PrimitiveType.Triangles);

        // Prepare attributes for add_vertex.
        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(0, 0));
        // Call last for each vertex, adds the above attributes.
        st.AddVertex(new Vector3(-1, -1, 0));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(0, 1));
        st.AddVertex(new Vector3(-1, 1, 0));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(1, 1));
        st.AddVertex(new Vector3(1, 1, 0));

        // Commit to a mesh.
        var mesh = st.Commit();
        Mesh = mesh;
    }

    public override void _Process(float delta)
    {
        
    }
}
