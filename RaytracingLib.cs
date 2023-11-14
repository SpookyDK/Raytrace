
namespace RaytracingBasics
{
    public class RaytracingLib
    {
        public static Ray CrossP(Ray ray1, Ray ray2)
        {
            Ray temp = new Ray(ray1.origin, new float3(ray1.direction.y * ray2.direction.z - ray1.direction.z * ray2.direction.y, -(ray1.direction.x * ray2.direction.z - ray1.direction.z * ray2.direction.x), ray1.direction.x * ray2.direction.y - ray1.direction.y * ray2.direction.x));
            return temp;
        }
        public struct Triangle
        {
            public float4 NormalPlane { get; set; }
            public Ray edge1 { get; set; }
            public Ray edge2 { get; set; }
            public Ray edge3 { get; set; }
            public float3 V1 { get; set; }
            public float3 V2 { get; set; }
            public float3 V3 { get; set; }

            public Triangle(float3 vertex1, float3 vertex2, float3 vertex3)
            {
                V1 = vertex1;
                V2 = vertex2;
                V3 = vertex3;
                NormalPlane = CalculateNormalPlane(vertex1, vertex2, vertex3);
                edge1 = new Ray(vertex1, new float3(vertex2.x - vertex1.x, vertex2.y - vertex1.y, vertex2.z - vertex1.z));
                edge2 = new Ray(vertex2, new float3(vertex3.x - vertex2.x, vertex3.y - vertex2.y, vertex3.z - vertex2.z));
                edge3 = new Ray(vertex3, new float3(vertex1.x - vertex3.x, vertex1.y - vertex3.y, vertex1.z - vertex3.z));

            }
        }
        public struct Ray
        {
            public float3 origin { get; set; }
            public float3 direction { get; set; }

            public Ray(float3 Origin, float3 Direction)
            {
                origin = Origin;
                direction = Direction;
            }
        }

        public struct float3
        {
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
            public float3(float X, float Y, float Z)
            {
                x = X;
                y = Y;
                z = Z;
            }

        }
        public struct float4
        {
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
            public float q { get; set; }
            public float4(float X, float Y, float Z, float Q)
            {
                x = X;
                y = Y;
                z = Z;
                q = Q;
            }

        }
        static public float3 CalculateIntersection(Triangle tri, Ray ray)
        {


            float t = (-tri.NormalPlane.x * ray.origin.x - tri.NormalPlane.y * ray.origin.y - tri.NormalPlane.z * ray.origin.z - tri.NormalPlane.q) / (tri.NormalPlane.x * ray.direction.x + tri.NormalPlane.y * ray.direction.y + tri.NormalPlane.z * ray.direction.z);

            float3 intersect = new float3(ray.origin.x + ray.direction.x * t, ray.origin.y + ray.direction.y * t, ray.origin.z + ray.direction.z * t);

            if (CheckInside(tri, ray, t, intersect))
            {

                return intersect;

            }
            else { return new float3(0, 0, 0); }
        }


