using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class optimizeMeshes : MonoBehaviour {


    public void CombineMeshes()
    {
        MeshFilter[] mFilt = GetComponentsInChildren<MeshFilter>() ;
        CombineInstance[] combine = new CombineInstance[mFilt.Length];
        for(int i = 0; i < mFilt.Length;i++)
        {
            combine[i].mesh = mFilt[i].sharedMesh;
            combine[i].transform = mFilt[i].transform.localToWorldMatrix;
            if(mFilt[i].gameObject.tag == "Wall")
            {
                mFilt[i].gameObject.active = false;
            }
         //   mFilt[i].gameObject.active = false;
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false);
        transform.gameObject.active = true;

    }
}
