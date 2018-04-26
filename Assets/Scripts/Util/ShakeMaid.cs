using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeMaid : MonoBehaviour
{
    public float power = .1f;

    private float timer = .0f;
    private Vector3 lastPos;

    public void Shake(float duration)
    {
        Shake(duration, power, true);
    }

    public void Shake(float duration, float pow)
    {
        Shake(duration, pow, true);
    }

    public void Shake(float duration, float pow, bool resetTimer)
    {
        if (timer <= .0f)
        {
            lastPos = transform.localPosition;
        }
        power = pow;
        if (resetTimer)
        {
            timer = duration;
        }
        else
        {
            timer += duration;
        }
    }

    public void Stop()
    {
        timer = .0f;
        transform.localPosition = lastPos;
    }

    private void FixedUpdate()
    {
        if (timer > .0f)
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0f)
            {
                Stop();
            }
            else
            {
                transform.localPosition = lastPos + new Vector3(Random.Range(-power, power), Random.Range(-power, power));
            }
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Shake(.1f);
        }
    }
}
