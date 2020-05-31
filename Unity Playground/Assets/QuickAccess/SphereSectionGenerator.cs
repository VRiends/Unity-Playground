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
        // TODO: Handle Edgecase when innerRadius is 0
        public Mesh CreateMesh(float resolution)
        {
            Mesh sphereSection = new Mesh();

            Mesh outerAndInnerSurfaces = CreateInnerAndOuterSphereSurface(resolution);
            Mesh leftAndRightSurfaces = CreateLeftAndRightSurfaces(resolution);
            Mesh topAndBottomSurfaces = CreateTopAndBottomSurfaces(resolution);

            CombineInstance[] combine = new CombineInstance[3];
            combine[0].mesh = outerAndInnerSurfaces;
            combine[1].mesh = leftAndRightSurfaces;
            combine[2].mesh = topAndBottomSurfaces;

            sphereSection.CombineMeshes(combine, true, false);

            sphereSection.RecalculateNormals();
            sphereSection.Optimize();

            return sphereSection;
        }

        private Mesh CreateLeftAndRightSurfaces(float resolution)
        {
            MeshData leftSurfaceData = new MeshData();
            MeshData rightSurfaceData = new MeshData();
            float cosVal = ((resolution * resolution) - 2 * (outerRadius * outerRadius)) / (-2 * outerRadius * outerRadius);
            float angleIncr = Mathf.Rad2Deg * Mathf.Acos(cosVal);
            float horizontalInkrement = resolution/(outerRadius - innerRadius);
            float verticalIncrement = angleIncr / Vector3.Angle(PointA, PointB);

            int vertsPerRow = 0;

            // create left and right surfaces of sprhere section
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
                    leftSurfaceData.vertices.Add((innerRadius + hor * (outerRadius - innerRadius)) * leftInterpolant);
                    rightSurfaceData.vertices.Add((innerRadius + hor * (outerRadius - innerRadius)) * rightInterpolant);
                                       
                    // TODO generate more usefull uvs
                    leftSurfaceData.uvCoords.Add(new Vector2(ver, hor));
                    rightSurfaceData.uvCoords.Add(new Vector2(ver, hor));
                    if (ver > 0 && hor > 0)
                    {
                        AddTrianglesOnSphere(vertsPerRow, ref leftSurfaceData);
                        AddTrianglesOnSphere(vertsPerRow, ref rightSurfaceData);                       
                    }
                }
            }

            return CreateCombinedMesh(rightSurfaceData, leftSurfaceData);
        }

        private Mesh CreateTopAndBottomSurfaces(float resolution)
        {
            MeshData topSurfaceData = new MeshData();
            MeshData bottomSurfaceData = new MeshData();
            float cosVal = ((resolution * resolution) - 2 * (outerRadius * outerRadius)) / (-2 * outerRadius * outerRadius);
            float angleIncr = Mathf.Rad2Deg * Mathf.Acos(cosVal);
            float horizontalInkrement = angleIncr / Vector3.Angle(PointA, PointD);
            float verticalIncrement = resolution / (outerRadius - innerRadius); 

            int vertsPerRow = 0;

            // create left and right surfaces of sprhere section
            for (float hor = 0; hor <= 1; hor = incValue(hor, horizontalInkrement))
            {
                // TODO do I need to normalize these?
                Vector3 topInterpolant = Vector3.Slerp(PointB, PointC, hor);
                Vector3 bottomInterpolant = Vector3.Slerp(PointA, PointD, hor);
                for (float ver = 0; ver <= 1; ver = incValue(ver, verticalIncrement))
                {
                    if (hor == 0)
                    {
                        vertsPerRow++;
                    }
                    topSurfaceData.vertices.Add((innerRadius + ver * (outerRadius - innerRadius)) * topInterpolant);
                    bottomSurfaceData.vertices.Add((innerRadius + ver * (outerRadius - innerRadius)) * bottomInterpolant);

                    // TODO generate more usefull uvs
                    topSurfaceData.uvCoords.Add(new Vector2(hor, ver));
                    bottomSurfaceData.uvCoords.Add(new Vector2(hor, ver));
                    if (hor > 0 && ver > 0)
                    {
                        AddTrianglesOnSphere(vertsPerRow, ref topSurfaceData);
                        AddTrianglesOnSphere(vertsPerRow, ref bottomSurfaceData);
                    }
                }
            }

            return CreateCombinedMesh(bottomSurfaceData, topSurfaceData);
        }

        private Mesh CreateInnerAndOuterSphereSurface(float resolution)
        {
            MeshData outerSurfaceData = new MeshData();
            MeshData innerSurfaceData = new MeshData();
            float cosVal =  ((resolution * resolution) - 2 * (outerRadius * outerRadius)) / (-2 * outerRadius * outerRadius);
            float angleIncr = Mathf.Rad2Deg * Mathf.Acos(cosVal);
            float horizontalInkrement = angleIncr / Vector3.Angle(PointA, PointD);
            float verticalIncrement = angleIncr / Vector3.Angle(PointA, PointB);

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

            return CreateCombinedMesh(innerSurfaceData, outerSurfaceData);

        }

        private Mesh CreateCombinedMesh(MeshData meshDataA, MeshData meshDataB)
        {
            Mesh meshA = new Mesh();
            meshA.vertices = meshDataA.vertices.ToArray();
            meshA.uv = meshDataA.uvCoords.ToArray();
            meshDataA.triangles.Reverse();
            meshA.triangles = meshDataA.triangles.ToArray();

            Mesh meshB = new Mesh();
            meshB.vertices = meshDataB.vertices.ToArray();
            meshB.uv = meshDataB.uvCoords.ToArray();
            meshB.triangles = meshDataB.triangles.ToArray();

            Mesh output = new Mesh();

            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = meshA;
            combine[1].mesh = meshB;

            output.CombineMeshes(combine, true, false);

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
