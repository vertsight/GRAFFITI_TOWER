using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float angle_1 = 1;
    [SerializeField] float angle_2 = -1;
    [SerializeField] float speed = 1;
    private void Start()
    {
        Rotate();
    }
    private void Rotate()
    {
        transform.DORotate(new Vector3(0,0,angle_1), speed).onComplete = () => 
        {
            transform.DORotate(new Vector3(0,0,angle_2), speed).onComplete = () => 
            {
                Rotate();
            };
        };
    }
}