using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PicaVoxel;



[RequireComponent(typeof(RoadEnt))]
public class PurpleRotationRoad : MonoBehaviour
{

    #region Attributes
    [Header("Purple Road Config")]
    [SerializeField]
    RoadPositionPurple AnchoredSide;
    [SerializeField]
    RoadPositionPurple StartingSide;

    int AnchorSideRotations = -1;

    int NumberOfClicks = 0;
    //bool InTransition = false;
    [SerializeField]
    RoadPositionPurple CurrentPurplePosition = RoadPositionPurple.None;

    [Header("References for High")]
    [SerializeField]
    Volume HighRoad;
    [SerializeField]
    Volume PurpleTopRoad;

    [Header("References for Low")]
    [SerializeField]
    SpriteRenderer LowRoad;
    [SerializeField]
    SpriteRenderer LowPurpleTopRoad;
    [SerializeField]
    SpriteRenderer BackPurpleRoad;

    [Header("References for Animator")]
    [SerializeField]
    Transform RotateTransform;
    [SerializeField]
    Animator MoveAnimator;
    [SerializeField]
    Animator PurpleInnerAnimator;
    #endregion

    #region Properties    
    GraphicQualitySettings QualitySettings
    {
        get
        {
            GraphicQualitySettings Candidate = GraphicQualitySettings.High;
            if (sProfileManager.s != null)
            {
                if (sProfileManager.ProfileSingleton != null)
                {
                    Candidate = sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings;
                }
            }
            if (GameController.s != null) Candidate = GameController.s.defaultQSettings;

            //We check if References are DEAD we switch our Quality.
            switch (Candidate)
            {
                case GraphicQualitySettings.Low:
                    if ((LowRoad == null || LowPurpleTopRoad == null) && (HighRoad != null && PurpleTopRoad != null)) Candidate = GraphicQualitySettings.High;
                    break;
                case GraphicQualitySettings.Medium:
                    if ((LowRoad == null || LowPurpleTopRoad == null) && (HighRoad != null && PurpleTopRoad != null)) Candidate = GraphicQualitySettings.High;
                    break;
                case GraphicQualitySettings.High:
                    if ((HighRoad == null || PurpleTopRoad == null) && (LowRoad != null && LowPurpleTopRoad != null)) Candidate = GraphicQualitySettings.Medium;
                    break;
            }
            return Candidate;
        }
    }
    RoadDirection _myDirection;
    public RoadDirection myDirection
    {
        get
        {
            return myRoadEnt.myDirection;
        }

        set
        {
            myRoadEnt.myDirection = value;
            MapController.s.PurpleTileClicked(myRoadEnt.position);
        }
    }

    RoadEnt _myRoadEnt;
    private RoadEnt myRoadEnt
    {
        get
        {
            if (_myRoadEnt == null) _myRoadEnt = GetComponent<RoadEnt>();
            return _myRoadEnt;
        }
    }

