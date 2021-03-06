using UnityEngine;
using System.Collections;


//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Este script realiza el Countdown 3,2,1, GO!
//////////////////////////////

public class CountDownSprite : MonoBehaviour {
    [SerializeField]
    UISprite mySprite;
    [SerializeField]
    int step = 3;
    [SerializeField]
    TweenScale myTweenScale;
    [SerializeField]
    TweenAlpha myTweenAlpha;


    public void StartCountDown()
    {

        if (SoundStore.s != null && GameConfig.s != null)SoundStore.s.PlaySoundByAlias("CountDown",0f,GameConfig.s.SoundVolume);
        myTweenScale.PlayForward();
        myTweenAlpha.PlayForward();
    }

    public void changeSprite()
    {
        step--;
        string s = "Countdown" + step.ToString();
        if (step == 0) s = "CountdownGO";
        
        
        if (step > -1)
        {
            myTweenAlpha.ResetToBeginning();
            myTweenScale.ResetToBeginning();
            mySprite.spriteName = s;
            myTweenScale.PlayForward();
            myTweenAlpha.PlayForward();
            
        }
        else
        {
            if (step == -1)
            {
                step = 3;
                mySprite.spriteName = "Countdown3";
                myTweenAlpha.ResetToBeginning();
                myTweenScale.ResetToBeginning();
                GameController.s.StartGame();
                
            }

        }
        if (step == 0)
        {
            if (SoundStore.s != null && GameConfig.s != null) SoundStore.s.PlaySoundByAlias("CountDownGO", 0f, GameConfig.s.SoundVolume);
        }else
        {
            if (step > 0 && step != 3) if (SoundStore.s != null && GameConfig.s != null) SoundStore.s.PlaySoundByAlias("CountDown", 0f, GameConfig.s.SoundVolume);
        }

        


    }
	


}
