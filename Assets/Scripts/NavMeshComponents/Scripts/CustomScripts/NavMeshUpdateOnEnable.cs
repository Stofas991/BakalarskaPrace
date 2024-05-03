using UnityEngine;
using UnityEngine.AI;

public class NavMeshUpdateOnEnable : MonoBehaviour
{
    public NavMeshData m_NavMeshData;
    private NavMeshDataInstance m_NavMeshInstance;

    void OnDestroy()
    {
        NavMesh.RemoveNavMeshData(m_NavMeshInstance);
    }
}