        static public float4 CalculateNormalPlane(Triangle tri)
        {

            Ray Normal = CrossP(tri.edge1, tri.edge2);
            float4 Plane = new float4(Normal.direction.x, Normal.direction.y, Normal.direction.z, 0);
            Plane.q = -Normal.direction.x * Normal.origin.x - Normal.direction.y * Normal.origin.y - Normal.direction.z * Normal.origin.z;

            return Plane;
        }
        static public float4 CalculateNormalPlane(float3 V1, float3 V2, float3 V3)
        {
            Ray edge1 = new Ray(V1, new float3(V1.x - V2.x, V1.y - V2.y, V1.z - V2.z));
            Ray edge2 = new Ray(V1, new float3(V1.x - V3.x, V1.y - V3.y, V1.z - V3.z));
            Ray Normal = CrossP(edge1, edge2);
            float4 Plane = new float4(Normal.direction.x, Normal.direction.y, Normal.direction.z, 0);
            Plane.q = -Normal.direction.x * Normal.origin.x - Normal.direction.y * Normal.origin.y - Normal.direction.z * Normal.origin.z;

            return Plane;
        }
        static public float4 CalculateNormalPlane(Ray edge1, Ray edge2)
        {
            Ray Normal = CrossP(edge1, edge2);
            float4 Plane = new float4(Normal.direction.x, Normal.direction.y, Normal.direction.z, 0);
            Plane.q = -Normal.direction.x * Normal.origin.x - Normal.direction.y * Normal.origin.y - Normal.direction.z * Normal.origin.z;

            return Plane;
        }
        static bool CheckInside(Triangle tri, Ray ray, float4 plane, float t, float3 intersect)
        {

            Ray edge1 = new Ray(tri.V1, new float3(tri.V2.x - tri.V1.x, tri.V2.y - tri.V1.y, tri.V2.z - tri.V1.z));
            Ray edge2 = new Ray(tri.V2, new float3(tri.V3.x - tri.V2.x, tri.V3.y - tri.V2.y, tri.V3.z - tri.V2.z));
            Ray edge3 = new Ray(tri.V3, new float3(tri.V1.x - tri.V3.x, tri.V1.y - tri.V3.y, tri.V1.z - tri.V3.z));
            Ray Q1 = new Ray(tri.V1, new float3(intersect.x - tri.V1.x, intersect.y - tri.V1.y, intersect.z - tri.V1.z));
            Ray Q2 = new Ray(tri.V1, new float3(intersect.x - tri.V2.x, intersect.y - tri.V2.y, intersect.z - tri.V2.z));
            Ray Q3 = new Ray(tri.V1, new float3(intersect.x - tri.V1.x, intersect.y - tri.V1.y, intersect.z - tri.V1.z));
            Ray Cross1 = CrossP(edge1, Q1);
            Ray Cross2 = CrossP(edge2, Q2);
            Ray Cross3 = CrossP(edge3, Q3);

            if (Cross1.direction.x * plane.x >= 0 && Cross1.direction.y * plane.y >= 0 && Cross1.direction.z * plane.z >= 0)
            {

                if (Cross2.direction.x * plane.x >= 0 && Cross2.direction.y * plane.y >= 0 && Cross2.direction.z * plane.z >= 0)
                {

                    if (Cross3.direction.x * plane.x >= 0 && Cross3.direction.y * plane.y >= 0 && Cross3.direction.z * plane.z >= 0)
                    {

                        return true;
                    }
                    else { return false; }
                }
                else { return false; }
            }
            else { return false; }


        }
        static bool CheckInside(Triangle tri, Ray ray, float t, float3 intersect)
        {


            Ray Q1 = new Ray(tri.V1, new float3(intersect.x - tri.V1.x, intersect.y - tri.V1.y, intersect.z - tri.V1.z));
            Ray Q2 = new Ray(tri.V1, new float3(intersect.x - tri.V2.x, intersect.y - tri.V2.y, intersect.z - tri.V2.z));
            Ray Q3 = new Ray(tri.V1, new float3(intersect.x - tri.V1.x, intersect.y - tri.V1.y, intersect.z - tri.V1.z));
            Ray Cross1 = CrossP(tri.edge1, Q1);
            Ray Cross2 = CrossP(tri.edge2, Q2);
            Ray Cross3 = CrossP(tri.edge3, Q3);


            if (Cross1.direction.x * tri.NormalPlane.x >= 0 && Cross1.direction.y * tri.NormalPlane.y >= 0 && Cross1.direction.z * tri.NormalPlane.z >= 0)
            {

                if (Cross2.direction.x * tri.NormalPlane.x >= 0 && Cross2.direction.y * tri.NormalPlane.y >= 0 && Cross2.direction.z * tri.NormalPlane.z >= 0)
                {

                    if (Cross3.direction.x * tri.NormalPlane.x >= 0 && Cross3.direction.y * tri.NormalPlane.y >= 0 && Cross3.direction.z * tri.NormalPlane.z >= 0)
                    {
                        return true;
                    }
                    else { return false; }
                }
                else { return false; }
            }
            else { return false; }


        }
    }
}