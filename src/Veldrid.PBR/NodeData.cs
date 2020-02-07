using System.Numerics;

namespace Veldrid.PBR
{
    public struct NodeData
    {
        public int Name;
        public int ParentNode;
        public int MeshIndex;
        public Matrix4x4 LocalTransform;
        public Matrix4x4 WorldTransform;
    }
}