    private List<TruckEnt> _TruckOnTop;
    private List<TruckEnt> TruckOnTop
    {
        get
        {
            if (myRoadEnt.OnTopTrucks.Count == 0) return myRoadEnt.OnTopTrucks;
            RoadPositionPurple current = CurrentPurplePosition;

            List<TruckEnt> temp = new List<TruckEnt>();
            _TruckOnTop = new List<TruckEnt>();

            if (current == AnchoredSide) current = current.Reverse();
            /*
            //MAIN SWITCH
            switch (AnchoredSide)
            {
                case RoadPositionPurple.N:
                    #region North
                    switch (CurrentPurplePosition)
                    {
                        case RoadPositionPurple.E:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DeltaXMoreThanRoad && q.DeltaYLessThanRoad);
                            break;
                        case RoadPositionPurple.S:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => (q.DistanceToMyRoad().y < 0f));
                            break;
                        case RoadPositionPurple.W:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DeltaXLessThanRoad && q.DeltaYLessThanRoad);
                            break;
                    }
                    #endregion
                    break;
                case RoadPositionPurple.E:
                    #region East
                    switch (CurrentPurplePosition)
                    {
                        case RoadPositionPurple.N:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DeltaXLessThanRoad && q.DeltaYMoreThanRoad);
                            break;
                        case RoadPositionPurple.S:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DeltaXLessThanRoad && q.DeltaYLessThanRoad);
                            break;
                        case RoadPositionPurple.W:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DistanceToMyRoad().x < 0f);
                            break;
                    }
                    #endregion
                    break;
                case RoadPositionPurple.S:
                    #region South
                    switch (CurrentPurplePosition)
                    {
                        case RoadPositionPurple.N:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DistanceToMyRoad().y > 0f);
                            break;
                        case RoadPositionPurple.E:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DeltaXMoreThanRoad && q.DeltaYMoreThanRoad);
                            break;
                        case RoadPositionPurple.W:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DeltaXLessThanRoad && q.DeltaYMoreThanRoad);
                            break;
                    }
                    #endregion
                    break;
                case RoadPositionPurple.W:
                    #region West
                    switch (CurrentPurplePosition)
                    {
                        case RoadPositionPurple.N:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DeltaXMoreThanRoad && q.DeltaYMoreThanRoad);
                            break;
                        case RoadPositionPurple.E:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DistanceToMyRoad().x > 0f);
                            break;
                        case RoadPositionPurple.S:
                            temp = myRoadEnt.OnTopTrucks.FindAll(q => q.DeltaXMoreThanRoad && q.DeltaYLessThanRoad);
                            break;
                    }
                    #endregion
                    break;
            }
           */
            foreach(TruckEnt te in myRoadEnt.OnTopTrucks)
            {
                bool inc = false;
                //If direction of truck is the same as the Anchored position or reverse
                //we calculate if it is standing in the moving part or not
                if (te.Direction == AnchoredSide.ToCardinalPoint() || te.Direction == AnchoredSide.ToCardinalPoint().Reverse())
                {
                    switch (AnchoredSide)
                    {
                        case RoadPositionPurple.N:
                            if (te.DeltaYLessThanRoad) inc = true;
                            break;
                        case RoadPositionPurple.E:
                            if (te.DeltaXLessThanRoad) inc = true;
                            break;
                        case RoadPositionPurple.W:
                            if (te.DeltaXMoreThanRoad) inc = true;
                            break;
                        case RoadPositionPurple.S:
                            if (te.DeltaYMoreThanRoad) inc = true;
                            break;
                    }
                }
                //If the direction of the truck isnt the same as the anchored road 
                //we always include this truck
                else
                {
                    inc = true;
                }

                //finally we include the truck to the list if it proceeds
                if (inc)
                {
                    temp.Add(te);
                }
            }




                _TruckOnTop = temp;
                return _TruckOnTop;


           
        }
    }

    string lowpath
    {
        get
        {
            if (GameConfig.s != null)
            {
                return GameConfig.s.LowIMGPath;
            }
            return "IMG\\LowIMGs\\";
        }
    }

    #endregion

    #region Initialization
    void Awake()
    {
        RoadInit();

        _TruckOnTop = new List<TruckEnt>();


    }

    public void RoadInit() {
        //We fix StartingSide to avoid it being the same than the Anchor
        if (StartingSide == AnchoredSide) StartingSide = StartingSide.TurnRight();

        //We move the MOVEanimator X times depending on the Starting Road
        AnchorSideRotations = ((int)AnchoredSide) - 1; //-1 because we start at 0 (north is 0)

        //-----
        //For now on we will use this number (AnchorSideRotations) 
        //to Rotate our rotations acordingly to the TopAnimator.
        //-----

        //we Move the TopAnimator.
        MoveAnimatorToRotation(AnchorSideRotations);
        CurrentPurplePosition = StartingSide;
        MoveInnerAnimatorTo(CurrentPurplePosition, AnchorSideRotations, true);


        
    }

