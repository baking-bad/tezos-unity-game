using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingScript : MonoBehaviour
{
    private float _intensity;
    private Vignette _vignette;

    private PlayerController _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        _player.healthChanged += ChangeHealth;
        
        _player.GetComponentInChildren<PostProcessVolume>()
            .profile
            .TryGetSettings<Vignette>(out _vignette);

        if (!_vignette)
        {
            Debug.LogError("Post processing error: vignette empty");
        }
        else
        {
            _vignette.enabled.Override(false );
        }
    }

    
    private void ChangeHealth(float _, bool damaged)
    {
        if (!damaged) return;
        
        StartCoroutine(DamageEffect());
    }

    private IEnumerator DamageEffect()
    {
        _vignette.enabled.Override(true);
        _vignette.intensity.Override(0.4f);
        yield return new WaitForSeconds(0.4f);

        while (_intensity > 0)
        {
            _intensity -= 0.01f;
            if (_intensity < 0) _intensity = 0;
            _vignette.intensity.Override(_intensity);
            yield return new WaitForSeconds(0.1f);
        }
        
        _vignette.enabled.Override(false);
        yield break;
    }
}
