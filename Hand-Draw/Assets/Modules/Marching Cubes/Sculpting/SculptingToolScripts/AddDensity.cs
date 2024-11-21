using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AddDensity : MonoBehaviour
{
    public DensityField densityField;
    public Vector3 intPos, tempVector3;
    public Transform handRoot, targetFinger, indexFinger, targetThumb;
    public float scale = 10.0f, sqrtTwoTimesTwo = 2.82842712475f;
    private float distance, distance2;
    private void Awake()
    {
        StartCoroutine("WaitForFinger");
    }

    private IEnumerator WaitForFinger()
    {
        yield return new WaitForSeconds(1.0f);
        targetThumb = handRoot.Find("Bones/Hand_WristRoot/Hand_Thumb0/Hand_Thumb1/Hand_Thumb2/Hand_Thumb3/Hand_ThumbTip");
        indexFinger = handRoot.Find("Bones/Hand_WristRoot/Hand_Index1/Hand_Index2/Hand_Index3/Hand_IndexTip");
        targetFinger = handRoot.Find("Bones/Hand_WristRoot/Hand_Middle1/Hand_Middle2/Hand_Middle3/Hand_MiddleTip");
        if(targetFinger == null || indexFinger == null || targetThumb == null)
            StartCoroutine(WaitForFinger());
    }

    private void FixedUpdate()
    {
        if(targetFinger == null || indexFinger == null || targetThumb == null)
            return;

        if (densityField != null && targetFinger != null)
        {
            for(int xi = 0; xi < densityField.dimension; xi++)
            {
                for(int yi = 0; yi < densityField.dimension; yi++)
                {
                    for(int zi = 0; zi < densityField.dimension; zi++)
                    {
                        tempVector3.x = xi;
                        tempVector3.y = yi;
                        tempVector3.z = zi;
                        distance = Mathf.Abs(Vector3.Distance(targetFinger.position, tempVector3 / 10.0f));
                        if(Vector3.Distance(targetFinger.position, targetThumb.position) < 0.01f && distance < 0.1f)
                                densityField.AddToDensity((0.3f - distance) * Time.deltaTime, xi, yi, zi);
                        distance2 = Mathf.Abs(Vector3.Distance(indexFinger.position, tempVector3 / 10.0f));

                        if (Vector3.Distance(indexFinger.position, targetThumb.position) < 0.02f && distance2 < 0.1f)
                            densityField.AddToDensity((distance2 - 0.4f) * Time.deltaTime, xi, yi, zi);
                    }
                }
            }
        }
    }
}
