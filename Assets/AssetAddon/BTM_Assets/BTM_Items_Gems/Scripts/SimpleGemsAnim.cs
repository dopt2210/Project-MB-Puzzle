namespace Benjathemaker
{
    using UnityEngine;

    public class SimpleGemsAnim : MonoBehaviour
    {
        [Header("Rotation")]
        public bool isRotating;
        public Vector3 rotationAxis = Vector3.up; // Gộp 3 trục lại
        public float rotationSpeed = 90f;

        [Header("Floating")]
        public bool isFloating;
        public bool useEasingForFloating;
        public float floatHeight = 1f;
        public float floatSpeed = 1f;

        [Header("Scaling")]
        public bool isScaling;
        public bool useEasingForScaling;
        public Vector3 startScale = Vector3.one;
        public Vector3 endScale = Vector3.one * 1.5f;
        public float scaleLerpSpeed = 1f;

        private Vector3 initialPosition;
        private Vector3 initialScale;
        private float floatTimer;
        private float scaleTimer;

        void Start()
        {
            initialPosition = transform.position;
            initialScale = transform.localScale;

            // Nếu startScale không được set, gán bằng scale ban đầu
            if (startScale == Vector3.zero)
                startScale = initialScale;

            // endScale được scale theo tỉ lệ từ startScale nếu chưa set
            if (endScale == Vector3.zero)
                endScale = startScale * 1.5f;
        }

        void Update()
        {
            if (isRotating)
                HandleRotation();

            if (isFloating)
                HandleFloating();

            if (isScaling)
                HandleScaling();
        }

        void HandleRotation()
        {
            transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime);
        }

        void HandleFloating()
        {
            floatTimer += Time.deltaTime * floatSpeed;
            float t = Mathf.PingPong(floatTimer, 1f);
            if (useEasingForFloating)
                t = EaseInOutQuad(t);

            Vector3 offset = Vector3.up * (t * floatHeight);
            transform.position = initialPosition + offset;
        }

        void HandleScaling()
        {
            scaleTimer += Time.deltaTime * scaleLerpSpeed;
            float t = Mathf.PingPong(scaleTimer, 1f);
            if (useEasingForScaling)
                t = EaseInOutQuad(t);

            transform.localScale = Vector3.Lerp(startScale, endScale, t);
        }

        float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        }
    }

}

