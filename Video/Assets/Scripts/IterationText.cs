using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum IterationTextState
{
    STATE_DISAPPER,
    STATE_ANIM,
    STATE_END
}

public class IterationText : MonoBehaviour
{
    public Text IterationT;

    public float StartTime;
    public float EndTime;
    public float counter;
    private IterationTextState state = IterationTextState.STATE_DISAPPER;

    private int EndIteration = 10000;

	void Update ()
    {
        counter += Time.deltaTime;
        if (counter >= StartTime)
            state = IterationTextState.STATE_ANIM;
        if (counter >= EndTime)
            state = IterationTextState.STATE_END;

        switch (state)
        {
            case IterationTextState.STATE_DISAPPER:
                {
                    IterationT.text = "Iteration 0";
                }
                break;
            case IterationTextState.STATE_ANIM:
                {
                    float number = Mathf.Lerp(0, EndIteration, (counter - StartTime) / (EndTime - StartTime));
                    IterationT.text = "Iteration " + Mathf.Round(number).ToString();
                }
                break;
            case IterationTextState.STATE_END:
                {
                    IterationT.text = "Iteration " + EndIteration.ToString();
                }
                break;
        }
        
	}
}
