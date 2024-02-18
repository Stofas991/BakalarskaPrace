using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

public class NavMeshUpdateOnEnable : MonoBehaviour
{
    public NavMeshData m_NavMeshData;
    private NavMeshDataInstance m_NavMeshInstance;

    void OnDestroy()
    {
        NavMesh.RemoveNavMeshData(m_NavMeshInstance);
    }
}
