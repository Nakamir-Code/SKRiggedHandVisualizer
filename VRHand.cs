using StereoKit;
using System;

// Adapted from https://github.com/ClonedPuppy/SKHands/blob/master/Platforms/SKHands_DotNet/Program.cs
namespace RiggedHandVisualizer
{
    public class VRHand : ModelWrapper
    {
        private Model handModel;

        class JointInfo
        {
            public ModelNode node;
            public FingerId finger;
            public JointId joint;
            public bool rootBone;
            public JointInfo(FingerId fingerId, JointId jointId, ModelNode fingerNode, bool fingerRootBone)
            {
                finger = fingerId;
                joint = jointId;
                node = fingerNode;
                rootBone = fingerRootBone;
            }
        }

        private JointInfo[] jointInfo;
        private float nodeScale;
        private float rootScale;
        private Quat defaultBoneRot;

        public VRHand(Handed whichHand)
        {
            string modelName = "Hand_" + whichHand.ToString() + ".glb";
            handModel = Model.FromFile(modelName);
            rootScale = 1;
            nodeScale = 1;
            defaultBoneRot = Quat.FromAngles(90, 0, 180);
            var nodes = handModel.Visuals;

            foreach (var node in nodes)
            {
                //making sure the high performaance mat is applied
                node.Material = Material.Default;
            }

            jointInfo = new JointInfo[] {
                // currently SK doesn't have an enum for wrist but populates the thumb root and knucklemajor with the same value,
                // I'm borrowing it here to store the wrist
                new JointInfo(FingerId.Thumb, JointId.Root, null, true),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMajor, null, false),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMid,   null, false),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMinor, null, false),

                new JointInfo(FingerId.Index, JointId.Root,         null, false),
                new JointInfo(FingerId.Index, JointId.KnuckleMajor, null, false),
                new JointInfo(FingerId.Index, JointId.KnuckleMid,   null, false),
                new JointInfo(FingerId.Index, JointId.KnuckleMinor, null, false),

                new JointInfo(FingerId.Middle, JointId.Root,        null, false),
                new JointInfo(FingerId.Middle, JointId.KnuckleMajor,null, false),
                new JointInfo(FingerId.Middle, JointId.KnuckleMid,  null, false),
                new JointInfo(FingerId.Middle, JointId.KnuckleMinor,null, false),

                new JointInfo(FingerId.Ring, JointId.Root,         null, false),
                new JointInfo(FingerId.Ring, JointId.KnuckleMajor, null, false),
                new JointInfo(FingerId.Ring, JointId.KnuckleMid,   null, false),
                new JointInfo(FingerId.Ring, JointId.KnuckleMinor, null, false),

                new JointInfo(FingerId.Little, JointId.Root,        null, false),
                new JointInfo(FingerId.Little, JointId.KnuckleMajor,null, false),
                new JointInfo(FingerId.Little, JointId.KnuckleMid,  null, false),
                new JointInfo(FingerId.Little, JointId.KnuckleMinor,null, false) };


            foreach (JointInfo j in jointInfo)
            {
                if (j.rootBone)
                {
                    string jointName = "Hand.Wrist." + whichHand.ToString();
                    j.node = handModel.FindNode(jointName);

                }
                else
                {
                    string jointName = j.finger.ToString() + "." + j.joint.ToString() + "." + whichHand.ToString();
                    j.node = handModel.FindNode(jointName);
                }

            }


        }

        // Display a hand with the provided joint information
        public void show(HandJoint[] data, bool drawAxis, bool toggleHand, Handed whichHand)
        {
            Pose wrist = Input.Hand(whichHand).wrist;

            foreach (JointInfo j in jointInfo)
            {
                if (drawAxis)
                {
                    Lines.AddAxis(j.node.ModelTransform.Pose);

                }
                if (j.rootBone)
                {
                    j.node.ModelTransform = Matrix.TRS(wrist.position, wrist.orientation * defaultBoneRot, rootScale);

                }
                else
                {
                    Pose joint = GetJoint(data, j.finger, j.joint).Pose;
                    j.node.ModelTransform = Matrix.TRS(joint.position, joint.orientation * defaultBoneRot, nodeScale);
                }

            }

            if (toggleHand)
            {
                handModel.Draw(Matrix.Identity);
            }


        }

        private HandJoint GetJoint(HandJoint[] poses, FingerId finger, JointId joint)
        {
            return poses[5 * (int)finger + (int)joint];
        }

    }
}
