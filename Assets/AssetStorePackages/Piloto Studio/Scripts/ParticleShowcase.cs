using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PilotoStudio
{
    public class ParticleShowcase : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> particles = new List<GameObject>();
        [SerializeField]
        private int currentlyActive = 0;
        [SerializeField]
        private Text displayName;

        private void Start()
        {
            foreach (Transform t in this.transform)
            {
                particles.Add(t.gameObject);
            }
            UpdateDisplayName();
            particles[currentlyActive].SetActive(true);
        }

        void UpdateDisplayName()
        {
            displayName.text = particles[currentlyActive].name;

        }


        public void ActivateNext()
        {
            if (currentlyActive + 1 >= particles.Count)
            {
                particles[currentlyActive].SetActive(false);
                currentlyActive = 0;
                particles[currentlyActive].SetActive(true);
            }
            else
            {
                particles[currentlyActive].SetActive(false);
                currentlyActive++;
                particles[currentlyActive].SetActive(true);
            }

            UpdateDisplayName();
        }

        public void ActivatePrevious()
        {
            if (currentlyActive - 1 < 0)
            {
                particles[currentlyActive].SetActive(false);
                currentlyActive = (particles.Count - 1);
                particles[currentlyActive].SetActive(true);
            }
            else
            {
                particles[currentlyActive].SetActive(false);
                currentlyActive--;
                particles[currentlyActive].SetActive(true);
            }

            UpdateDisplayName();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ActivatePrevious();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                ActivateNext();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                particles[currentlyActive].GetComponent<ParticleSystem>().Play();
            }
        }

    }
}