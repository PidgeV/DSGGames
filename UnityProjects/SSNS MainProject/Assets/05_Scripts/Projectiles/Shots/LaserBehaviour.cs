using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    [SerializeField] LineRenderer laserLine;
    [SerializeField] ParticleSystemRenderer laserParticle;
    [SerializeField] LineRenderer line;
    [SerializeField] float laserLength = 500f;
    [SerializeField] float laserDamagePerSecond = 5f;
    public bool fadeIn = false;

    Material[] laserMaterials;
    public float Length { get { return laserLength; } }
    public float Damage { get { return laserDamagePerSecond * Time.deltaTime; } }

    private void Start()
    {
        laserMaterials = new Material[2];
        laserMaterials[0] = laserLine.material;
        laserMaterials[1] = laserParticle.material;

        StartCoroutine(Fade());
    }

    public void SetLaser(Vector3 endPosition)
    {
        line.SetPosition(1, endPosition);
    }

    IEnumerator Fade()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            float fade;
            float fade2;
            if (fadeIn)
            {
                fade = Mathf.Clamp((laserMaterials[0].GetFloat("Vector1_2C1D604B") - 0.5f * Time.deltaTime), 0.5f, 1);
                fade2 = Mathf.Clamp((laserMaterials[0].GetFloat("Vector1_2C1D604B") - 0.7f * Time.deltaTime), 0, 1);
            }
            else
            {
                fade = Mathf.Clamp((laserMaterials[0].GetFloat("Vector1_2C1D604B") + 0.5f * Time.deltaTime), 0.5f, 1);
                fade2 = Mathf.Clamp((laserMaterials[0].GetFloat("Vector1_2C1D604B") + 0.7f * Time.deltaTime), 0, 1);
            }

            laserMaterials[0].SetFloat("Vector1_2C1D604B", fade);
            laserMaterials[1].SetFloat("Vector1_2C1D604B", fade2);

            //Debug.Log("Fading material: " + laserMaterials[0] + " to " + fade);
            //Debug.Log("Fading material: " + laserMaterials[1] + " to " + fade2);
        }
    }
}
