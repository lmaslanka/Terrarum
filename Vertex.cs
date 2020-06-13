namespace Terrarum
{
    using OpenTK;

    public class Vertex
    {
        public Vertex(float[] position)
        {
            this.position = position;
        }

        public readonly float[] position;
    }
}