using UnityEngine;
using Cinemachine;

public class CinemachineCamera : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCamera;
    private float DefaultOrthographicSize = 5f;
    private float MaxOrthographicSize = 8f;

    private void Start()
    {
        cinemachineCamera.m_Lens.OrthographicSize = DefaultOrthographicSize;
    }

    private void Update()
    {
        return;
        Player player = FindObjectOfType<Player>();
        float offset = player.velocity / DefaultOrthographicSize;
        if (player.isBoosting)
        {
            if (cinemachineCamera.m_Lens.OrthographicSize >= MaxOrthographicSize)
            {
                SetOrthographicSize(MaxOrthographicSize);
            } else
            {
                SetOrthographicSize(cinemachineCamera.m_Lens.OrthographicSize + 1.5f * Time.deltaTime);
            }
        } else
        {
            if (cinemachineCamera.m_Lens.OrthographicSize > DefaultOrthographicSize)
            {
                SetOrthographicSize(cinemachineCamera.m_Lens.OrthographicSize - 1.5f * Time.deltaTime);
            } else
            {
                SetOrthographicSize(DefaultOrthographicSize);
            }
        }
    }

    public void SetOrthographicSize(float value)
    {
        cinemachineCamera.m_Lens.OrthographicSize = value;
    }

    public float GetRatioOrthographicSize()
    {
        return cinemachineCamera.m_Lens.OrthographicSize / DefaultOrthographicSize;
    }
}
