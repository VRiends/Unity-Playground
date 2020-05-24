using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSectionGenerator : MonoBehaviour
{

    /// <summary>
    /// PointA - D defined relative to SphereCenter and normalized to lenght 1
    /// </summary>
    public class SphereSection
    {
        private class MeshData
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<Vector2> uvCoords = new List<Vector2>();
            public List<int> triangles = new List<int>();
        }

        public SphereSection(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float outR, float inR)
        {
            PointA = A;
            PointB = B;
            PointC = C;
            PointD = D;
            outerRadius = outR;
            innerRadius = inR;
        }
        
        Vector3 PointA; // down left
        Vector3 PointB; // up left
        Vector3 PointC; // up right
        Vector3 PointD; // down right
        float outerRadius;
        float innerRadius;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution">distance between neighbouring vertices on the outside</param>
        /// <returns></returns>
        public Mesh CreateMesh(float resolution)
        {
            Mesh sphereSection = new Mesh();

            MeshData meshData = new MeshData();

            float angleIncr = Mathf.Acos(-1 * (resolution * resolution) / (-2 * outerRadius * outerRadius));
            float horizontalInkrement = angleIncr / Vector3.Angle(PointA, PointD);
            float verticalIncrement = angleIncr / Vector3.Angle(PointA, PointB);

            int vertsPerRowOld = (int) Mathf.Ceil(1 / horizontalInkrement);
            int vertsPerRow = 0;
            int currentVertIndex = 0;

            for (float ver= 0; ver <=1; ver = incValue(ver, verticalIncrement))
            {
                // TODO do I need to normalize these?
                Vector3 leftInterpolant = Vector3.Slerp(PointA, PointB, ver);
                Vector3 rightInterpolant = Vector3.Slerp(PointD, PointC, ver);
                for (float hor = 0; hor <=1; hor = incValue(hor, horizontalInkrement))
                {
                    if(ver == 0)
                    {
                        vertsPerRow++;
                    }
                    meshData.vertices.Add(outerRadius * Vector3.Slerp(leftInterpolant, rightInterpolant, hor));
                    currentVertIndex = meshData.vertices.Count -1;
                    // TODO generate more usefull uvs
                    meshData.uvCoords.Add(new Vector2(ver, hor));
                    if (ver > 0 && hor > 0)
                    {
                        meshData.triangles.Add(currentVertIndex);
                        meshData.triangles.Add(currentVertIndex-1);
                        meshData.triangles.Add(currentVertIndex - vertsPerRow);

                        meshData.triangles.Add(currentVertIndex - 1);
                        meshData.triangles.Add(currentVertIndex - 1 - vertsPerRow);
                        meshData.triangles.Add(currentVertIndex - vertsPerRow);
                    }
                }
            }
            
            


            sphereSection.vertices = meshData.vertices.ToArray();
            sphereSection.uv = meshData.uvCoords.ToArray();
            sphereSection.triangles = meshData.triangles.ToArray();

            return sphereSection;
        }

        private float incValue(float curr, float increment)
        {
            if(curr < 1)
            {
                return curr + increment < 1 ? curr + increment : 1;
            }
            return 2;
        }
    }

    [SerializeField]
    Vector3 PointA; // down left
    [SerializeField]
    Vector3 PointB; // up left
    [SerializeField]
    Vector3 PointC; // up right
    [SerializeField]
    Vector3 PointD; // down right
    [SerializeField]
    float outerRadius;
    [SerializeField] 
    float innerRadius;

    [SerializeField]
    float resolution;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public void Awake()
    {
        PointA.Normalize();
        PointB.Normalize();
        PointC.Normalize();
        PointD.Normalize();

        SphereSection section = new SphereSection(PointA, PointB, PointC, PointD, outerRadius, innerRadius);

        meshFilter = GetComponent<MeshFilter>();
        
        meshFilter.mesh = section.CreateMesh(resolution);
    }

    
}
