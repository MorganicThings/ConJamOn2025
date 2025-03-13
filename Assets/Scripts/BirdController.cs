using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace Scripts.Airplanes
{
    public class BirdController : MonoBehaviour
    {
        [SerializeField, Range(5f, 1000f), Tooltip("Velocidad avance m/s")] float forwardSpeed = 10f;
        [SerializeField, Range(30f, 760f), Tooltip("Velocidad rotacion grados/s")] float rollSpeed = 90f;
        [SerializeField, Range(20f, 180f), Tooltip("Máxima rotación")] float maxRoll = 90f;
        [SerializeField, Range(0.5f, 5f), Tooltip("Margen de error en la rotación")] float epsilon = 1f;
        //[SerializeField] ParticleSystem particleSystemPrefab;

        [Range(0, 1000)]
        public float xSensitivity = 100f;

        [Range(0, 1000)]
        public float ySensitivity = 100f;


        Rigidbody rb;

        float verticalInput;
        float horizontalInput;


        float mouseX;
        float mouseY;
        float xRotation;    
        float yRotation;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            //Assert.IsNotNull(particleSystemPrefab, "ERROR: olvidaste las partículas");
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            horizontalInput = Input.GetAxisRaw(Constants.HORIZONTAL_AXIS);
            mouseX = Input.GetAxisRaw("Mouse X") * xSensitivity * Time.deltaTime;
            mouseY = Input.GetAxisRaw("Mouse Y") * ySensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -89f, 89f);

            yRotation += mouseX;

            transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(xRotation, yRotation, transform.rotation.eulerAngles.z)));

            verticalInput = Input.GetAxis(Constants.VERTICAL_AXIS);
            horizontalInput = Input.GetAxis(Constants.HORIZONTAL_AXIS);

            UpdateVelocity();
            UpdateRotation();
        }

        private void UpdateVelocity() 
        {
            rb.velocity = forwardSpeed * transform.forward;
        }

        private void UpdateRotation()
        {
            var rotationInput = Mathf.Clamp(-mouseX - horizontalInput, -1f, 1f);
            var eulerZ = transform.rotation.eulerAngles.z;
            float deltaRotation = rollSpeed * Time.deltaTime * rotationInput;

            //Recuperar rotación si no pulsamos la tecla
            if (rotationInput == 0 && !(eulerZ <= epsilon || eulerZ >= 360 - epsilon))
            {
                rotationInput = eulerZ > 0 && eulerZ < maxRoll + epsilon ? -1 : 1;
                deltaRotation = (rollSpeed / 3) * Time.deltaTime * rotationInput;
            }

            // // Creamos límites de rotación
            // if ((horizontalInput > 0 && (eulerZ <= maxRoll || eulerZ >= 360 - maxRoll + epsilon))
            //     || (horizontalInput < 0 && (eulerZ >= 360 - maxRoll || eulerZ <= maxRoll - epsilon)))
            // {
            //     transform.Rotate(rotationInput * rollSpeed * Time.deltaTime * Vector3.forward);
            // }
            if ((rotationInput > 0 || rotationInput < 0)
                && (eulerZ + deltaRotation <= maxRoll || eulerZ + deltaRotation >= 360 - maxRoll))
            {
                transform.Rotate(0, 0, deltaRotation);
            }

        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            GUI.Label(new Rect(10, 0, 0, 0), "Euler Z: " + transform.rotation.eulerAngles.z, style);
            GUI.Label(new Rect(10, 20, 0, 0), "Horizontal Input: " + mouseX, style);
            GUI.Label(new Rect(10, 40, 0, 0), "Vertical Input: " + mouseY, style);
        }
#endif

        internal void Destroy()
        {
        // Instanciar las partículas y las ubico donde el avión
        //var ps = Instantiate(particleSystemPrefab, transform.position, transform.rotation);
        // Hacer que el Airplane sea hijo de las partículas
        //transform.SetParent(ps.transform);
        // deshabilitar el Airplane
        gameObject.SetActive(false);
        }
}
}
