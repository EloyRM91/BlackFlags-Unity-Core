using UnityEngine;

public class OceanicRoute : MonoBehaviour
{
    private static Vector3[] points_IN, points_OUT;
    void Awake()
    {
        points_IN = new Vector3[transform.GetChild(0).childCount];
        points_OUT = new Vector3[transform.GetChild(1).childCount];
        for (byte i = 0; i < points_IN.Length; i++)
        {
            points_IN[i] = transform.GetChild(0).GetChild(i).position;
        }
        for (int i = 0; i < points_OUT.Length; i++)
        {
            points_OUT[i] = transform.GetChild(1).GetChild(i).position;
        }
    }

    public static Vector3 GetOceanicRouteIn(Vector3 origin)
    {
        float bestDis = (origin - points_IN[0]).sqrMagnitude;
        var dest = points_IN[0];
        for (byte i = 0; i < points_IN.Length; i++)
        {
            var dis = (origin - points_IN[i]).sqrMagnitude;
            if (dis < bestDis)
            {
                bestDis = dis;
                dest = points_IN[i];
            }
        }
        return dest;
    }
    public static Vector3 GetOceanicRouteOut(Vector3 origin)
    {
        float bestDis = (origin - points_OUT[0]).sqrMagnitude;
        var dest = points_OUT[0];
        for (byte i = 0; i < points_OUT.Length; i++)
        {
            var dis = (origin - points_OUT[i]).sqrMagnitude;
            if (dis < bestDis)
            {
                bestDis = dis;
                dest = points_OUT[i];
            }
        }
        return dest;
    }
}
