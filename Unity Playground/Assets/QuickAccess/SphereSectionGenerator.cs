using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

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

            Mesh outerAndInnerSurfaces = CreateInnerAndOuterSphereSurface(resolution);

            CombineInstance[] combine = new CombineInstance[3];
            combine[0].mesh = outerAndInnerSurfaces;
            //combine[1].mesh = LeftAndRightSurfaces;
            //combine[2].mesh = TopAndBottomSurfaces;

            sphereSection.CombineMeshes(combine, true, false);

            sphereSection.RecalculateNormals();
            sphereSection.Optimize();

            return sphereSection;
        }

        private Mesh CreateInnerAndOuterSphereSurface(float resolution)
        {
            MeshData outerSurfaceData = new MeshData();
            MeshData innerSurfaceData = new MeshData();
            // TODO does not work as intended, yet
            float cosVal =  ((resolution * resolution) - 2 * (outerRadius * outerRadius)) / (-2 * outerRadius * outerRadius);
            float angleIncr = Mathf.Rad2Deg * Mathf.Acos(cosVal);
            float horizontalInkrement = angleIncr / Vector3.Angle(PointA, PointD);
            float verticalIncrement = angleIncr / Vector3.Angle(PointA, PointB);

            Debug.Log($"cosval:{cosVal}");
            Debug.Log($"angleIncr:{angleIncr}");
            Debug.Log($"horiztontalIncr:{horizontalInkrement}");
            Debug.Log($"verticalIncrement:{verticalIncrement}");

            int vertsPerRow = 0;


            // create outer and inner Sphere surfaces
            for (float ver = 0; ver <= 1; ver = incValue(ver, verticalIncrement))
            {
                // TODO do I need to normalize these?
                Vector3 leftInterpolant = Vector3.Slerp(PointA, PointB, ver);
                Vector3 rightInterpolant = Vector3.Slerp(PointD, PointC, ver);
                for (float hor = 0; hor <= 1; hor = incValue(hor, horizontalInkrement))
                {
                    if (ver == 0)
                    {
                        vertsPerRow++;
                    }
                    outerSurfaceData.vertices.Add(outerRadius * Vector3.Slerp(leftInterpolant, rightInterpolant, hor));
                    innerSurfaceData.vertices.Add(innerRadius * Vector3.Slerp(leftInterpolant, rightInterpolant, hor));
                                       
                    // TODO generate more usefull uvs
                    outerSurfaceData.uvCoords.Add(new Vector2(ver, hor));
                    innerSurfaceData.uvCoords.Add(new Vector2(ver, hor));
                    if (ver > 0 && hor > 0)
                    {
                        AddTrianglesOnSphere(vertsPerRow, ref outerSurfaceData);
                        AddTrianglesOnSphere(vertsPerRow, ref innerSurfaceData);                       
                    }
                }
            }

            Mesh innerSurface = new Mesh();
            innerSurface.vertices = innerSurfaceData.vertices.ToArray();
            innerSurface.uv = innerSurfaceData.uvCoords.ToArray();
            innerSurfaceData.triangles.Reverse();
            innerSurface.triangles = innerSurfaceData.triangles.ToArray();

            Mesh outerSurface = new Mesh();
            outerSurface.vertices = outerSurfaceData.vertices.ToArray();
            outerSurface.uv= outerSurfaceData.uvCoords.ToArray();
            outerSurface.triangles = outerSurfaceData.triangles.ToArray();

            Mesh output = new Mesh();

            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = innerSurface;
            combine[1].mesh = outerSurface;

            output.CombineMeshes(combine,true,false);

            return output;

        }

        private void AddTrianglesOnSphere(int vertsPerRow, ref MeshData meshData)
        {
            int currentVertIndex = meshData.vertices.Count - 1;
            meshData.triangles.Add(currentVertIndex);
            meshData.triangles.Add(currentVertIndex - 1);
            meshData.triangles.Add(currentVertIndex - vertsPerRow);

            meshData.triangles.Add(currentVertIndex - 1);
            meshData.triangles.Add(currentVertIndex - 1 - vertsPerRow);
            meshData.triangles.Add(currentVertIndex - vertsPerRow);
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
