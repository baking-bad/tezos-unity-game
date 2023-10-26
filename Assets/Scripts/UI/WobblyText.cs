using TMPro;
using UnityEngine;

namespace UI
{
    public class WobblyText : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private float duration;

        private float _duration;

        private void Start()
        {
            _duration = duration;
        }

        // Update is called once per frame
        void Update()
        {
            if (duration <= 0)
            {
                gameObject.SetActive(false);
                duration = _duration;
            }

            textComponent.ForceMeshUpdate();
            var textInfo = textComponent.textInfo;

            for (var i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
            
                if (!charInfo.isVisible)
                    continue;

                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                for (var j = 0; j < 4; j++)
                {
                    var orig = verts[charInfo.vertexIndex + j];
                    verts[charInfo.vertexIndex + j] =
                        orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 10f, 0);
                }
            }

            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                textComponent.UpdateGeometry(meshInfo.mesh, i);
            }

            duration -= Time.deltaTime;
        }
    }
}
