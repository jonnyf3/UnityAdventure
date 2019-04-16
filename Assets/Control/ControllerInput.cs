using UnityEngine;

namespace RPG.Control
{
    public static class ControllerInput
    {
        public const string MOVE_Y_AXIS    = "Vertical";
        public const string MOVE_X_AXIS    = "Horizontal";
        public const string DPAD_X_AXIS    = "DpadHorizontal";
        public const string DPAD_Y_AXIS    = "DpadVertical";
        public const string CAMERA_X_AXIS  = "CameraX";
        public const string CAMERA_Y_AXIS  = "CameraY";

        public const string PAUSE_BUTTON   = "Options";
        public const string ATTACK_BUTTON  = "RightShoulder";
        public const string ABILITY_BUTTON = "RightTrigger";
        public const string FOCUS_BUTTON   = "LeftTrigger";
        public const string ROLL_BUTTON    = "Circle";
        public const string INTERACT_BUTTON = "Triangle";
        public const string SHOW_UI_BUTTON = "R3";

        //Wrapper methods to make DPAD axes behave like discrete buttons
        private static float lastFrameDPADvertical = 0;
        public static bool GetVerticalButtonsDown(out int direction) {
            var thisFrameDPADvertical = Input.GetAxis(DPAD_Y_AXIS);
            direction = 0;

            //Button press same as last frame
            if (thisFrameDPADvertical == lastFrameDPADvertical) {
                return false;
            }
            //Button not pressed this frame
            if (thisFrameDPADvertical == 0) {
                lastFrameDPADvertical = thisFrameDPADvertical;
                return false;
            }
            //Button press same direction as this frame (but different size)
            if (Mathf.Sign(thisFrameDPADvertical) == Mathf.Sign(lastFrameDPADvertical) && lastFrameDPADvertical != 0) {
                lastFrameDPADvertical = thisFrameDPADvertical;
                return false;
            }

            //Any other case should return true
            lastFrameDPADvertical = thisFrameDPADvertical;
            direction = (int)Mathf.Sign(lastFrameDPADvertical);
            return true;
        }

        private static float lastFrameDPADhorizontal = 0;
        public static bool GetHorizontalButtonsDown(out int direction) {
            var thisFrameDPADhorizontal = Input.GetAxis(DPAD_X_AXIS);
            direction = 0;

            //Button press same as last frame
            if (thisFrameDPADhorizontal == lastFrameDPADhorizontal) {
                return false;
            }
            //Button not pressed this frame
            if (thisFrameDPADhorizontal == 0) {
                lastFrameDPADhorizontal = thisFrameDPADhorizontal;
                return false;
            }
            //Button press same direction as this frame (but different size)
            if (Mathf.Sign(thisFrameDPADhorizontal) == Mathf.Sign(lastFrameDPADhorizontal) && lastFrameDPADhorizontal != 0) {
                lastFrameDPADhorizontal = thisFrameDPADhorizontal;
                return false;
            }

            //Any other case should return true
            lastFrameDPADhorizontal = thisFrameDPADhorizontal;
            direction = (int)Mathf.Sign(lastFrameDPADhorizontal);
            return true;
        }
    }
}