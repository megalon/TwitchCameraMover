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
        public Vector3 StartPos { get; set; }
        public Vector3 TargetPos { get; set; }
        public float Progress { get; set; }
        public float Duration { get; set; } // duration in seconds for slide to complete

        public SlideAction()
        {
            Duration = 1;
            Progress = 0;
        }

        public SlideAction(Vector3 start, Vector3 target, float duration)
        {
            this.StartPos = start;
            this.TargetPos = target;
            this.Duration = duration;
            this.Progress = 0;
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

        public Vector3 GetPos(float? progress = null)
        {
            return Vector3.Lerp(StartPos, TargetPos, progress == null ? Progress : progress.Value);
        }
    }
}