    /// <summary>
    /// Moves de Top Animator to a specific rotation.
    /// </summary>
    /// <param name="theRotation"></param>
    void MoveAnimatorToRotation(int theRotation)
    {
        //We want to move it instant
        MoveAnimator.SetBool("NoAnim", true);
        //Starting Rotation for MoveAnimator is NE (North, now for us)
        if (theRotation == 0) return;
        //East is 1
        if (theRotation == 1) MoveAnimator.SetBool("GoToSE", true);
        //South is 2
        if (theRotation == 2) MoveAnimator.SetBool("GoToSW", true);
        //West is 3
        if (theRotation == 3) MoveAnimator.SetBool("GoToNW", true);
    }

    void MoveInnerAnimatorTo(RoadPositionPurple position, int offset, bool instant = false)
    {
        if (instant) PurpleInnerAnimator.SetBool("NoAnim", true);

        //The inner animator directions are rotated by the top animator, we have
        //to translate it.
        RoadPositionPurple InnerPosition = position.TurnLeft(offset);
        //North is always occupied by the anchored position
        if (InnerPosition == RoadPositionPurple.N) InnerPosition.TurnRight();

        switch (InnerPosition)
        {
            case RoadPositionPurple.E:
                PurpleInnerAnimator.SetBool("GoToThree", true);
                break;
            case RoadPositionPurple.S:
                PurpleInnerAnimator.SetBool("GoToTwo", true);
                break;
            case RoadPositionPurple.W:
                PurpleInnerAnimator.SetBool("GoToOne", true);
                break;
        }


    }



    /*

    RoadDirection BuildRoadDirection()
    {

    }
    */



    #endregion

    /// <summary>
    /// Outside Click
    /// Simulate a click from another script
    /// </summary>
    /// <param name="isPressed"></param>

    public void SimulateClick()
    {
        OnPress(true);
    }

    void OnPress(bool isPressed)
    {
        //Failure states.
        if (!isPressed) return; //Required by NGUI
        NumberOfClicks += 1;

        if (NumberOfClicks == 1)
        {
            TriggerAnimation();


        }
    }

    void TriggerAnimation()
    {

        foreach (TruckEnt te in TruckOnTop)
        {
            //In case of multiple clicks we want to store the first "oldDirection" of each truck
            //For that, oldDirection will go back to "None" when the process is complete, so we only change when it's "None"  
            if (te.oldDirection == CardinalPoint.None) te.oldDirection = te.Direction;
            //Once current direction is saved onto oldDirection we are free to Stop the truck on it's tracks
            te.Direction = CardinalPoint.None;
            //We reduce the speed of the truck
            te.CurrentSpeed = 0.1f;
        }
        PurpleInnerAnimator.SetBool("Next", true);
    }

    public void StartAnimation(bool Upwards)
    {
        //if (!twoState) Upwards = !Upwards;

        //We have Trucks OnTop
        if (TruckOnTop.Count != 0)
        {
            //parent 'em
            foreach (TruckEnt te in TruckOnTop)
            {
                if (te.transform.parent == null) te.transform.SetParent(RotateTransform);
                te.ApplyNoRotationEffect();
            }
        }
        if (Upwards) CurrentPurplePosition = CurrentPurplePosition.TurnLeft();
        if (!Upwards) CurrentPurplePosition = CurrentPurplePosition.TurnRight();
    }
    public void EndAnimation(bool Upwards, bool twoState)
    {
        //We have trucks on top (calculated before, on the TruckOnTop of StartAnimation)
        //calling _TruckOnTop returns the previously trucks on top calculated array.
        //So, if movement of animator denies any trucks of being "OnTop" they still do end animation
        if (_TruckOnTop.Count != 0)
        {
            foreach (TruckEnt te in myRoadEnt.OnTopTrucks)
            {
                if (te.transform.parent == RotateTransform)
                {
                    //We unparent them
                    te.transform.SetParent(null);

                    //In this case the vehicle started turning already on its own.
                    //This means the number of rotations needed may change
                    if (te.oldDirection != te.DirectionWhenEnteredRoad)
                    {
                        //the truck is now destined to go through the Anchored part
                        //This avoid any more rotations
                        if (te.oldDirection == AnchoredSide.ToCardinalPoint())
                        {
                            te.oldDirection = CardinalPoint.None;
                            te.NumberOfGreenRotations = 0;
                            te.Direction = AnchoredSide.ToCardinalPoint();
                        }else
                        {

                        }


                    }
                    
                    //Now we start the truck by given it it's calculated new direction

                    
                    if (te.oldDirection != CardinalPoint.None)
                    {
                        if (!twoState) Upwards = !Upwards;
                        if (Upwards) te.Point = te.oldDirection.TurnLeft();
                        if (!Upwards) te.Point = te.oldDirection.TurnRight();
                    }
                    





                    //we reset the oldDirection so the truck can endure this process again
                    te.oldDirection = CardinalPoint.None;
                    //we call this to Fix rotations on the truck and avoid float and animator minor shifts
                    te.FinishRotation();
                }
            }
            foreach (TruckEnt te in gameObject.GetComponentsInChildren<TruckEnt>())
            {
                te.gameObject.transform.SetParent(null);
                
            }

        }
        foreach (TruckEnt te in myRoadEnt.OnTopTrucks)
        {
            /*if (!_TruckOnTop.Find(x => x == te) || AnchoredSide == CurrentPurplePosition.Reverse())*/ te.FireOnTruckDirectionChanged(true, false);
        }

        MapController.s.PurpleTileClicked(myRoadEnt.position);



        NumberOfClicks -= 1;
        if (NumberOfClicks < 0)
        {
            NumberOfClicks = 0;
        }
        else if (NumberOfClicks > 0)
        {
            TriggerAnimation();
        }




    }

