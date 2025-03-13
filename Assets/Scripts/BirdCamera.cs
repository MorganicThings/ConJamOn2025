using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Scripts.Airplanes
{
    public class BirdCamera : MonoBehaviour
    {

        [SerializeField] Transform targetTransform;

        Vector3 cameraDeltaPos;

        // Start is called before the first frame update
        void Start()
        {
            cameraDeltaPos = transform.position - targetTransform.position;

            /*if (!Application.isEditor)*/
            
        }

        // Update is called once per frame
        void Update()
        {
            
            transform.position = targetTransform.position + cameraDeltaPos.z * targetTransform.forward + cameraDeltaPos.y * targetTransform.up;
            transform.LookAt(new Vector3(targetTransform.position.x, targetTransform.position.y + cameraDeltaPos.y, targetTransform.position.z));
        }

        private void Awake()
        {
            Assert.IsNotNull(targetTransform, "ERROR olvidaste el target");
        }
    }
}
