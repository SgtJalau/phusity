using UnityEngine;

public class CastUtils
{
    public static bool GetMaterialAtHit(RaycastHit hit, out Material material)
    {
        material = null;
        if (hit.collider.material)
        {
            if (hit.collider.gameObject.TryGetComponent(out MeshFilter mesh) && mesh.mesh.isReadable)
            {
                var index = hit.triangleIndex;
                var count = mesh.mesh.subMeshCount;

                //Debug.Log(index + " | " + count);

                var meshRenderer = hit.collider.GetComponent<MeshRenderer>();
                for (var x = 0; x < count; x++)
                {
                    var triangles = mesh.mesh.GetTriangles(x);

                    for (var y = 0; y < triangles.Length; y++)
                    {
                        if (triangles[y] == index)
                        {
                            //Debug.Log(triangles[y] + " | " + y + " | Mat Index: " + x + " | " +  hit.collider.GetComponent<MeshRenderer>().materials[x + 1]);
                            var materials = meshRenderer.materials;

                            //No idea why the materials are shifted by one but for some reason they are
                            var matIndex = x + 1 < materials.Length ? x + 1 : materials.Length - 1;

                            material = materials[matIndex];
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}