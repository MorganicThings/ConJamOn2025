using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Scripts.Airplanes
{
    public class BirdCamera : MonoBehaviour
    {
        [SerializeField] Transform targetTransform;
        Vector3 offset;

        private void Awake()
        {
            Assert.IsNotNull(targetTransform, "ERROR olvidaste el target");
            offset = this.transform.position - targetTransform.position;
        }

        private void LateUpdate()
        {
            // Al profe no le gusta un pelo comparar con null
            if (targetTransform != null)
            {
                // Conservamos la posición x e y de la cámara pero modificamos la z
                transform.position = new Vector3(
                    x: transform.position.x,
                    y: transform.position.y,
                    z: targetTransform.position.z + offset.z);
            }
        }
    }
}