    #region Finish Movement from Animator

    public void UpdateRoadDirection(RoadDirection RD)
    {
        //we ensure it has been INITIALIZED (AnchorSideRotation defualt state is -1)
        if (AnchorSideRotations != -1)
        {
            //we translate the rotation given in this whole script in a 
            //Truck oriente rotation (north IS north)
            //Depending of Rotations given to the TOP Animator at the begining of the script (see awake)
            myDirection = RD.TurnRight(AnchorSideRotations);
        }
        else
        {
            myDirection = RD;
        }
    }

    #endregion


    #region SetFrames   
    //These Methods update the PicaVoxel Volume OR the Sprite Renderer
    //while the animation is happening
    public void SetPurpleRoadFrame(int frame, bool upwards)
    {
        string backname = "";
        string filename = "sprite_TopPurple_" + (frame + 1).ToString();
        if (frame == 0) backname = "sprite_NS";
        if (frame == 1)
        {
            if (upwards) backname = "sprite_NW";
            if (!upwards) backname = "sprite_NE";
        }
        if (frame == 2) backname = "sprite_NS";

        if (frame > 2 || frame < 0) return;

        switch (QualitySettings)
        {
            case GraphicQualitySettings.Low:
                LowPurpleTopRoad.sprite = (Sprite)Resources.Load<Sprite>(lowpath + filename);
                BackPurpleRoad.sprite = (Sprite)Resources.Load<Sprite>(lowpath + backname);
                break;
            case GraphicQualitySettings.Medium:
                LowPurpleTopRoad.sprite = (Sprite)Resources.Load<Sprite>(lowpath + filename);
                BackPurpleRoad.sprite = (Sprite)Resources.Load<Sprite>(lowpath + backname);
                break;
            case GraphicQualitySettings.High:
                PurpleTopRoad.SetFrame(frame);
                break;
            default:
                break;
        }
    }
    public void SetMainFrame(int frame)
    {
        string filename = "sprite_InnerPurple_" + (frame + 1).ToString();
        switch (QualitySettings)
        {
            case GraphicQualitySettings.None:
                break;
            case GraphicQualitySettings.Low:
                LowRoad.sprite = (Sprite)Resources.Load<Sprite>(lowpath + filename);
                break;
            case GraphicQualitySettings.Medium:
                LowRoad.sprite = (Sprite)Resources.Load<Sprite>(lowpath + filename);
                break;
            case GraphicQualitySettings.High:
                HighRoad.SetFrame(frame);
                break;
            default:
                break;
        }
    }
    #endregion

}