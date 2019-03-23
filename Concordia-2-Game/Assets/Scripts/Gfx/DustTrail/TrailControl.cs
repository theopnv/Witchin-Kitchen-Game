using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class TrailControl : MonoBehaviour
    {
        public GameObject Target;

        [Range(0.0f, 50.0f)]
        public float MaxSpeed = 1.0f;

        [Range(0.0f, 1.0f)]
        public float SpeedCutoff = 0.1f;

        public AnimationCurve Falloff;

        protected ParticleSystem Particles;
        protected Rigidbody TargetBody;

        protected float StartSize;
        protected float StartSpeed;

        // Start is called before the first frame update
        void Start()
        {
            Particles = GetComponent<ParticleSystem>();
            TargetBody = Target.GetComponent<Rigidbody>();

            StartSize = Particles.main.startSize.constant;
            StartSpeed = Particles.main.startSpeed.constant;
        }

        // Update is called once per frame
        void Update()
        {
            if (TargetBody != null)
            {
                var fwd = Quaternion.AngleAxis(45.0f, TargetBody.transform.right) * -TargetBody.velocity;
                if (fwd != Vector3.zero) // Avoid spamming the logger with https://answers.unity.com/questions/397425/look-rotation-viewing-vector-is-zero-error.html
                {
                    Particles.gameObject.transform.forward = fwd;
                }

                var speed = TargetBody.velocity.magnitude;
                var falloff = Falloff.Evaluate(Mathf.Clamp01(speed / MaxSpeed));

                ParticleSystem.MainModule particlesMain = Particles.main;
                particlesMain.startSize = StartSize * falloff;
                particlesMain.startSpeed = StartSpeed * falloff;

                ParticleSystem.EmissionModule particlesEmission = Particles.emission;
                particlesEmission.enabled = speed > SpeedCutoff;
            }
        }
    }
}
