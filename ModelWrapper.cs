using StereoKit;

namespace RiggedHandVisualizer
{
    interface ModelWrapper
    {
        void show(HandJoint[] data,bool drawAxis,bool toggleHand,Handed whichHand);
    }
}
