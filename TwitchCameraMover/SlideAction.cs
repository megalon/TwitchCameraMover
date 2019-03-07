using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TwitchCameraMover
{
    public class SlideAction
    {
        public enum SlideMotionType { Linear, Bezier, EaseInOut };

        public Vector3 StartPos { get; set; }
        public Vector3 TargetPos { get; set; }
        public float Progress { get; set; }
        public float Duration { get; set; } // duration in seconds for slide to complete
        public SlideMotionType SlideMotion { get; set; }

        public SlideAction()
        {
            Duration = 1;
            Progress = 0;
            SlideMotion = SlideMotionType.Linear;
        }

        public SlideAction(Vector3 start, Vector3 target, float duration, SlideMotionType slideMotionType = SlideMotionType.Linear)
        {
            this.StartPos = start;
            this.TargetPos = target;
            this.Duration = duration;
            this.Progress = 0;
            this.SlideMotion = slideMotionType;
        }

        public Vector3 MoveAndGetPos(float deltaTime, bool easeInOut)
        {
            if (Duration <= 0) return TargetPos;

            var deltaProgress = deltaTime / Duration;
            if (Progress + deltaProgress > 1) return TargetPos;
            Progress += deltaProgress;

            if (easeInOut)
            {
                float sqt = Progress * Progress;
                //float bezierProg = sqt * (3 - 2 * Progress);
                float prog = sqt / (2.0f * (sqt - Progress) + 1.0f);
                return Vector3.Lerp(StartPos, TargetPos, prog);
            }
            else
            {
                return Vector3.Lerp(StartPos, TargetPos, Progress);
            }

        }
        public float UpdateProgress(float deltaTime)
        {
            if (Duration <= 0) return 1;

            var deltaProgress = deltaTime / Duration;
            if (deltaProgress <= 1)
                Progress += deltaProgress;

            return deltaProgress;
        }

        public Vector3 GetPos(float? progress = null, SlideMotionType slideMotionType = SlideMotionType.Linear)
        {
            var prog = progress == null ? Progress : progress.Value;
            float sqt = prog * prog;

            switch (slideMotionType)
            {
                case SlideMotionType.Bezier:
                    // less curvy
                    // bezier func: x^2(3-2x)     https://www.wolframalpha.com/input/?i=x%5E2(3-2x)+from+0+to+1
                    return Vector3.Lerp(StartPos, TargetPos, sqt * (3 - 2 * prog));
                case SlideMotionType.EaseInOut:
                    // more curvy
                    // easeinout func: x^2/(2*(x^2 -x)+1)   https://www.wolframalpha.com/input/?i=x%5E2%2F(2*(x%5E2+-x)%2B1)+from+0+to+1
                    return Vector3.Lerp(StartPos, TargetPos, sqt / (2.0f * (sqt - prog) + 1.0f));
                default:
                    return Vector3.Lerp(StartPos, TargetPos, prog);
            }
        }
    }
}
