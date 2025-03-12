using UnityEngine;
using UnityEngine.Assertions;

namespace Scripts.Airplanes
{
    public class BirdController : MonoBehaviour
    {
        [SerializeField, Range(5f, 20f), Tooltip("Velocidad avance m/s")] float forwardSpeed = 10f;
        [SerializeField, Range(2f, 10f), Tooltip("Velocidad vertical m/s")] float verticalSpeed = 5f;
        [SerializeField, Range(2f, 10f), Tooltip("Velocidad horizontal m/s")] float horizontalSpeed = 5f;
        [SerializeField, Range(30f, 720f), Tooltip("Velocidad rotacion grados/s")] float rollSpeed = 90f;
        [SerializeField, Min(5f), Tooltip("Mínimo valor x")] float minX = 111.5f;
        [SerializeField, Min(10f), Tooltip("Máximo valor x")] float maxX = 119.8f;
        [SerializeField, Min(5f), Tooltip("Mínimo valor y")] float minY = 7.4f;
        [SerializeField, Min(10f), Tooltip("Máximo valor y")] float maxY = 12.5f;
        [SerializeField, Range(20f, 180f), Tooltip("Máxima rotación")] float maxRoll = 90f;
        [SerializeField, Range(0.1f, 2f)] float turbulences = 1f;
        [SerializeField, Range(0.5f, 5f), Tooltip("Margen de error en la rotación")] float epsilon = 1f;
        //[SerializeField] ParticleSystem particleSystemPrefab;

        float verticalInput;
        float horizontalInput;

        private void Awake()
        {
            //Assert.IsNotNull(particleSystemPrefab, "ERROR: olvidaste las partículas");
        }

        private void Update()
        {
            verticalInput = Input.GetAxis(Constants.VERTICAL_AXIS);
            horizontalInput = Input.GetAxis(Constants.HORIZONTAL_AXIS);

            UpdateTranslation();
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            var rotationInput = -horizontalInput;
            var eulerZ = transform.rotation.eulerAngles.z;
            float deltaRotation = rollSpeed * Time.deltaTime * rotationInput;

            // Recuperar rotación si no pulsamos la tecla
            if (horizontalInput == 0 && !(eulerZ <= epsilon || eulerZ >= 360 - epsilon))
            {
                rotationInput = eulerZ > 0 && eulerZ < maxRoll + epsilon ? -1 : 1;
                deltaRotation = rollSpeed * Time.deltaTime * rotationInput;
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
                transform.Rotate(deltaRotation * Vector3.forward);
            }

        }

        private void UpdateTranslation()
        {
            float deltaVerticalTranslation = Time.deltaTime * verticalInput * verticalSpeed;
            Vector3 verticalTranslation = deltaVerticalTranslation * Vector3.up;
            Vector3 horizontalTranslation = Time.deltaTime * horizontalInput * horizontalSpeed * Vector3.right;
            Vector3 forwardTranslation = Time.deltaTime * forwardSpeed * Vector3.forward;
            //Vector3 turbulenceTranslation = GetTurbulenceTranslation();
            Vector3 translation = forwardTranslation /*+ turbulenceTranslation*/;

            // Movimiento Vertical
            if ((transform.position.y + deltaVerticalTranslation <= maxY && verticalInput > 0) ||
                (transform.position.y + deltaVerticalTranslation >= minY && verticalInput < 0))
            {
                translation += verticalTranslation;
            }

            // Movimiento Horizontal
            if ((transform.position.x <= maxX && horizontalInput > 0) ||
                (transform.position.x >= minX && horizontalInput < 0))
            {
                translation += horizontalTranslation;
            }

            //Debemos realizar las rotaciones usando espacio global y no local
            transform.Translate(translation, Space.World);
        }

        private Vector3 GetTurbulenceTranslation()
        {
            float minRange = transform.position.y <= maxY ? +turbulences : 0f;
            float maxRange = transform.position.y >= minY ? -turbulences : 0f;

            return Random.Range(minRange, maxRange) * Time.deltaTime * Vector3.up;
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            GUI.Label(new Rect(10, 0, 0, 0), "Euler Z: " + transform.rotation.eulerAngles.z, style);
            GUI.Label(new Rect(10, 20, 0, 0), "Horizontal Input: " + horizontalInput, style);
            GUI.Label(new Rect(10, 40, 0, 0), "Vertical Input: " + verticalInput, style);
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
