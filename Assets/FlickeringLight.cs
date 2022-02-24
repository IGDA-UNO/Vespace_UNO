using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour {

	public enum WaveForm {sin, tri, sqr, saw, inv, noise}; 
	public WaveForm waveform = WaveForm.sin;   

	public float baseStart = 0.0f; // start 
	public float amplitude = 1.0f; // amplitude of the wave
	public float phase = 0.0f; // start point inside on wave cycle
	public float frequency = 0.5f; // cycle frequency per second
	public float assimetry = 0.2f;

	// Keep a copy of the original color
	private Color originalColor; 
	private Light light;
	private float randomizer;

	// Store the original color
	void Start () {   
		light = GetComponent<Light>(); 
		originalColor = light.color;
		randomizer = 1.0f;
	}

	void Update () {  
		light.color = originalColor * (EvalWave());
	}

	float EvalWave () { 
		float x = (Time.time + phase) * frequency * randomizer;
		float y ;
		x = x - Mathf.Floor(x); // normalized value (0..1)

		if (waveform == WaveForm.sin) {
			float ratio = assimetry;
			float multiplier = 0.5f / (1.0f - ratio);
			if(x<0.2f) {
				y = - Mathf.Cos(x * (0.5f / ratio) * 2 * Mathf.PI);
			} else {
				y = Mathf.Cos((x - ratio) * 2 * multiplier * Mathf.PI);
			}
			
		} 
		else if (waveform == WaveForm.tri) {

			if (x < 0.5f)
				y = 4.0f * x - 1.0f;
			else
				y = -4.0f * x + 3.0f;  
		}      
		else if (waveform == WaveForm.sqr) {

			if (x < 0.5f)
				y = 1.0f;
			else
				y = -1.0f;  
		}      
		else if (waveform == WaveForm.saw) {

			y = x;
		}      
		else if (waveform == WaveForm.inv) {

			y = 1.0f - x;
		}      
		else if (waveform == WaveForm.noise) { 
			if(x < 0.01f) {
				//Debug.Log("x is thero") ; 
				randomizer = 2 * Random.value ; 
			}
			float ratio = assimetry;
			float multiplier = 0.5f / (1.0f - ratio);
			if(x<0.2f) {
				y = - Mathf.Cos(x * (0.5f / ratio) * 2 * Mathf.PI);
			} else {
				y = Mathf.Cos((x - ratio) * 2 * multiplier * Mathf.PI);
			}
		}
		else { 
			y = 1.0f;
		}          
		return (y * amplitude) + baseStart;    
	}
}

